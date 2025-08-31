using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class ThirdPartyIntegration
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required, StringLength(50)]
  public string Name { get; set; } = string.Empty;

  [Required, StringLength(50)]
  public string Provider { get; set; } = string.Empty; // GitHub, Slack, GoogleWorkspace, etc.

  [Required]
  public string ClientId { get; set; } = string.Empty;

  [Required]
  public string ClientSecret { get; set; } = string.Empty;

  public string? AccessToken { get; set; }

  public string? RefreshToken { get; set; }

  public DateTime? TokenExpiresAt { get; set; }

  public string? WebhookUrl { get; set; }

  public string? WebhookSecret { get; set; }

  public bool IsEnabled { get; set; } = true;

  public bool AutoSync { get; set; } = false;

  public string? SyncSettings { get; set; } // JSON configuration

  public string? OrganizationId { get; set; }

  public string? CreatedById { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  public DateTime? LastSyncAt { get; set; }

  public string? LastSyncStatus { get; set; }

  public string? LastSyncError { get; set; }

  // Navigation properties
  public virtual Organization? Organization { get; set; }
  public virtual User? CreatedBy { get; set; }
  public virtual ICollection<IntegrationEvent> Events { get; set; } = new List<IntegrationEvent>();
  public virtual ICollection<IntegrationSync> Syncs { get; set; } = new List<IntegrationSync>();
}

public class IntegrationEvent
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string IntegrationId { get; set; } = string.Empty;

  [Required, StringLength(50)]
  public string EventType { get; set; } = string.Empty; // webhook, sync, error, etc.

  [Required]
  public string Payload { get; set; } = string.Empty; // JSON data

  public string? ExternalId { get; set; }

  public string? Status { get; set; } = "pending";

  public string? ErrorMessage { get; set; }

  public int RetryCount { get; set; } = 0;

  public DateTime? ProcessedAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual ThirdPartyIntegration Integration { get; set; } = null!;
}

public class IntegrationSync
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string IntegrationId { get; set; } = string.Empty;

  [Required, StringLength(50)]
  public string SyncType { get; set; } = string.Empty; // full, incremental, etc.

  [Required]
  public string EntityType { get; set; } = string.Empty; // project, task, user, etc.

  public string? EntityId { get; set; }

  public int ItemsProcessed { get; set; } = 0;

  public int ItemsCreated { get; set; } = 0;

  public int ItemsUpdated { get; set; } = 0;

  public int ItemsFailed { get; set; } = 0;

  public string? Status { get; set; } = "running";

  public string? ErrorMessage { get; set; }

  public DateTime StartedAt { get; set; } = DateTime.UtcNow;

  public DateTime? CompletedAt { get; set; }

  public int DurationMs { get; set; } = 0;

  // Navigation properties
  public virtual ThirdPartyIntegration Integration { get; set; } = null!;
}

// GitHub-specific models
public class GitHubRepository
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string IntegrationId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Name { get; set; } = string.Empty;

  public string? FullName { get; set; }

  public string? Description { get; set; }

  public string? Url { get; set; }

  public string? DefaultBranch { get; set; }

  public bool IsPrivate { get; set; } = false;

  public bool IsFork { get; set; } = false;

  public string? Language { get; set; }

  public int Stars { get; set; } = 0;

  public int Forks { get; set; } = 0;

  public DateTime? LastSyncAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual ThirdPartyIntegration Integration { get; set; } = null!;
  public virtual ICollection<GitHubIssue> Issues { get; set; } = new List<GitHubIssue>();
  public virtual ICollection<GitHubPullRequest> PullRequests { get; set; } = new List<GitHubPullRequest>();
}

public class GitHubIssue
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string RepositoryId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Title { get; set; } = string.Empty;

  public string? Description { get; set; }

  public string? State { get; set; } = "open";

  public string? Assignee { get; set; }

  public string? Author { get; set; }

  public string? Labels { get; set; } // JSON array

  public int Number { get; set; }

  public string? Url { get; set; }

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }

  public DateTime? ClosedAt { get; set; }

  public DateTime LastSyncAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual GitHubRepository Repository { get; set; } = null!;
}

public class GitHubPullRequest
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string RepositoryId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Title { get; set; } = string.Empty;

  public string? Description { get; set; }

  public string? State { get; set; } = "open";

  public string? Author { get; set; }

  public string? Assignee { get; set; }

  public string? Reviewers { get; set; } // JSON array

  public string? Labels { get; set; } // JSON array

  public int Number { get; set; }

  public string? Url { get; set; }

  public string? BaseBranch { get; set; }

  public string? HeadBranch { get; set; }

  public DateTime? CreatedAt { get; set; }

  public DateTime? UpdatedAt { get; set; }

  public DateTime? MergedAt { get; set; }

  public DateTime? ClosedAt { get; set; }

  public DateTime LastSyncAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual GitHubRepository Repository { get; set; } = null!;
}

// Slack-specific models
public class SlackWorkspace
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string IntegrationId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Name { get; set; } = string.Empty;

  public string? Domain { get; set; }

  public string? Icon { get; set; }

  public bool IsEnterprise { get; set; } = false;

  public string? EnterpriseId { get; set; }

  public DateTime? LastSyncAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual ThirdPartyIntegration Integration { get; set; } = null!;
  public virtual ICollection<SlackChannel> Channels { get; set; } = new List<SlackChannel>();
  public virtual ICollection<SlackUser> Users { get; set; } = new List<SlackUser>();
}

public class SlackChannel
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string WorkspaceId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Name { get; set; } = string.Empty;

  public string? Topic { get; set; }

  public string? Purpose { get; set; }

  public bool IsPrivate { get; set; } = false;

  public bool IsArchived { get; set; } = false;

  public int MemberCount { get; set; } = 0;

  public DateTime? LastSyncAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual SlackWorkspace Workspace { get; set; } = null!;
  public virtual ICollection<SlackMessage> Messages { get; set; } = new List<SlackMessage>();
}

public class SlackUser
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string WorkspaceId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Name { get; set; } = string.Empty;

  public string? RealName { get; set; }

  public string? Email { get; set; }

  public string? Avatar { get; set; }

  public string? Status { get; set; }

  public string? StatusText { get; set; }

  public bool IsBot { get; set; } = false;

  public bool IsDeleted { get; set; } = false;

  public DateTime? LastSyncAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual SlackWorkspace Workspace { get; set; } = null!;
}

public class SlackMessage
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string ChannelId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Text { get; set; } = string.Empty;

  public string? UserId { get; set; }

  public string? Username { get; set; }

  public string? Type { get; set; } = "message";

  public string? Subtype { get; set; }

  public string? ThreadTs { get; set; }

  public string? ParentMessageId { get; set; }

  public string? Attachments { get; set; } // JSON array

  public string? Reactions { get; set; } // JSON array

  public DateTime? Timestamp { get; set; }

  public DateTime? LastSyncAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual SlackChannel Channel { get; set; } = null!;
  public virtual SlackMessage? ParentMessage { get; set; }
  public virtual ICollection<SlackMessage> Replies { get; set; } = new List<SlackMessage>();
}

// Google Workspace-specific models
public class GoogleWorkspaceDomain
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string IntegrationId { get; set; } = string.Empty;

  [Required]
  public string Domain { get; set; } = string.Empty;

  public string? Name { get; set; }

  public bool IsPrimary { get; set; } = false;

  public bool IsVerified { get; set; } = false;

  public DateTime? LastSyncAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual ThirdPartyIntegration Integration { get; set; } = null!;
  public virtual ICollection<GoogleWorkspaceUser> Users { get; set; } = new List<GoogleWorkspaceUser>();
  public virtual ICollection<GoogleWorkspaceGroup> Groups { get; set; } = new List<GoogleWorkspaceGroup>();
}

public class GoogleWorkspaceUser
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string DomainId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Email { get; set; } = string.Empty;

  public string? FirstName { get; set; }

  public string? LastName { get; set; }

  public string? DisplayName { get; set; }

  public string? Avatar { get; set; }

  public string? Department { get; set; }

  public string? JobTitle { get; set; }

  public string? Location { get; set; }

  public bool IsAdmin { get; set; } = false;

  public bool IsSuspended { get; set; } = false;

  public DateTime? LastSyncAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual GoogleWorkspaceDomain Domain { get; set; } = null!;
  public virtual ICollection<GoogleWorkspaceGroup> Groups { get; set; } = new List<GoogleWorkspaceGroup>();
}

public class GoogleWorkspaceGroup
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string DomainId { get; set; } = string.Empty;

  [Required]
  public string ExternalId { get; set; } = string.Empty;

  [Required]
  public string Name { get; set; } = string.Empty;

  public string? Email { get; set; }

  public string? Description { get; set; }

  public string? Type { get; set; } = "group";

  public int MemberCount { get; set; } = 0;

  public DateTime? LastSyncAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual GoogleWorkspaceDomain Domain { get; set; } = null!;
  public virtual ICollection<GoogleWorkspaceUser> Users { get; set; } = new List<GoogleWorkspaceUser>();
}
