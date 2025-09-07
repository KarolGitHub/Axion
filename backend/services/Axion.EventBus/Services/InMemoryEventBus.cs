using Axion.Shared.Events;
using Axion.Shared.Interfaces;
using System.Collections.Concurrent;

namespace Axion.EventBus.Services
{
  public class InMemoryEventBus : IEventBus
  {
    private readonly ConcurrentDictionary<string, List<Func<BaseEvent, Task>>> _handlers;
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
      _handlers = new ConcurrentDictionary<string, List<Func<BaseEvent, Task>>>();
      _logger = logger;
    }

    public async Task PublishAsync<T>(T @event) where T : BaseEvent
    {
      try
      {
        var eventType = @event.EventType;

        if (_handlers.TryGetValue(eventType, out var handlers))
        {
          var tasks = handlers.Select(handler => handler(@event));
          await Task.WhenAll(tasks);
        }

        _logger.LogInformation("Event {EventType} with ID {EventId} published to {HandlerCount} handlers",
            eventType, @event.EventId, handlers?.Count ?? 0);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error publishing event {EventType} with ID {EventId}",
            @event.EventType, @event.EventId);
        throw;
      }
    }

    public Task SubscribeAsync<T>(string eventType, Func<T, Task> handler) where T : BaseEvent
    {
      try
      {
        var handlers = _handlers.GetOrAdd(eventType, _ => new List<Func<BaseEvent, Task>>());

        Func<BaseEvent, Task> wrappedHandler = async (baseEvent) =>
        {
          if (baseEvent is T typedEvent)
          {
            await handler(typedEvent);
          }
        };

        handlers.Add(wrappedHandler);

        _logger.LogInformation("Handler subscribed to event type {EventType}", eventType);
        return Task.CompletedTask;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error subscribing to event type {EventType}", eventType);
        throw;
      }
    }

    public Task UnsubscribeAsync<T>(string eventType) where T : BaseEvent
    {
      try
      {
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
          handlers.Clear();
        }

        _logger.LogInformation("All handlers unsubscribed from event type {EventType}", eventType);
        return Task.CompletedTask;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error unsubscribing from event type {EventType}", eventType);
        throw;
      }
    }
  }
}

