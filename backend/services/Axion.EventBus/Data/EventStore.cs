using Axion.Shared.Events;
using Axion.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Axion.EventBus.Data
{
  public class EventStore : IEventStore
  {
    private readonly EventDbContext _context;
    private readonly ILogger<EventStore> _logger;

    public EventStore(EventDbContext context, ILogger<EventStore> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task SaveEventAsync(BaseEvent @event)
    {
      try
      {
        var eventEntity = new EventEntity
        {
          EventId = @event.EventId,
          EventType = @event.EventType,
          Timestamp = @event.Timestamp,
          OrganizationId = @event.OrganizationId,
          UserId = @event.UserId,
          Source = @event.Source,
          EventData = JsonSerializer.Serialize(@event),
          CreatedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Event {EventType} with ID {EventId} saved to event store",
            @event.EventType, @event.EventId);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error saving event {EventType} with ID {EventId}",
            @event.EventType, @event.EventId);
        throw;
      }
    }

    public async Task<List<BaseEvent>> GetEventsAsync(string eventType, DateTime? fromDate = null, DateTime? toDate = null)
    {
      try
      {
        var query = _context.Events.AsQueryable();

        if (!string.IsNullOrEmpty(eventType))
        {
          query = query.Where(e => e.EventType == eventType);
        }

        if (fromDate.HasValue)
        {
          query = query.Where(e => e.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
          query = query.Where(e => e.Timestamp <= toDate.Value);
        }

        var events = await query
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();

        var baseEvents = new List<BaseEvent>();
        foreach (var eventEntity in events)
        {
          try
          {
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(eventEntity.EventData);
            if (baseEvent != null)
            {
              baseEvents.Add(baseEvent);
            }
          }
          catch (JsonException ex)
          {
            _logger.LogWarning(ex, "Failed to deserialize event {EventId}", eventEntity.EventId);
          }
        }

        return baseEvents;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving events");
        throw;
      }
    }

    public async Task<List<BaseEvent>> GetEventsByOrganizationAsync(int organizationId, DateTime? fromDate = null, DateTime? toDate = null)
    {
      try
      {
        var query = _context.Events.Where(e => e.OrganizationId == organizationId);

        if (fromDate.HasValue)
        {
          query = query.Where(e => e.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
          query = query.Where(e => e.Timestamp <= toDate.Value);
        }

        var events = await query
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();

        var baseEvents = new List<BaseEvent>();
        foreach (var eventEntity in events)
        {
          try
          {
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(eventEntity.EventData);
            if (baseEvent != null)
            {
              baseEvents.Add(baseEvent);
            }
          }
          catch (JsonException ex)
          {
            _logger.LogWarning(ex, "Failed to deserialize event {EventId}", eventEntity.EventId);
          }
        }

        return baseEvents;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving events for organization {OrganizationId}", organizationId);
        throw;
      }
    }

    public async Task<List<BaseEvent>> GetEventsByUserAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
      try
      {
        var query = _context.Events.Where(e => e.UserId == userId);

        if (fromDate.HasValue)
        {
          query = query.Where(e => e.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
          query = query.Where(e => e.Timestamp <= toDate.Value);
        }

        var events = await query
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();

        var baseEvents = new List<BaseEvent>();
        foreach (var eventEntity in events)
        {
          try
          {
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(eventEntity.EventData);
            if (baseEvent != null)
            {
              baseEvents.Add(baseEvent);
            }
          }
          catch (JsonException ex)
          {
            _logger.LogWarning(ex, "Failed to deserialize event {EventId}", eventEntity.EventId);
          }
        }

        return baseEvents;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving events for user {UserId}", userId);
        throw;
      }
    }
  }

  public class EventEntity
  {
    public int Id { get; set; }
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public string Source { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
  }

  public class EventDbContext : DbContext
  {
    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
    {
    }

    public DbSet<EventEntity> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EventEntity>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.EventId).IsRequired();
        entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Source).IsRequired().HasMaxLength(100);
        entity.Property(e => e.EventData).IsRequired().HasColumnType("nvarchar(max)");
        entity.HasIndex(e => e.EventId).IsUnique();
        entity.HasIndex(e => new { e.EventType, e.Timestamp });
        entity.HasIndex(e => new { e.OrganizationId, e.Timestamp });
        entity.HasIndex(e => new { e.UserId, e.Timestamp });
      });
    }
  }
}

