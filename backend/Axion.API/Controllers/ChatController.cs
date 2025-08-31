using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axion.API.Data;
using Axion.API.Models;
using Axion.API.DTOs;
using System.Security.Claims;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
  private readonly AxionDbContext _context;
  private readonly ILogger<ChatController> _logger;

  public ChatController(AxionDbContext context, ILogger<ChatController> logger)
  {
    _context = context;
    _logger = logger;
  }

  // GET: api/chat/rooms
  [HttpGet("rooms")]
  public async Task<ActionResult<IEnumerable<ChatRoomListResponse>>> GetChatRooms()
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var rooms = await _context.ChatRooms
        .Include(r => r.Participants)
        .Include(r => r.Project)
        .Include(r => r.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
        .Where(r => r.Participants.Any(p => p.Id == userId) || !r.IsPrivate)
        .Select(r => new ChatRoomListResponse
        {
          Id = r.Id,
          Name = r.Name,
          Type = r.Type.ToString(),
          ProjectId = r.ProjectId,
          ProjectName = r.Project?.Name,
          IsPrivate = r.IsPrivate,
          ParticipantCount = r.Participants.Count,
          UnreadCount = 0, // TODO: Implement read status tracking
          LastMessageAt = r.Messages.FirstOrDefault()?.CreatedAt,
          LastMessage = r.Messages.FirstOrDefault() != null ? new MessageResponse
          {
            Id = r.Messages.First().Id,
            Content = r.Messages.First().Content,
            SenderId = r.Messages.First().SenderId,
            SenderName = r.Messages.First().SenderName,
            Type = r.Messages.First().Type.ToString(),
            CreatedAt = r.Messages.First().CreatedAt
          } : null
        })
        .OrderByDescending(r => r.LastMessageAt)
        .ToListAsync();

    return rooms;
  }

  // GET: api/chat/rooms/{id}
  [HttpGet("rooms/{id}")]
  public async Task<ActionResult<ChatRoomResponse>> GetChatRoom(string id)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var room = await _context.ChatRooms
        .Include(r => r.Participants)
        .Include(r => r.Project)
        .Include(r => r.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
        .FirstOrDefaultAsync(r => r.Id == id);

    if (room == null) return NotFound();

    // Check if user has access
    if (room.IsPrivate && !room.Participants.Any(p => p.Id == userId))
      return Forbid();

    var response = new ChatRoomResponse
    {
      Id = room.Id,
      Name = room.Name,
      Description = room.Description,
      Type = room.Type.ToString(),
      ProjectId = room.ProjectId,
      ProjectName = room.Project?.Name,
      IsPrivate = room.IsPrivate,
      IsArchived = room.IsArchived,
      ParticipantCount = room.Participants.Count,
      MessageCount = room.Messages.Count,
      CreatedAt = room.CreatedAt,
      UpdatedAt = room.UpdatedAt,
      Participants = room.Participants.Select(p => new UserResponse
      {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Email = p.Email,
        Role = p.Role.ToString()
      }).ToList(),
      LastMessage = room.Messages.FirstOrDefault() != null ? new MessageResponse
      {
        Id = room.Messages.First().Id,
        Content = room.Messages.First().Content,
        SenderId = room.Messages.First().SenderId,
        SenderName = room.Messages.First().SenderName,
        Type = room.Messages.First().Type.ToString(),
        CreatedAt = room.Messages.First().CreatedAt
      } : null
    };

    return response;
  }

  // POST: api/chat/rooms
  [HttpPost("rooms")]
  public async Task<ActionResult<ChatRoomResponse>> CreateChatRoom(CreateChatRoomRequest request)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var room = new ChatRoom
    {
      Name = request.Name,
      Description = request.Description,
      Type = Enum.Parse<RoomType>(request.Type),
      ProjectId = request.ProjectId,
      IsPrivate = request.IsPrivate,
      OrganizationId = GetUserOrganizationId()
    };

    _context.ChatRooms.Add(room);

    // Add participants
    var participants = await _context.Users
        .Where(u => request.ParticipantIds.Contains(u.Id))
        .ToListAsync();

    foreach (var participant in participants)
    {
      room.Participants.Add(participant);
    }

    // Add creator as participant
    var creator = await _context.Users.FindAsync(userId);
    if (creator != null)
    {
      room.Participants.Add(creator);
    }

    await _context.SaveChangesAsync();

    var response = new ChatRoomResponse
    {
      Id = room.Id,
      Name = room.Name,
      Description = room.Description,
      Type = room.Type.ToString(),
      ProjectId = room.ProjectId,
      IsPrivate = room.IsPrivate,
      IsArchived = room.IsArchived,
      ParticipantCount = room.Participants.Count,
      MessageCount = 0,
      CreatedAt = room.CreatedAt,
      UpdatedAt = room.UpdatedAt,
      Participants = room.Participants.Select(p => new UserResponse
      {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Email = p.Email,
        Role = p.Role.ToString()
      }).ToList()
    };

    return CreatedAtAction(nameof(GetChatRoom), new { id = room.Id }, response);
  }

  // PUT: api/chat/rooms/{id}
  [HttpPut("rooms/{id}")]
  public async Task<IActionResult> UpdateChatRoom(string id, UpdateChatRoomRequest request)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var room = await _context.ChatRooms
        .Include(r => r.Participants)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (room == null) return NotFound();

    // Check if user is participant (for private rooms) or admin
    if (room.IsPrivate && !room.Participants.Any(p => p.Id == userId))
      return Forbid();

    room.Name = request.Name;
    room.Description = request.Description;
    room.IsPrivate = request.IsPrivate;
    room.IsArchived = request.IsArchived;
    room.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return NoContent();
  }

  // GET: api/chat/rooms/{id}/messages
  [HttpGet("rooms/{id}/messages")]
  public async Task<ActionResult<IEnumerable<MessageResponse>>> GetRoomMessages(
      string id,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 50)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var room = await _context.ChatRooms
        .Include(r => r.Participants)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (room == null) return NotFound();

    if (room.IsPrivate && !room.Participants.Any(p => p.Id == userId))
      return Forbid();

    var messages = await _context.Messages
        .Include(m => m.Sender)
        .Include(m => m.ReplyToMessage)
        .Include(m => m.Replies)
        .Where(m => m.RoomId == id && m.DeletedAt == null)
        .OrderByDescending(m => m.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(m => new MessageResponse
        {
          Id = m.Id,
          Content = m.Content,
          SenderId = m.SenderId,
          SenderName = m.SenderName,
          Type = m.Type.ToString(),
          RoomId = m.RoomId,
          ProjectId = m.ProjectId,
          TaskId = m.TaskId,
          ReplyToMessageId = m.ReplyToMessageId,
          Mentions = m.Mentions,
          Attachments = m.Attachments,
          IsEdited = m.IsEdited,
          CreatedAt = m.CreatedAt,
          UpdatedAt = m.UpdatedAt,
          ReplyToMessage = m.ReplyToMessage != null ? new MessageResponse
          {
            Id = m.ReplyToMessage.Id,
            Content = m.ReplyToMessage.Content,
            SenderId = m.ReplyToMessage.SenderId,
            SenderName = m.ReplyToMessage.SenderName,
            Type = m.ReplyToMessage.Type.ToString(),
            CreatedAt = m.ReplyToMessage.CreatedAt
          } : null
        })
        .ToListAsync();

    return messages;
  }

  // POST: api/chat/rooms/{id}/join
  [HttpPost("rooms/{id}/join")]
  public async Task<IActionResult> JoinRoom(string id)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var room = await _context.ChatRooms
        .Include(r => r.Participants)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (room == null) return NotFound();

    if (room.IsPrivate && !room.Participants.Any(p => p.Id == userId))
      return Forbid();

    var user = await _context.Users.FindAsync(userId);
    if (user == null) return NotFound();

    if (!room.Participants.Any(p => p.Id == userId))
    {
      room.Participants.Add(user);
      await _context.SaveChangesAsync();
    }

    return NoContent();
  }

  // POST: api/chat/rooms/{id}/leave
  [HttpPost("rooms/{id}/leave")]
  public async Task<IActionResult> LeaveRoom(string id)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var room = await _context.ChatRooms
        .Include(r => r.Participants)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (room == null) return NotFound();

    var participant = room.Participants.FirstOrDefault(p => p.Id == userId);
    if (participant != null)
    {
      room.Participants.Remove(participant);
      await _context.SaveChangesAsync();
    }

    return NoContent();
  }

  // POST: api/chat/search
  [HttpPost("search")]
  public async Task<ActionResult<ChatSearchResponse>> SearchMessages(ChatSearchRequest request)
  {
    var userId = GetUserId();
    if (userId == null) return Unauthorized();

    var query = _context.Messages
        .Include(m => m.Sender)
        .Where(m => m.DeletedAt == null);

    if (!string.IsNullOrEmpty(request.Query))
    {
      query = query.Where(m => m.Content.Contains(request.Query));
    }

    if (!string.IsNullOrEmpty(request.RoomId))
    {
      query = query.Where(m => m.RoomId == request.RoomId);
    }

    if (!string.IsNullOrEmpty(request.ProjectId))
    {
      query = query.Where(m => m.ProjectId == request.ProjectId);
    }

    if (request.FromDate.HasValue)
    {
      query = query.Where(m => m.CreatedAt >= request.FromDate.Value);
    }

    if (request.ToDate.HasValue)
    {
      query = query.Where(m => m.CreatedAt <= request.ToDate.Value);
    }

    var totalCount = await query.CountAsync();
    var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

    var messages = await query
        .OrderByDescending(m => m.CreatedAt)
        .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(m => new MessageResponse
        {
          Id = m.Id,
          Content = m.Content,
          SenderId = m.SenderId,
          SenderName = m.SenderName,
          Type = m.Type.ToString(),
          RoomId = m.RoomId,
          ProjectId = m.ProjectId,
          TaskId = m.TaskId,
          Mentions = m.Mentions,
          Attachments = m.Attachments,
          IsEdited = m.IsEdited,
          CreatedAt = m.CreatedAt,
          UpdatedAt = m.UpdatedAt
        })
        .ToListAsync();

    return new ChatSearchResponse
    {
      Messages = messages,
      TotalCount = totalCount,
      Page = request.Page,
      PageSize = request.PageSize,
      TotalPages = totalPages
    };
  }

  private string? GetUserId()
  {
    return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  }

  private string? GetUserOrganizationId()
  {
    return User.FindFirst("OrganizationId")?.Value;
  }
}
