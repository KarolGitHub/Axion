namespace Axion.API.DTOs;

// Third-party integration DTOs
public class ThirdPartyIntegrationResponse
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Provider { get; set; } = string.Empty;
  public bool IsEnabled { get; set; }
  public bool AutoSync { get; set; }
  public string? SyncSettings { get; set; }
  public string? OrganizationId { get; set; }
  public string? CreatedById { get; set; }
  public string? CreatedByName { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public string? LastSyncStatus { get; set; }
  public string? LastSyncError { get; set; }
}

public class CreateThirdPartyIntegrationRequest
{
  public string Name { get; set; } = string.Empty;
  public string Provider { get; set; } = string.Empty;
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
  public string? WebhookUrl { get; set; }
  public string? WebhookSecret { get; set; }
  public bool AutoSync { get; set; } = false;
  public string? SyncSettings { get; set; }
  public string? OrganizationId { get; set; }
}

public class UpdateThirdPartyIntegrationRequest
{
  public string Name { get; set; } = string.Empty;
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
  public string? WebhookUrl { get; set; }
  public string? WebhookSecret { get; set; }
  public bool IsEnabled { get; set; }
  public bool AutoSync { get; set; }
  public string? SyncSettings { get; set; }
}

public class IntegrationEventResponse
{
  public string Id { get; set; } = string.Empty;
  public string IntegrationId { get; set; } = string.Empty;
  public string EventType { get; set; } = string.Empty;
  public string Payload { get; set; } = string.Empty;
  public string? ExternalId { get; set; }
  public string? Status { get; set; }
  public string? ErrorMessage { get; set; }
  public int RetryCount { get; set; }
  public DateTime? ProcessedAt { get; set; }
  public DateTime CreatedAt { get; set; }
}

public class IntegrationSyncResponse
{
  public string Id { get; set; } = string.Empty;
  public string IntegrationId { get; set; } = string.Empty;
  public string SyncType { get; set; } = string.Empty;
  public string EntityType { get; set; } = string.Empty;
  public string? EntityId { get; set; }
  public int ItemsProcessed { get; set; }
  public int ItemsCreated { get; set; }
  public int ItemsUpdated { get; set; }
  public int ItemsFailed { get; set; }
  public string? Status { get; set; }
  public string? ErrorMessage { get; set; }
  public DateTime StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }
  public int DurationMs { get; set; }
}

// GitHub DTOs
public class GitHubRepositoryResponse
{
  public string Id { get; set; } = string.Empty;
  public string IntegrationId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? FullName { get; set; }
  public string? Description { get; set; }
  public string? Url { get; set; }
  public string? DefaultBranch { get; set; }
  public bool IsPrivate { get; set; }
  public bool IsFork { get; set; }
  public string? Language { get; set; }
  public int Stars { get; set; }
  public int Forks { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public int IssueCount { get; set; }
  public int PullRequestCount { get; set; }
}

public class GitHubIssueResponse
{
  public string Id { get; set; } = string.Empty;
  public string RepositoryId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? State { get; set; }
  public string? Assignee { get; set; }
  public string? Author { get; set; }
  public string? Labels { get; set; }
  public int Number { get; set; }
  public string? Url { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public DateTime? ClosedAt { get; set; }
  public DateTime LastSyncAt { get; set; }
}

public class GitHubPullRequestResponse
{
  public string Id { get; set; } = string.Empty;
  public string RepositoryId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public string? State { get; set; }
  public string? Author { get; set; }
  public string? Assignee { get; set; }
  public string? Reviewers { get; set; }
  public string? Labels { get; set; }
  public int Number { get; set; }
  public string? Url { get; set; }
  public string? BaseBranch { get; set; }
  public string? HeadBranch { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public DateTime? MergedAt { get; set; }
  public DateTime? ClosedAt { get; set; }
  public DateTime LastSyncAt { get; set; }
}

// Slack DTOs
public class SlackWorkspaceResponse
{
  public string Id { get; set; } = string.Empty;
  public string IntegrationId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? Domain { get; set; }
  public string? Icon { get; set; }
  public bool IsEnterprise { get; set; }
  public string? EnterpriseId { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public int ChannelCount { get; set; }
  public int UserCount { get; set; }
}

public class SlackChannelResponse
{
  public string Id { get; set; } = string.Empty;
  public string WorkspaceId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? Topic { get; set; }
  public string? Purpose { get; set; }
  public bool IsPrivate { get; set; }
  public bool IsArchived { get; set; }
  public int MemberCount { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public int MessageCount { get; set; }
}

public class SlackUserResponse
{
  public string Id { get; set; } = string.Empty;
  public string WorkspaceId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? RealName { get; set; }
  public string? Email { get; set; }
  public string? Avatar { get; set; }
  public string? Status { get; set; }
  public string? StatusText { get; set; }
  public bool IsBot { get; set; }
  public bool IsDeleted { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}

public class SlackMessageResponse
{
  public string Id { get; set; } = string.Empty;
  public string ChannelId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Text { get; set; } = string.Empty;
  public string? UserId { get; set; }
  public string? Username { get; set; }
  public string? Type { get; set; }
  public string? Subtype { get; set; }
  public string? ThreadTs { get; set; }
  public string? ParentMessageId { get; set; }
  public string? Attachments { get; set; }
  public string? Reactions { get; set; }
  public DateTime? Timestamp { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public int ReplyCount { get; set; }
}

// Google Workspace DTOs
public class GoogleWorkspaceDomainResponse
{
  public string Id { get; set; } = string.Empty;
  public string IntegrationId { get; set; } = string.Empty;
  public string Domain { get; set; } = string.Empty;
  public string? Name { get; set; }
  public bool IsPrimary { get; set; }
  public bool IsVerified { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public int UserCount { get; set; }
  public int GroupCount { get; set; }
}

public class GoogleWorkspaceUserResponse
{
  public string Id { get; set; } = string.Empty;
  public string DomainId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? DisplayName { get; set; }
  public string? Avatar { get; set; }
  public string? Department { get; set; }
  public string? JobTitle { get; set; }
  public string? Location { get; set; }
  public bool IsAdmin { get; set; }
  public bool IsSuspended { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public int GroupCount { get; set; }
}

public class GoogleWorkspaceGroupResponse
{
  public string Id { get; set; } = string.Empty;
  public string DomainId { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? Email { get; set; }
  public string? Description { get; set; }
  public string? Type { get; set; }
  public int MemberCount { get; set; }
  public DateTime? LastSyncAt { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}

// Integration management DTOs
public class SyncIntegrationRequest
{
  public string IntegrationId { get; set; } = string.Empty;
  public string SyncType { get; set; } = "full"; // full, incremental
  public string? EntityType { get; set; } // project, task, user, etc.
  public string? EntityId { get; set; }
}

public class TestIntegrationRequest
{
  public string IntegrationId { get; set; } = string.Empty;
  public string TestType { get; set; } = "connection"; // connection, webhook, sync
}

public class TestIntegrationResponse
{
  public bool Success { get; set; }
  public string? Message { get; set; }
  public string? ErrorDetails { get; set; }
  public DateTime TestedAt { get; set; }
}

public class IntegrationStatsResponse
{
  public string IntegrationId { get; set; } = string.Empty;
  public string Provider { get; set; } = string.Empty;
  public int TotalEvents { get; set; }
  public int PendingEvents { get; set; }
  public int FailedEvents { get; set; }
  public int TotalSyncs { get; set; }
  public int SuccessfulSyncs { get; set; }
  public int FailedSyncs { get; set; }
  public DateTime? LastSuccessfulSync { get; set; }
  public DateTime? LastFailedSync { get; set; }
  public string? LastError { get; set; }
}

public class WebhookPayload
{
  public string EventType { get; set; } = string.Empty;
  public string Provider { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public string Payload { get; set; } = string.Empty;
  public DateTime Timestamp { get; set; }
  public string? Signature { get; set; }
}
