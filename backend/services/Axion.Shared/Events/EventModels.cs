using System.ComponentModel.DataAnnotations;

namespace Axion.Shared.Events
{
  public abstract class BaseEvent
  {
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public int UserId { get; set; }
    public string Source { get; set; } = string.Empty;
  }

  // User Events
  public class UserCreatedEvent : BaseEvent
  {
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int OrganizationId { get; set; }

    public UserCreatedEvent()
    {
      EventType = "UserCreated";
    }
  }

  public class UserUpdatedEvent : BaseEvent
  {
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int OrganizationId { get; set; }

    public UserUpdatedEvent()
    {
      EventType = "UserUpdated";
    }
  }

  public class UserDeletedEvent : BaseEvent
  {
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int OrganizationId { get; set; }

    public UserDeletedEvent()
    {
      EventType = "UserDeleted";
    }
  }

  // Project Events
  public class ProjectCreatedEvent : BaseEvent
  {
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Budget { get; set; }
    public int CreatedBy { get; set; }
    public int OrganizationId { get; set; }

    public ProjectCreatedEvent()
    {
      EventType = "ProjectCreated";
    }
  }

  public class ProjectUpdatedEvent : BaseEvent
  {
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Budget { get; set; }
    public int OrganizationId { get; set; }

    public ProjectUpdatedEvent()
    {
      EventType = "ProjectUpdated";
    }
  }

  public class ProjectDeletedEvent : BaseEvent
  {
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OrganizationId { get; set; }

    public ProjectDeletedEvent()
    {
      EventType = "ProjectDeleted";
    }
  }

  // Task Events
  public class TaskCreatedEvent : BaseEvent
  {
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int ProjectId { get; set; }
    public int AssignedTo { get; set; }
    public int CreatedBy { get; set; }
    public int OrganizationId { get; set; }

    public TaskCreatedEvent()
    {
      EventType = "TaskCreated";
    }
  }

  public class TaskUpdatedEvent : BaseEvent
  {
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int ProjectId { get; set; }
    public int AssignedTo { get; set; }
    public int OrganizationId { get; set; }

    public TaskUpdatedEvent()
    {
      EventType = "TaskUpdated";
    }
  }

  public class TaskCompletedEvent : BaseEvent
  {
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int AssignedTo { get; set; }
    public decimal ActualHours { get; set; }
    public int OrganizationId { get; set; }

    public TaskCompletedEvent()
    {
      EventType = "TaskCompleted";
    }
  }

  public class TaskDeletedEvent : BaseEvent
  {
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int OrganizationId { get; set; }

    public TaskDeletedEvent()
    {
      EventType = "TaskDeleted";
    }
  }

  // Notification Events
  public class NotificationCreatedEvent : BaseEvent
  {
    public int NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int OrganizationId { get; set; }

    public NotificationCreatedEvent()
    {
      EventType = "NotificationCreated";
    }
  }

  public class NotificationReadEvent : BaseEvent
  {
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public int OrganizationId { get; set; }

    public NotificationReadEvent()
    {
      EventType = "NotificationRead";
    }
  }

  // System Events
  public class SystemHealthEvent : BaseEvent
  {
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();

    public SystemHealthEvent()
    {
      EventType = "SystemHealth";
    }
  }

  public class AuditEvent : BaseEvent
  {
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string OldValues { get; set; } = string.Empty;
    public string NewValues { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;

    public AuditEvent()
    {
      EventType = "Audit";
    }
  }
}

