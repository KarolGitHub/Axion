using Axion.Shared.Events;

namespace Axion.Shared.Interfaces
{
  public interface IEventBus
  {
    Task PublishAsync<T>(T @event) where T : BaseEvent;
    Task SubscribeAsync<T>(string eventType, Func<T, Task> handler) where T : BaseEvent;
    Task UnsubscribeAsync<T>(string eventType) where T : BaseEvent;
  }

  public interface IEventHandler<T> where T : BaseEvent
  {
    Task HandleAsync(T @event);
  }

  public interface IEventStore
  {
    Task SaveEventAsync(BaseEvent @event);
    Task<List<BaseEvent>> GetEventsAsync(string eventType, DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<BaseEvent>> GetEventsByOrganizationAsync(int organizationId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<BaseEvent>> GetEventsByUserAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
  }

  public interface IEventPublisher
  {
    Task PublishAsync<T>(T @event) where T : BaseEvent;
  }

  public interface IEventSubscriber
  {
    Task SubscribeAsync<T>(string eventType, Func<T, Task> handler) where T : BaseEvent;
    Task UnsubscribeAsync<T>(string eventType) where T : BaseEvent;
  }
}

