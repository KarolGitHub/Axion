using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Axion.Shared.Events;
using Axion.Shared.Interfaces;
using Axion.Shared.DTOs;
using System.Text.Json;

namespace Axion.EventBus.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class EventBusController : ControllerBase
  {
    private readonly IEventBus _eventBus;
    private readonly IEventStore _eventStore;
    private readonly ILogger<EventBusController> _logger;

    public EventBusController(IEventBus eventBus, IEventStore eventStore, ILogger<EventBusController> logger)
    {
      _eventBus = eventBus;
      _eventStore = eventStore;
      _logger = logger;
    }

    [HttpPost("publish")]
    public async Task<ActionResult<ApiResponse<object>>> PublishEvent([FromBody] BaseEvent @event)
    {
      try
      {
        await _eventBus.PublishAsync(@event);
        await _eventStore.SaveEventAsync(@event);

        _logger.LogInformation("Event {EventType} with ID {EventId} published successfully",
            @event.EventType, @event.EventId);

        return Ok(new ApiResponse<object>
        {
          Success = true,
          Message = "Event published successfully",
          Data = new { EventId = @event.EventId, EventType = @event.EventType }
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error publishing event {EventType}", @event.EventType);
        return StatusCode(500, new ApiResponse<object>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<ApiResponse<object>>> SubscribeToEvent([FromBody] SubscribeRequest request)
    {
      try
      {
        // In a real implementation, this would register the subscription
        // For now, we'll just log the subscription
        _logger.LogInformation("Subscription request for event type {EventType} from service {ServiceName}",
            request.EventType, request.ServiceName);

        return Ok(new ApiResponse<object>
        {
          Success = true,
          Message = "Subscription registered successfully"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error registering subscription for event type {EventType}", request.EventType);
        return StatusCode(500, new ApiResponse<object>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("events")]
    public async Task<ActionResult<ApiResponse<List<EventResponse>>>> GetEvents(
        [FromQuery] string? eventType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int? organizationId = null)
    {
      try
      {
        List<BaseEvent> events;

        if (!string.IsNullOrEmpty(eventType))
        {
          events = await _eventStore.GetEventsAsync(eventType, fromDate, toDate);
        }
        else if (organizationId.HasValue)
        {
          events = await _eventStore.GetEventsByOrganizationAsync(organizationId.Value, fromDate, toDate);
        }
        else
        {
          events = await _eventStore.GetEventsAsync("", fromDate, toDate);
        }

        var eventResponses = events.Select(e => new EventResponse
        {
          EventId = e.EventId,
          EventType = e.EventType,
          Timestamp = e.Timestamp,
          OrganizationId = e.OrganizationId,
          UserId = e.UserId,
          Source = e.Source
        }).ToList();

        return Ok(new ApiResponse<List<EventResponse>>
        {
          Success = true,
          Data = eventResponses
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving events");
        return StatusCode(500, new ApiResponse<List<EventResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("events/{eventId}")]
    public async Task<ActionResult<ApiResponse<EventDetailResponse>>> GetEvent(Guid eventId)
    {
      try
      {
        var events = await _eventStore.GetEventsAsync("");
        var @event = events.FirstOrDefault(e => e.EventId == eventId);

        if (@event == null)
        {
          return NotFound(new ApiResponse<EventDetailResponse>
          {
            Success = false,
            Message = "Event not found"
          });
        }

        var eventDetail = new EventDetailResponse
        {
          EventId = @event.EventId,
          EventType = @event.EventType,
          Timestamp = @event.Timestamp,
          OrganizationId = @event.OrganizationId,
          UserId = @event.UserId,
          Source = @event.Source,
          EventData = JsonSerializer.Serialize(@event)
        };

        return Ok(new ApiResponse<EventDetailResponse>
        {
          Success = true,
          Data = eventDetail
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving event {EventId}", eventId);
        return StatusCode(500, new ApiResponse<EventDetailResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("health")]
    public ActionResult<ApiResponse<object>> GetHealth()
    {
      return Ok(new ApiResponse<object>
      {
        Success = true,
        Message = "Event Bus is healthy",
        Data = new { Status = "Healthy", Timestamp = DateTime.UtcNow }
      });
    }
  }

  public class SubscribeRequest
  {
    public string EventType { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
  }

  public class EventResponse
  {
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public string Source { get; set; } = string.Empty;
  }

  public class EventDetailResponse : EventResponse
  {
    public string EventData { get; set; } = string.Empty;
  }
}

