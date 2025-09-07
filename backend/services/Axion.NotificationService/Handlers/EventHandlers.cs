using Axion.Shared.Events;
using Axion.Shared.Interfaces;
using Axion.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Axion.NotificationService.Handlers
{
  public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
  {
    private readonly NotificationDbContext _context;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(NotificationDbContext context, ILogger<UserCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task HandleAsync(UserCreatedEvent @event)
    {
      try
      {
        var notification = new Notification
        {
          Title = "Welcome to Axion!",
          Message = $"Welcome {@event.FirstName} {@event.LastName}! Your account has been created successfully.",
          Type = "Welcome",
          UserId = @event.UserId,
          OrganizationId = @event.OrganizationId,
          CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Welcome notification created for user {UserId}", @event.UserId);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error handling UserCreatedEvent for user {UserId}", @event.UserId);
      }
    }
  }

  public class TaskCreatedEventHandler : IEventHandler<TaskCreatedEvent>
  {
    private readonly NotificationDbContext _context;
    private readonly ILogger<TaskCreatedEventHandler> _logger;

    public TaskCreatedEventHandler(NotificationDbContext context, ILogger<TaskCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task HandleAsync(TaskCreatedEvent @event)
    {
      try
      {
        var notification = new Notification
        {
          Title = "New Task Assigned",
          Message = $"A new task '{@event.Title}' has been assigned to you.",
          Type = "TaskAssignment",
          UserId = @event.AssignedTo,
          OrganizationId = @event.OrganizationId,
          CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task assignment notification created for user {UserId}", @event.AssignedTo);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error handling TaskCreatedEvent for user {UserId}", @event.AssignedTo);
      }
    }
  }

  public class TaskCompletedEventHandler : IEventHandler<TaskCompletedEvent>
  {
    private readonly NotificationDbContext _context;
    private readonly ILogger<TaskCompletedEventHandler> _logger;

    public TaskCompletedEventHandler(NotificationDbContext context, ILogger<TaskCompletedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task HandleAsync(TaskCompletedEvent @event)
    {
      try
      {
        // Notify project managers about task completion
        var projectManagers = await _context.Users
            .Where(u => u.OrganizationId == @event.OrganizationId && u.Role == "ProjectManager")
            .ToListAsync();

        foreach (var manager in projectManagers)
        {
          var notification = new Notification
          {
            Title = "Task Completed",
            Message = $"Task '{@event.Title}' has been completed by the assigned user.",
            Type = "TaskCompletion",
            UserId = manager.Id,
            OrganizationId = @event.OrganizationId,
            CreatedAt = DateTime.UtcNow
          };

          _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Task completion notifications created for {ManagerCount} managers", projectManagers.Count);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error handling TaskCompletedEvent for task {TaskId}", @event.TaskId);
      }
    }
  }

  public class ProjectCreatedEventHandler : IEventHandler<ProjectCreatedEvent>
  {
    private readonly NotificationDbContext _context;
    private readonly ILogger<ProjectCreatedEventHandler> _logger;

    public ProjectCreatedEventHandler(NotificationDbContext context, ILogger<ProjectCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task HandleAsync(ProjectCreatedEvent @event)
    {
      try
      {
        // Notify all users in the organization about the new project
        var users = await _context.Users
            .Where(u => u.OrganizationId == @event.OrganizationId && u.IsActive)
            .ToListAsync();

        foreach (var user in users)
        {
          var notification = new Notification
          {
            Title = "New Project Created",
            Message = $"A new project '{@event.Name}' has been created in your organization.",
            Type = "ProjectUpdate",
            UserId = user.Id,
            OrganizationId = @event.OrganizationId,
            CreatedAt = DateTime.UtcNow
          };

          _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Project creation notifications created for {UserCount} users", users.Count);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error handling ProjectCreatedEvent for project {ProjectId}", @event.ProjectId);
      }
    }
  }
}

