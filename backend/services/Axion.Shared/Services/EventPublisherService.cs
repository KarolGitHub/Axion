using Axion.Shared.Events;
using Axion.Shared.Interfaces;
using System.Text.Json;
using System.Text;

namespace Axion.Shared.Services
{
  public class EventPublisherService : IEventPublisher
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EventPublisherService> _logger;

    public EventPublisherService(HttpClient httpClient, IConfiguration configuration, ILogger<EventPublisherService> logger)
    {
      _httpClient = httpClient;
      _configuration = configuration;
      _logger = logger;
    }

    public async Task PublishAsync<T>(T @event) where T : BaseEvent
    {
      try
      {
        var eventBusUrl = _configuration["EventBus:Url"] ?? "http://localhost:5005";
        var json = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{eventBusUrl}/api/eventbus/publish", content);

        if (response.IsSuccessStatusCode)
        {
          _logger.LogInformation("Event {EventType} with ID {EventId} published successfully",
              @event.EventType, @event.EventId);
        }
        else
        {
          _logger.LogWarning("Failed to publish event {EventType} with ID {EventId}. Status: {StatusCode}",
              @event.EventType, @event.EventId, response.StatusCode);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error publishing event {EventType} with ID {EventId}",
            @event.EventType, @event.EventId);
        throw;
      }
    }
  }
}

