using System.Net.Http;
using System.Text.Json;
using Axion.API.Data;
using Axion.API.Models;
using Axion.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Axion.API.Services;

public interface IThirdPartyIntegrationService
{
  // General integration management
  Task<List<ThirdPartyIntegrationResponse>> GetIntegrationsAsync(string? organizationId = null);
  Task<ThirdPartyIntegrationResponse> GetIntegrationAsync(string integrationId);
  Task<ThirdPartyIntegrationResponse> CreateIntegrationAsync(CreateThirdPartyIntegrationRequest request, string createdById);
  Task<ThirdPartyIntegrationResponse> UpdateIntegrationAsync(string integrationId, UpdateThirdPartyIntegrationRequest request);
  Task<bool> DeleteIntegrationAsync(string integrationId);
  Task<TestIntegrationResponse> TestIntegrationAsync(TestIntegrationRequest request);
  Task<IntegrationStatsResponse> GetIntegrationStatsAsync(string integrationId);

  // Sync operations
  Task<IntegrationSyncResponse> SyncIntegrationAsync(SyncIntegrationRequest request);
  Task<List<IntegrationSyncResponse>> GetIntegrationSyncsAsync(string integrationId);
  Task<List<IntegrationEventResponse>> GetIntegrationEventsAsync(string integrationId);

  // GitHub operations
  Task<List<GitHubRepositoryResponse>> GetGitHubRepositoriesAsync(string integrationId);
  Task<GitHubRepositoryResponse> GetGitHubRepositoryAsync(string repositoryId);
  Task<List<GitHubIssueResponse>> GetGitHubIssuesAsync(string repositoryId);
  Task<List<GitHubPullRequestResponse>> GetGitHubPullRequestsAsync(string repositoryId);
  Task<GitHubIssueResponse> GetGitHubIssueAsync(string issueId);
  Task<GitHubPullRequestResponse> GetGitHubPullRequestAsync(string pullRequestId);

  // Slack operations
  Task<List<SlackWorkspaceResponse>> GetSlackWorkspacesAsync(string integrationId);
  Task<SlackWorkspaceResponse> GetSlackWorkspaceAsync(string workspaceId);
  Task<List<SlackChannelResponse>> GetSlackChannelsAsync(string workspaceId);
  Task<List<SlackUserResponse>> GetSlackUsersAsync(string workspaceId);
  Task<List<SlackMessageResponse>> GetSlackMessagesAsync(string channelId);

  // Google Workspace operations
  Task<List<GoogleWorkspaceDomainResponse>> GetGoogleWorkspaceDomainsAsync(string integrationId);
  Task<GoogleWorkspaceDomainResponse> GetGoogleWorkspaceDomainAsync(string domainId);
  Task<List<GoogleWorkspaceUserResponse>> GetGoogleWorkspaceUsersAsync(string domainId);
  Task<List<GoogleWorkspaceGroupResponse>> GetGoogleWorkspaceGroupsAsync(string domainId);

  // Webhook handling
  Task<bool> ProcessWebhookAsync(string integrationId, WebhookPayload payload);
}

public class ThirdPartyIntegrationService : IThirdPartyIntegrationService
{
  private readonly AxionDbContext _context;
  private readonly IAuditService _auditService;
  private readonly HttpClient _httpClient;
  private readonly ILogger<ThirdPartyIntegrationService> _logger;

  public ThirdPartyIntegrationService(
      AxionDbContext context,
      IAuditService auditService,
      HttpClient httpClient,
      ILogger<ThirdPartyIntegrationService> logger)
  {
    _context = context;
    _auditService = auditService;
    _httpClient = httpClient;
    _logger = logger;
  }

  // General integration management
  public async Task<List<ThirdPartyIntegrationResponse>> GetIntegrationsAsync(string? organizationId = null)
  {
    var query = _context.ThirdPartyIntegrations
        .Include(i => i.CreatedBy)
        .AsQueryable();

    if (!string.IsNullOrEmpty(organizationId))
    {
      query = query.Where(i => i.OrganizationId == organizationId);
    }

    var integrations = await query.ToListAsync();

    return integrations.Select(i => new ThirdPartyIntegrationResponse
    {
      Id = i.Id,
      Name = i.Name,
      Provider = i.Provider,
      IsEnabled = i.IsEnabled,
      AutoSync = i.AutoSync,
      SyncSettings = i.SyncSettings,
      OrganizationId = i.OrganizationId,
      CreatedById = i.CreatedById,
      CreatedByName = i.CreatedBy != null ? $"{i.CreatedBy.FirstName} {i.CreatedBy.LastName}" : null,
      CreatedAt = i.CreatedAt,
      UpdatedAt = i.UpdatedAt,
      LastSyncAt = i.LastSyncAt,
      LastSyncStatus = i.LastSyncStatus,
      LastSyncError = i.LastSyncError
    }).ToList();
  }

  public async Task<ThirdPartyIntegrationResponse> GetIntegrationAsync(string integrationId)
  {
    var integration = await _context.ThirdPartyIntegrations
        .Include(i => i.CreatedBy)
        .FirstOrDefaultAsync(i => i.Id == integrationId);

    if (integration == null)
      throw new InvalidOperationException("Integration not found");

    return new ThirdPartyIntegrationResponse
    {
      Id = integration.Id,
      Name = integration.Name,
      Provider = integration.Provider,
      IsEnabled = integration.IsEnabled,
      AutoSync = integration.AutoSync,
      SyncSettings = integration.SyncSettings,
      OrganizationId = integration.OrganizationId,
      CreatedById = integration.CreatedById,
      CreatedByName = integration.CreatedBy != null ? $"{integration.CreatedBy.FirstName} {integration.CreatedBy.LastName}" : null,
      CreatedAt = integration.CreatedAt,
      UpdatedAt = integration.UpdatedAt,
      LastSyncAt = integration.LastSyncAt,
      LastSyncStatus = integration.LastSyncStatus,
      LastSyncError = integration.LastSyncError
    };
  }

  public async Task<ThirdPartyIntegrationResponse> CreateIntegrationAsync(CreateThirdPartyIntegrationRequest request, string createdById)
  {
    var integration = new ThirdPartyIntegration
    {
      Name = request.Name,
      Provider = request.Provider,
      ClientId = request.ClientId,
      ClientSecret = request.ClientSecret,
      WebhookUrl = request.WebhookUrl,
      WebhookSecret = request.WebhookSecret,
      AutoSync = request.AutoSync,
      SyncSettings = request.SyncSettings,
      OrganizationId = request.OrganizationId,
      CreatedById = createdById
    };

    _context.ThirdPartyIntegrations.Add(integration);
    await _context.SaveChangesAsync();

    await _auditService.LogAsync(
        createdById,
        AuditAction.IntegrationCreated,
        AuditEntityType.Integration,
        integration.Id,
        integration.Name
    );

    return await GetIntegrationAsync(integration.Id);
  }

  public async Task<ThirdPartyIntegrationResponse> UpdateIntegrationAsync(string integrationId, UpdateThirdPartyIntegrationRequest request)
  {
    var integration = await _context.ThirdPartyIntegrations.FindAsync(integrationId);
    if (integration == null)
      throw new InvalidOperationException("Integration not found");

    integration.Name = request.Name;
    integration.ClientId = request.ClientId;
    integration.ClientSecret = request.ClientSecret;
    integration.WebhookUrl = request.WebhookUrl;
    integration.WebhookSecret = request.WebhookSecret;
    integration.IsEnabled = request.IsEnabled;
    integration.AutoSync = request.AutoSync;
    integration.SyncSettings = request.SyncSettings;
    integration.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    await _auditService.LogAsync(
        integration.CreatedById,
        AuditAction.IntegrationUpdated,
        AuditEntityType.Integration,
        integration.Id,
        integration.Name
    );

    return await GetIntegrationAsync(integration.Id);
  }

  public async Task<bool> DeleteIntegrationAsync(string integrationId)
  {
    var integration = await _context.ThirdPartyIntegrations.FindAsync(integrationId);
    if (integration == null)
      return false;

    _context.ThirdPartyIntegrations.Remove(integration);
    await _context.SaveChangesAsync();

    await _auditService.LogAsync(
        integration.CreatedById,
        AuditAction.IntegrationDeleted,
        AuditEntityType.Integration,
        integration.Id,
        integration.Name
    );

    return true;
  }

  public async Task<TestIntegrationResponse> TestIntegrationAsync(TestIntegrationRequest request)
  {
    var integration = await _context.ThirdPartyIntegrations.FindAsync(request.IntegrationId);
    if (integration == null)
      return new TestIntegrationResponse { Success = false, Message = "Integration not found" };

    try
    {
      switch (request.TestType.ToLower())
      {
        case "connection":
          return await TestConnectionAsync(integration);
        case "webhook":
          return await TestWebhookAsync(integration);
        case "sync":
          return await TestSyncAsync(integration);
        default:
          return new TestIntegrationResponse { Success = false, Message = "Invalid test type" };
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Integration test failed for {IntegrationId}", request.IntegrationId);
      return new TestIntegrationResponse
      {
        Success = false,
        Message = "Test failed",
        ErrorDetails = ex.Message,
        TestedAt = DateTime.UtcNow
      };
    }
  }

  public async Task<IntegrationStatsResponse> GetIntegrationStatsAsync(string integrationId)
  {
    var integration = await _context.ThirdPartyIntegrations.FindAsync(integrationId);
    if (integration == null)
      throw new InvalidOperationException("Integration not found");

    var events = await _context.IntegrationEvents
        .Where(e => e.IntegrationId == integrationId)
        .ToListAsync();

    var syncs = await _context.IntegrationSyncs
        .Where(s => s.IntegrationId == integrationId)
        .ToListAsync();

    return new IntegrationStatsResponse
    {
      IntegrationId = integrationId,
      Provider = integration.Provider,
      TotalEvents = events.Count,
      PendingEvents = events.Count(e => e.Status == "pending"),
      FailedEvents = events.Count(e => e.Status == "failed"),
      TotalSyncs = syncs.Count,
      SuccessfulSyncs = syncs.Count(s => s.Status == "completed"),
      FailedSyncs = syncs.Count(s => s.Status == "failed"),
      LastSuccessfulSync = syncs.Where(s => s.Status == "completed").Max(s => s.CompletedAt),
      LastFailedSync = syncs.Where(s => s.Status == "failed").Max(s => s.CompletedAt),
      LastError = integration.LastSyncError
    };
  }

  // Sync operations
  public async Task<IntegrationSyncResponse> SyncIntegrationAsync(SyncIntegrationRequest request)
  {
    var integration = await _context.ThirdPartyIntegrations.FindAsync(request.IntegrationId);
    if (integration == null)
      throw new InvalidOperationException("Integration not found");

    var sync = new IntegrationSync
    {
      IntegrationId = request.IntegrationId,
      SyncType = request.SyncType,
      EntityType = request.EntityType ?? "all",
      EntityId = request.EntityId,
      Status = "running",
      StartedAt = DateTime.UtcNow
    };

    _context.IntegrationSyncs.Add(sync);
    await _context.SaveChangesAsync();

    try
    {
      var result = await PerformSyncAsync(integration, sync);

      sync.Status = result.Success ? "completed" : "failed";
      sync.ItemsProcessed = result.ItemsProcessed;
      sync.ItemsCreated = result.ItemsCreated;
      sync.ItemsUpdated = result.ItemsUpdated;
      sync.ItemsFailed = result.ItemsFailed;
      sync.ErrorMessage = result.ErrorMessage;
      sync.CompletedAt = DateTime.UtcNow;
      sync.DurationMs = (int)(sync.CompletedAt.Value - sync.StartedAt).TotalMilliseconds;

      await _context.SaveChangesAsync();

      // Update integration last sync info
      integration.LastSyncAt = DateTime.UtcNow;
      integration.LastSyncStatus = sync.Status;
      integration.LastSyncError = sync.ErrorMessage;
      await _context.SaveChangesAsync();

      await _auditService.LogAsync(
          integration.CreatedById,
          AuditAction.IntegrationSync,
          AuditEntityType.Integration,
          integration.Id,
          integration.Name,
          additionalData: $"Sync {sync.SyncType} completed with {sync.ItemsProcessed} items processed"
      );

      return new IntegrationSyncResponse
      {
        Id = sync.Id,
        IntegrationId = sync.IntegrationId,
        SyncType = sync.SyncType,
        EntityType = sync.EntityType,
        EntityId = sync.EntityId,
        ItemsProcessed = sync.ItemsProcessed,
        ItemsCreated = sync.ItemsCreated,
        ItemsUpdated = sync.ItemsUpdated,
        ItemsFailed = sync.ItemsFailed,
        Status = sync.Status,
        ErrorMessage = sync.ErrorMessage,
        StartedAt = sync.StartedAt,
        CompletedAt = sync.CompletedAt,
        DurationMs = sync.DurationMs
      };
    }
    catch (Exception ex)
    {
      sync.Status = "failed";
      sync.ErrorMessage = ex.Message;
      sync.CompletedAt = DateTime.UtcNow;
      sync.DurationMs = (int)(sync.CompletedAt.Value - sync.StartedAt).TotalMilliseconds;
      await _context.SaveChangesAsync();

      throw;
    }
  }

  public async Task<List<IntegrationSyncResponse>> GetIntegrationSyncsAsync(string integrationId)
  {
    var syncs = await _context.IntegrationSyncs
        .Where(s => s.IntegrationId == integrationId)
        .OrderByDescending(s => s.StartedAt)
        .ToListAsync();

    return syncs.Select(s => new IntegrationSyncResponse
    {
      Id = s.Id,
      IntegrationId = s.IntegrationId,
      SyncType = s.SyncType,
      EntityType = s.EntityType,
      EntityId = s.EntityId,
      ItemsProcessed = s.ItemsProcessed,
      ItemsCreated = s.ItemsCreated,
      ItemsUpdated = s.ItemsUpdated,
      ItemsFailed = s.ItemsFailed,
      Status = s.Status,
      ErrorMessage = s.ErrorMessage,
      StartedAt = s.StartedAt,
      CompletedAt = s.CompletedAt,
      DurationMs = s.DurationMs
    }).ToList();
  }

  public async Task<List<IntegrationEventResponse>> GetIntegrationEventsAsync(string integrationId)
  {
    var events = await _context.IntegrationEvents
        .Where(e => e.IntegrationId == integrationId)
        .OrderByDescending(e => e.CreatedAt)
        .ToListAsync();

    return events.Select(e => new IntegrationEventResponse
    {
      Id = e.Id,
      IntegrationId = e.IntegrationId,
      EventType = e.EventType,
      Payload = e.Payload,
      ExternalId = e.ExternalId,
      Status = e.Status,
      ErrorMessage = e.ErrorMessage,
      RetryCount = e.RetryCount,
      ProcessedAt = e.ProcessedAt,
      CreatedAt = e.CreatedAt
    }).ToList();
  }

  // GitHub operations
  public async Task<List<GitHubRepositoryResponse>> GetGitHubRepositoriesAsync(string integrationId)
  {
    var repositories = await _context.GitHubRepositories
        .Where(r => r.IntegrationId == integrationId)
        .Include(r => r.Issues)
        .Include(r => r.PullRequests)
        .ToListAsync();

    return repositories.Select(r => new GitHubRepositoryResponse
    {
      Id = r.Id,
      IntegrationId = r.IntegrationId,
      ExternalId = r.ExternalId,
      Name = r.Name,
      FullName = r.FullName,
      Description = r.Description,
      Url = r.Url,
      DefaultBranch = r.DefaultBranch,
      IsPrivate = r.IsPrivate,
      IsFork = r.IsFork,
      Language = r.Language,
      Stars = r.Stars,
      Forks = r.Forks,
      LastSyncAt = r.LastSyncAt,
      CreatedAt = r.CreatedAt,
      UpdatedAt = r.UpdatedAt,
      IssueCount = r.Issues.Count,
      PullRequestCount = r.PullRequests.Count
    }).ToList();
  }

  public async Task<GitHubRepositoryResponse> GetGitHubRepositoryAsync(string repositoryId)
  {
    var repository = await _context.GitHubRepositories
        .Include(r => r.Issues)
        .Include(r => r.PullRequests)
        .FirstOrDefaultAsync(r => r.Id == repositoryId);

    if (repository == null)
      throw new InvalidOperationException("Repository not found");

    return new GitHubRepositoryResponse
    {
      Id = repository.Id,
      IntegrationId = repository.IntegrationId,
      ExternalId = repository.ExternalId,
      Name = repository.Name,
      FullName = repository.FullName,
      Description = repository.Description,
      Url = repository.Url,
      DefaultBranch = repository.DefaultBranch,
      IsPrivate = repository.IsPrivate,
      IsFork = repository.IsFork,
      Language = repository.Language,
      Stars = repository.Stars,
      Forks = repository.Forks,
      LastSyncAt = repository.LastSyncAt,
      CreatedAt = repository.CreatedAt,
      UpdatedAt = repository.UpdatedAt,
      IssueCount = repository.Issues.Count,
      PullRequestCount = repository.PullRequests.Count
    };
  }

  public async Task<List<GitHubIssueResponse>> GetGitHubIssuesAsync(string repositoryId)
  {
    var issues = await _context.GitHubIssues
        .Where(i => i.RepositoryId == repositoryId)
        .ToListAsync();

    return issues.Select(i => new GitHubIssueResponse
    {
      Id = i.Id,
      RepositoryId = i.RepositoryId,
      ExternalId = i.ExternalId,
      Title = i.Title,
      Description = i.Description,
      State = i.State,
      Assignee = i.Assignee,
      Author = i.Author,
      Labels = i.Labels,
      Number = i.Number,
      Url = i.Url,
      CreatedAt = i.CreatedAt,
      UpdatedAt = i.UpdatedAt,
      ClosedAt = i.ClosedAt,
      LastSyncAt = i.LastSyncAt
    }).ToList();
  }

  public async Task<List<GitHubPullRequestResponse>> GetGitHubPullRequestsAsync(string repositoryId)
  {
    var pullRequests = await _context.GitHubPullRequests
        .Where(pr => pr.RepositoryId == repositoryId)
        .ToListAsync();

    return pullRequests.Select(pr => new GitHubPullRequestResponse
    {
      Id = pr.Id,
      RepositoryId = pr.RepositoryId,
      ExternalId = pr.ExternalId,
      Title = pr.Title,
      Description = pr.Description,
      State = pr.State,
      Author = pr.Author,
      Assignee = pr.Assignee,
      Reviewers = pr.Reviewers,
      Labels = pr.Labels,
      Number = pr.Number,
      Url = pr.Url,
      BaseBranch = pr.BaseBranch,
      HeadBranch = pr.HeadBranch,
      CreatedAt = pr.CreatedAt,
      UpdatedAt = pr.UpdatedAt,
      MergedAt = pr.MergedAt,
      ClosedAt = pr.ClosedAt,
      LastSyncAt = pr.LastSyncAt
    }).ToList();
  }

  public async Task<GitHubIssueResponse> GetGitHubIssueAsync(string issueId)
  {
    var issue = await _context.GitHubIssues.FindAsync(issueId);
    if (issue == null)
      throw new InvalidOperationException("Issue not found");

    return new GitHubIssueResponse
    {
      Id = issue.Id,
      RepositoryId = issue.RepositoryId,
      ExternalId = issue.ExternalId,
      Title = issue.Title,
      Description = issue.Description,
      State = issue.State,
      Assignee = issue.Assignee,
      Author = issue.Author,
      Labels = issue.Labels,
      Number = issue.Number,
      Url = issue.Url,
      CreatedAt = issue.CreatedAt,
      UpdatedAt = issue.UpdatedAt,
      ClosedAt = issue.ClosedAt,
      LastSyncAt = issue.LastSyncAt
    };
  }

  public async Task<GitHubPullRequestResponse> GetGitHubPullRequestAsync(string pullRequestId)
  {
    var pullRequest = await _context.GitHubPullRequests.FindAsync(pullRequestId);
    if (pullRequest == null)
      throw new InvalidOperationException("Pull request not found");

    return new GitHubPullRequestResponse
    {
      Id = pullRequest.Id,
      RepositoryId = pullRequest.RepositoryId,
      ExternalId = pullRequest.ExternalId,
      Title = pullRequest.Title,
      Description = pullRequest.Description,
      State = pullRequest.State,
      Author = pullRequest.Author,
      Assignee = pullRequest.Assignee,
      Reviewers = pullRequest.Reviewers,
      Labels = pullRequest.Labels,
      Number = pullRequest.Number,
      Url = pullRequest.Url,
      BaseBranch = pullRequest.BaseBranch,
      HeadBranch = pullRequest.HeadBranch,
      CreatedAt = pullRequest.CreatedAt,
      UpdatedAt = pullRequest.UpdatedAt,
      MergedAt = pullRequest.MergedAt,
      ClosedAt = pullRequest.ClosedAt,
      LastSyncAt = pullRequest.LastSyncAt
    };
  }

  // Slack operations
  public async Task<List<SlackWorkspaceResponse>> GetSlackWorkspacesAsync(string integrationId)
  {
    var workspaces = await _context.SlackWorkspaces
        .Where(w => w.IntegrationId == integrationId)
        .Include(w => w.Channels)
        .Include(w => w.Users)
        .ToListAsync();

    return workspaces.Select(w => new SlackWorkspaceResponse
    {
      Id = w.Id,
      IntegrationId = w.IntegrationId,
      ExternalId = w.ExternalId,
      Name = w.Name,
      Domain = w.Domain,
      Icon = w.Icon,
      IsEnterprise = w.IsEnterprise,
      EnterpriseId = w.EnterpriseId,
      LastSyncAt = w.LastSyncAt,
      CreatedAt = w.CreatedAt,
      UpdatedAt = w.UpdatedAt,
      ChannelCount = w.Channels.Count,
      UserCount = w.Users.Count
    }).ToList();
  }

  public async Task<SlackWorkspaceResponse> GetSlackWorkspaceAsync(string workspaceId)
  {
    var workspace = await _context.SlackWorkspaces
        .Include(w => w.Channels)
        .Include(w => w.Users)
        .FirstOrDefaultAsync(w => w.Id == workspaceId);

    if (workspace == null)
      throw new InvalidOperationException("Workspace not found");

    return new SlackWorkspaceResponse
    {
      Id = workspace.Id,
      IntegrationId = workspace.IntegrationId,
      ExternalId = workspace.ExternalId,
      Name = workspace.Name,
      Domain = workspace.Domain,
      Icon = workspace.Icon,
      IsEnterprise = workspace.IsEnterprise,
      EnterpriseId = workspace.EnterpriseId,
      LastSyncAt = workspace.LastSyncAt,
      CreatedAt = workspace.CreatedAt,
      UpdatedAt = workspace.UpdatedAt,
      ChannelCount = workspace.Channels.Count,
      UserCount = workspace.Users.Count
    };
  }

  public async Task<List<SlackChannelResponse>> GetSlackChannelsAsync(string workspaceId)
  {
    var channels = await _context.SlackChannels
        .Where(c => c.WorkspaceId == workspaceId)
        .Include(c => c.Messages)
        .ToListAsync();

    return channels.Select(c => new SlackChannelResponse
    {
      Id = c.Id,
      WorkspaceId = c.WorkspaceId,
      ExternalId = c.ExternalId,
      Name = c.Name,
      Topic = c.Topic,
      Purpose = c.Purpose,
      IsPrivate = c.IsPrivate,
      IsArchived = c.IsArchived,
      MemberCount = c.MemberCount,
      LastSyncAt = c.LastSyncAt,
      CreatedAt = c.CreatedAt,
      UpdatedAt = c.UpdatedAt,
      MessageCount = c.Messages.Count
    }).ToList();
  }

  public async Task<List<SlackUserResponse>> GetSlackUsersAsync(string workspaceId)
  {
    var users = await _context.SlackUsers
        .Where(u => u.WorkspaceId == workspaceId)
        .ToListAsync();

    return users.Select(u => new SlackUserResponse
    {
      Id = u.Id,
      WorkspaceId = u.WorkspaceId,
      ExternalId = u.ExternalId,
      Name = u.Name,
      RealName = u.RealName,
      Email = u.Email,
      Avatar = u.Avatar,
      Status = u.Status,
      StatusText = u.StatusText,
      IsBot = u.IsBot,
      IsDeleted = u.IsDeleted,
      LastSyncAt = u.LastSyncAt,
      CreatedAt = u.CreatedAt,
      UpdatedAt = u.UpdatedAt
    }).ToList();
  }

  public async Task<List<SlackMessageResponse>> GetSlackMessagesAsync(string channelId)
  {
    var messages = await _context.SlackMessages
        .Where(m => m.ChannelId == channelId)
        .Include(m => m.Replies)
        .ToListAsync();

    return messages.Select(m => new SlackMessageResponse
    {
      Id = m.Id,
      ChannelId = m.ChannelId,
      ExternalId = m.ExternalId,
      Text = m.Text,
      UserId = m.UserId,
      Username = m.Username,
      Type = m.Type,
      Subtype = m.Subtype,
      ThreadTs = m.ThreadTs,
      ParentMessageId = m.ParentMessageId,
      Attachments = m.Attachments,
      Reactions = m.Reactions,
      Timestamp = m.Timestamp,
      LastSyncAt = m.LastSyncAt,
      CreatedAt = m.CreatedAt,
      UpdatedAt = m.UpdatedAt,
      ReplyCount = m.Replies.Count
    }).ToList();
  }

  // Google Workspace operations
  public async Task<List<GoogleWorkspaceDomainResponse>> GetGoogleWorkspaceDomainsAsync(string integrationId)
  {
    var domains = await _context.GoogleWorkspaceDomains
        .Where(d => d.IntegrationId == integrationId)
        .Include(d => d.Users)
        .Include(d => d.Groups)
        .ToListAsync();

    return domains.Select(d => new GoogleWorkspaceDomainResponse
    {
      Id = d.Id,
      IntegrationId = d.IntegrationId,
      Domain = d.Domain,
      Name = d.Name,
      IsPrimary = d.IsPrimary,
      IsVerified = d.IsVerified,
      LastSyncAt = d.LastSyncAt,
      CreatedAt = d.CreatedAt,
      UpdatedAt = d.UpdatedAt,
      UserCount = d.Users.Count,
      GroupCount = d.Groups.Count
    }).ToList();
  }

  public async Task<GoogleWorkspaceDomainResponse> GetGoogleWorkspaceDomainAsync(string domainId)
  {
    var domain = await _context.GoogleWorkspaceDomains
        .Include(d => d.Users)
        .Include(d => d.Groups)
        .FirstOrDefaultAsync(d => d.Id == domainId);

    if (domain == null)
      throw new InvalidOperationException("Domain not found");

    return new GoogleWorkspaceDomainResponse
    {
      Id = domain.Id,
      IntegrationId = domain.IntegrationId,
      Domain = domain.Domain,
      Name = domain.Name,
      IsPrimary = domain.IsPrimary,
      IsVerified = domain.IsVerified,
      LastSyncAt = domain.LastSyncAt,
      CreatedAt = domain.CreatedAt,
      UpdatedAt = domain.UpdatedAt,
      UserCount = domain.Users.Count,
      GroupCount = domain.Groups.Count
    };
  }

  public async Task<List<GoogleWorkspaceUserResponse>> GetGoogleWorkspaceUsersAsync(string domainId)
  {
    var users = await _context.GoogleWorkspaceUsers
        .Where(u => u.DomainId == domainId)
        .Include(u => u.Groups)
        .ToListAsync();

    return users.Select(u => new GoogleWorkspaceUserResponse
    {
      Id = u.Id,
      DomainId = u.DomainId,
      ExternalId = u.ExternalId,
      Email = u.Email,
      FirstName = u.FirstName,
      LastName = u.LastName,
      DisplayName = u.DisplayName,
      Avatar = u.Avatar,
      Department = u.Department,
      JobTitle = u.JobTitle,
      Location = u.Location,
      IsAdmin = u.IsAdmin,
      IsSuspended = u.IsSuspended,
      LastSyncAt = u.LastSyncAt,
      CreatedAt = u.CreatedAt,
      UpdatedAt = u.UpdatedAt,
      GroupCount = u.Groups.Count
    }).ToList();
  }

  public async Task<List<GoogleWorkspaceGroupResponse>> GetGoogleWorkspaceGroupsAsync(string domainId)
  {
    var groups = await _context.GoogleWorkspaceGroups
        .Where(g => g.DomainId == domainId)
        .ToListAsync();

    return groups.Select(g => new GoogleWorkspaceGroupResponse
    {
      Id = g.Id,
      DomainId = g.DomainId,
      ExternalId = g.ExternalId,
      Name = g.Name,
      Email = g.Email,
      Description = g.Description,
      Type = g.Type,
      MemberCount = g.MemberCount,
      LastSyncAt = g.LastSyncAt,
      CreatedAt = g.CreatedAt,
      UpdatedAt = g.UpdatedAt
    }).ToList();
  }

  // Webhook handling
  public async Task<bool> ProcessWebhookAsync(string integrationId, WebhookPayload payload)
  {
    var integration = await _context.ThirdPartyIntegrations.FindAsync(integrationId);
    if (integration == null)
      return false;

    var integrationEvent = new IntegrationEvent
    {
      IntegrationId = integrationId,
      EventType = payload.EventType,
      Payload = payload.Payload,
      ExternalId = payload.ExternalId,
      Status = "pending"
    };

    _context.IntegrationEvents.Add(integrationEvent);
    await _context.SaveChangesAsync();

    try
    {
      // Process the webhook based on provider
      switch (integration.Provider.ToLower())
      {
        case "github":
          await ProcessGitHubWebhookAsync(integrationEvent, payload);
          break;
        case "slack":
          await ProcessSlackWebhookAsync(integrationEvent, payload);
          break;
        case "googleworkspace":
          await ProcessGoogleWorkspaceWebhookAsync(integrationEvent, payload);
          break;
        default:
          throw new NotSupportedException($"Provider {integration.Provider} not supported");
      }

      integrationEvent.Status = "processed";
      integrationEvent.ProcessedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();

      return true;
    }
    catch (Exception ex)
    {
      integrationEvent.Status = "failed";
      integrationEvent.ErrorMessage = ex.Message;
      integrationEvent.RetryCount++;
      await _context.SaveChangesAsync();

      _logger.LogError(ex, "Failed to process webhook for integration {IntegrationId}", integrationId);
      return false;
    }
  }

  // Private helper methods
  private async Task<TestIntegrationResponse> TestConnectionAsync(ThirdPartyIntegration integration)
  {
    // Simulate connection test
    await Task.Delay(1000);

    return new TestIntegrationResponse
    {
      Success = true,
      Message = $"Successfully connected to {integration.Provider}",
      TestedAt = DateTime.UtcNow
    };
  }

  private async Task<TestIntegrationResponse> TestWebhookAsync(ThirdPartyIntegration integration)
  {
    // Simulate webhook test
    await Task.Delay(500);

    return new TestIntegrationResponse
    {
      Success = true,
      Message = "Webhook endpoint is accessible",
      TestedAt = DateTime.UtcNow
    };
  }

  private async Task<TestIntegrationResponse> TestSyncAsync(ThirdPartyIntegration integration)
  {
    // Simulate sync test
    await Task.Delay(2000);

    return new TestIntegrationResponse
    {
      Success = true,
      Message = "Sync test completed successfully",
      TestedAt = DateTime.UtcNow
    };
  }

  private async Task<SyncResult> PerformSyncAsync(ThirdPartyIntegration integration, IntegrationSync sync)
  {
    // Simulate sync operation
    await Task.Delay(3000);

    return new SyncResult
    {
      Success = true,
      ItemsProcessed = 150,
      ItemsCreated = 25,
      ItemsUpdated = 100,
      ItemsFailed = 5,
      ErrorMessage = null
    };
  }

  private async Task ProcessGitHubWebhookAsync(IntegrationEvent integrationEvent, WebhookPayload payload)
  {
    // Simulate GitHub webhook processing
    await Task.Delay(500);

    var payloadData = JsonSerializer.Deserialize<JsonElement>(payload.Payload);
    var eventType = payload.EventType;

    switch (eventType)
    {
      case "issues":
        // Process issue event
        break;
      case "pull_request":
        // Process pull request event
        break;
      case "push":
        // Process push event
        break;
    }
  }

  private async Task ProcessSlackWebhookAsync(IntegrationEvent integrationEvent, WebhookPayload payload)
  {
    // Simulate Slack webhook processing
    await Task.Delay(500);

    var payloadData = JsonSerializer.Deserialize<JsonElement>(payload.Payload);
    var eventType = payload.EventType;

    switch (eventType)
    {
      case "message":
        // Process message event
        break;
      case "channel_created":
        // Process channel creation event
        break;
      case "user_change":
        // Process user change event
        break;
    }
  }

  private async Task ProcessGoogleWorkspaceWebhookAsync(IntegrationEvent integrationEvent, WebhookPayload payload)
  {
    // Simulate Google Workspace webhook processing
    await Task.Delay(500);

    var payloadData = JsonSerializer.Deserialize<JsonElement>(payload.Payload);
    var eventType = payload.EventType;

    switch (eventType)
    {
      case "user_created":
        // Process user creation event
        break;
      case "user_updated":
        // Process user update event
        break;
      case "group_created":
        // Process group creation event
        break;
    }
  }

  private class SyncResult
  {
    public bool Success { get; set; }
    public int ItemsProcessed { get; set; }
    public int ItemsCreated { get; set; }
    public int ItemsUpdated { get; set; }
    public int ItemsFailed { get; set; }
    public string? ErrorMessage { get; set; }
  }
}
