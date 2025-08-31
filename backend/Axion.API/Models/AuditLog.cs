using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class AuditLog
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string UserId { get; set; } = string.Empty;

  public string? UserEmail { get; set; }

  public string? UserName { get; set; }

  [Required]
  public string Action { get; set; } = string.Empty; // Login, Logout, Create, Update, Delete, etc.

  [Required]
  public string EntityType { get; set; } = string.Empty; // User, Project, Task, etc.

  public string? EntityId { get; set; }

  public string? EntityName { get; set; }

  public string? OldValues { get; set; } // JSON serialized old values

  public string? NewValues { get; set; } // JSON serialized new values

  public string? IpAddress { get; set; }

  public string? UserAgent { get; set; }

  public string? SessionId { get; set; }

  public string? OrganizationId { get; set; }

  public string? AdditionalData { get; set; } // JSON serialized additional data

  public DateTime Timestamp { get; set; } = DateTime.UtcNow;

  public bool Success { get; set; } = true;

  public string? ErrorMessage { get; set; }

  public int? ResponseTimeMs { get; set; }

  // Navigation properties
  public virtual User User { get; set; } = null!;
  public virtual Organization? Organization { get; set; }
}

public enum AuditAction
{
  Login,
  Logout,
  LoginFailed,
  PasswordChanged,
  PasswordReset,
  TwoFactorEnabled,
  TwoFactorDisabled,
  TwoFactorVerified,
  TwoFactorFailed,
  SSOLogin,
  SSOLoginFailed,
  UserCreated,
  UserUpdated,
  UserDeleted,
  UserActivated,
  UserDeactivated,
  ProjectCreated,
  ProjectUpdated,
  ProjectDeleted,
  TaskCreated,
  TaskUpdated,
  TaskDeleted,
  TaskStatusChanged,
  ResourceCreated,
  ResourceUpdated,
  ResourceDeleted,
  BookingCreated,
  BookingUpdated,
  BookingDeleted,
  ChatMessageSent,
  ChatMessageDeleted,
  FileUploaded,
  FileDeleted,
  ExportRequested,
  ImportCompleted,
  SettingsChanged,
  RoleChanged,
  PermissionGranted,
  PermissionRevoked,
  SystemMaintenance,
  DataBackup,
  DataRestore,
  SecurityAlert,
  ComplianceCheck,
  IntegrationCreated,
  IntegrationUpdated,
  IntegrationDeleted,
  IntegrationSync,
  IntegrationTest,
  WebhookReceived,
  WebhookProcessed,
  WebhookFailed
}

public enum AuditEntityType
{
  User,
  Organization,
  Project,
  Task,
  Comment,
  Resource,
  Booking,
  ChatRoom,
  Message,
  File,
  Settings,
  Role,
  Permission,
  System,
  Security,
  Compliance,
  Integration
}
