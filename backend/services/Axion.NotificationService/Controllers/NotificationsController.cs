using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Axion.Shared.Models;
using Axion.Shared.DTOs;

namespace Axion.NotificationService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class NotificationsController : ControllerBase
  {
    private readonly NotificationDbContext _context;

    public NotificationsController(NotificationDbContext context)
    {
      _context = context;
    }

    private int GetOrganizationId()
    {
      var organizationIdClaim = User.FindFirst("OrganizationId")?.Value;
      if (int.TryParse(organizationIdClaim, out int organizationId))
      {
        return organizationId;
      }
      throw new UnauthorizedAccessException("Organization ID not found in token");
    }

    private int GetUserId()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdClaim, out int userId))
      {
        return userId;
      }
      throw new UnauthorizedAccessException("User ID not found in token");
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NotificationResponse>>>> GetNotifications()
    {
      try
      {
        var userId = GetUserId();
        var organizationId = GetOrganizationId();

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.OrganizationId == organizationId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationResponse
            {
              Id = n.Id,
              Title = n.Title,
              Message = n.Message,
              Type = n.Type,
              IsRead = n.IsRead,
              CreatedAt = n.CreatedAt,
              UserId = n.UserId,
              OrganizationId = n.OrganizationId
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<NotificationResponse>>
        {
          Success = true,
          Data = notifications
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<NotificationResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NotificationResponse>>> GetNotification(int id)
    {
      try
      {
        var userId = GetUserId();
        var organizationId = GetOrganizationId();

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && n.OrganizationId == organizationId);

        if (notification == null)
        {
          return NotFound(new ApiResponse<NotificationResponse>
          {
            Success = false,
            Message = "Notification not found"
          });
        }

        var notificationResponse = new NotificationResponse
        {
          Id = notification.Id,
          Title = notification.Title,
          Message = notification.Message,
          Type = notification.Type,
          IsRead = notification.IsRead,
          CreatedAt = notification.CreatedAt,
          UserId = notification.UserId,
          OrganizationId = notification.OrganizationId
        };

        return Ok(new ApiResponse<NotificationResponse>
        {
          Success = true,
          Data = notificationResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<NotificationResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<NotificationResponse>>> CreateNotification(CreateNotificationRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();

        var notification = new Notification
        {
          Title = request.Title,
          Message = request.Message,
          Type = request.Type,
          UserId = request.UserId,
          OrganizationId = organizationId,
          CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var notificationResponse = new NotificationResponse
        {
          Id = notification.Id,
          Title = notification.Title,
          Message = notification.Message,
          Type = notification.Type,
          IsRead = notification.IsRead,
          CreatedAt = notification.CreatedAt,
          UserId = notification.UserId,
          OrganizationId = notification.OrganizationId
        };

        return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, new ApiResponse<NotificationResponse>
        {
          Success = true,
          Data = notificationResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<NotificationResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPut("{id}/mark-read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(int id)
    {
      try
      {
        var userId = GetUserId();
        var organizationId = GetOrganizationId();

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && n.OrganizationId == organizationId);

        if (notification == null)
        {
          return NotFound(new ApiResponse<object>
          {
            Success = false,
            Message = "Notification not found"
          });
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
          Success = true,
          Message = "Notification marked as read"
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<object>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPut("mark-all-read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
    {
      try
      {
        var userId = GetUserId();
        var organizationId = GetOrganizationId();

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.OrganizationId == organizationId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
          notification.IsRead = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
          Success = true,
          Message = $"{notifications.Count} notifications marked as read"
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<object>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNotification(int id)
    {
      try
      {
        var userId = GetUserId();
        var organizationId = GetOrganizationId();

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && n.OrganizationId == organizationId);

        if (notification == null)
        {
          return NotFound(new ApiResponse<object>
          {
            Success = false,
            Message = "Notification not found"
          });
        }

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
          Success = true,
          Message = "Notification deleted successfully"
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<object>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
      try
      {
        var userId = GetUserId();
        var organizationId = GetOrganizationId();

        var unreadCount = await _context.Notifications
            .CountAsync(n => n.UserId == userId && n.OrganizationId == organizationId && !n.IsRead);

        return Ok(new ApiResponse<int>
        {
          Success = true,
          Data = unreadCount
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<int>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("by-type/{type}")]
    public async Task<ActionResult<ApiResponse<List<NotificationResponse>>>> GetNotificationsByType(string type)
    {
      try
      {
        var userId = GetUserId();
        var organizationId = GetOrganizationId();

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.OrganizationId == organizationId && n.Type == type)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationResponse
            {
              Id = n.Id,
              Title = n.Title,
              Message = n.Message,
              Type = n.Type,
              IsRead = n.IsRead,
              CreatedAt = n.CreatedAt,
              UserId = n.UserId,
              OrganizationId = n.OrganizationId
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<NotificationResponse>>
        {
          Success = true,
          Data = notifications
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<NotificationResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }
  }
}
