using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Axion.API.Data;
using Axion.API.Models;
using Axion.API.DTOs;
using System.Security.Claims;

namespace Axion.API.Hubs;

public class ChatHub : Hub
{
  private readonly AxionDbContext _context;
  private readonly ILogger<ChatHub> _logger;

  public ChatHub(AxionDbContext context, ILogger<ChatHub> logger)
  {
    _context = context;
    _logger = logger;
  }

  public override async Task OnConnectedAsync()
  {
    var userId = GetUserId();
    if (userId != null)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
      await Clients.All.SendAsync("UserConnected", userId);
    }
    await base.OnConnectedAsync();
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    var userId = GetUserId();
    if (userId != null)
    {
      await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
      await Clients.All.SendAsync("UserDisconnected", userId);
    }
    await base.OnDisconnectedAsync(exception);
  }

  public async Task JoinRoom(string roomId)
  {
    var userId = GetUserId();
    if (userId == null) return;

    // Check if user has access to the room
    var hasAccess = await _context.ChatRooms
        .AnyAsync(r => r.Id == roomId &&
                      (r.Participants.Any(p => p.Id == userId) || !r.IsPrivate));

    if (hasAccess)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
      await Clients.Group($"room_{roomId}").SendAsync("UserJoinedRoom", userId, roomId);
    }
  }

  public async Task LeaveRoom(string roomId)
  {
    var userId = GetUserId();
    if (userId == null) return;

    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");
    await Clients.Group($"room_{roomId}").SendAsync("UserLeftRoom", userId, roomId);
  }

  public async Task SendMessage(CreateMessageRequest request)
  {
    var userId = GetUserId();
    if (userId == null) return;

    var user = await _context.Users.FindAsync(userId);
    if (user == null) return;

    var message = new Message
    {
      Content = request.Content,
      SenderId = userId,
      SenderName = $"{user.FirstName} {user.LastName}",
      Type = MessageType.Text,
      RoomId = request.RoomId,
      ProjectId = request.ProjectId,
      TaskId = request.TaskId,
      ReplyToMessageId = request.ReplyToMessageId,
      Mentions = request.Mentions,
      Attachments = request.Attachments
    };

    _context.Messages.Add(message);
    await _context.SaveChangesAsync();

    var messageResponse = new MessageResponse
    {
      Id = message.Id,
      Content = message.Content,
      SenderId = message.SenderId,
      SenderName = message.SenderName,
      Type = message.Type.ToString(),
      RoomId = message.RoomId,
      ProjectId = message.ProjectId,
      TaskId = message.TaskId,
      ReplyToMessageId = message.ReplyToMessageId,
      Mentions = message.Mentions,
      Attachments = message.Attachments,
      IsEdited = message.IsEdited,
      CreatedAt = message.CreatedAt,
      UpdatedAt = message.UpdatedAt
    };

    if (message.RoomId != null)
    {
      await Clients.Group($"room_{message.RoomId}").SendAsync("MessageReceived", messageResponse);
    }

    // Send notifications to mentioned users
    foreach (var mention in message.Mentions)
    {
      await Clients.Group($"user_{mention}").SendAsync("MentionReceived", messageResponse);
    }
  }

  public async Task UpdateMessage(string messageId, UpdateMessageRequest request)
  {
    var userId = GetUserId();
    if (userId == null) return;

    var message = await _context.Messages.FindAsync(messageId);
    if (message == null || message.SenderId != userId) return;

    message.Content = request.Content;
    message.IsEdited = true;
    message.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    var messageResponse = new MessageResponse
    {
      Id = message.Id,
      Content = message.Content,
      SenderId = message.SenderId,
      SenderName = message.SenderName,
      Type = message.Type.ToString(),
      RoomId = message.RoomId,
      ProjectId = message.ProjectId,
      TaskId = message.TaskId,
      ReplyToMessageId = message.ReplyToMessageId,
      Mentions = message.Mentions,
      Attachments = message.Attachments,
      IsEdited = message.IsEdited,
      CreatedAt = message.CreatedAt,
      UpdatedAt = message.UpdatedAt
    };

    if (message.RoomId != null)
    {
      await Clients.Group($"room_{message.RoomId}").SendAsync("MessageUpdated", messageResponse);
    }
  }

  public async Task DeleteMessage(string messageId)
  {
    var userId = GetUserId();
    if (userId == null) return;

    var message = await _context.Messages.FindAsync(messageId);
    if (message == null || message.SenderId != userId) return;

    message.DeletedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();

    if (message.RoomId != null)
    {
      await Clients.Group($"room_{message.RoomId}").SendAsync("MessageDeleted", messageId);
    }
  }

  public async Task StartTyping(string roomId)
  {
    var userId = GetUserId();
    if (userId == null) return;

    var user = await _context.Users.FindAsync(userId);
    if (user == null) return;

    var notification = new TypingNotification
    {
      UserId = userId,
      UserName = $"{user.FirstName} {user.LastName}",
      RoomId = roomId,
      IsTyping = true
    };

    await Clients.Group($"room_{roomId}").SendAsync("TypingStarted", notification);
  }

  public async Task StopTyping(string roomId)
  {
    var userId = GetUserId();
    if (userId == null) return;

    var user = await _context.Users.FindAsync(userId);
    if (user == null) return;

    var notification = new TypingNotification
    {
      UserId = userId,
      UserName = $"{user.FirstName} {user.LastName}",
      RoomId = roomId,
      IsTyping = false
    };

    await Clients.Group($"room_{roomId}").SendAsync("TypingStopped", notification);
  }

  public async Task MarkAsRead(string roomId)
  {
    var userId = GetUserId();
    if (userId == null) return;

    // In a real implementation, you would update a read status table
    await Clients.Group($"room_{roomId}").SendAsync("MessageRead", userId, roomId);
  }

  private string? GetUserId()
  {
    return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  }
}
