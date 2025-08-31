using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Axion.API.Services;
using Axion.API.DTOs;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IntegrationsController : ControllerBase
{
  private readonly IThirdPartyIntegrationService _integrationService;
  private readonly ILogger<IntegrationsController> _logger;

  public IntegrationsController(
      IThirdPartyIntegrationService integrationService,
      ILogger<IntegrationsController> logger)
  {
    _integrationService = integrationService;
    _logger = logger;
  }

  // General integration management
  [HttpGet]
  public async Task<ActionResult<List<ThirdPartyIntegrationResponse>>> GetIntegrations([FromQuery] string? organizationId)
  {
    try
    {
      var integrations = await _integrationService.GetIntegrationsAsync(organizationId);
      return Ok(integrations);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get integrations");
      return StatusCode(500, "Failed to retrieve integrations");
    }
  }

  [HttpGet("{integrationId}")]
  public async Task<ActionResult<ThirdPartyIntegrationResponse>> GetIntegration(string integrationId)
  {
    try
    {
      var integration = await _integrationService.GetIntegrationAsync(integrationId);
      return Ok(integration);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to retrieve integration");
    }
  }

  [HttpPost]
  public async Task<ActionResult<ThirdPartyIntegrationResponse>> CreateIntegration(CreateThirdPartyIntegrationRequest request)
  {
    try
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userId))
        return Unauthorized();

      var integration = await _integrationService.CreateIntegrationAsync(request, userId);
      return CreatedAtAction(nameof(GetIntegration), new { integrationId = integration.Id }, integration);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to create integration");
      return StatusCode(500, "Failed to create integration");
    }
  }

  [HttpPut("{integrationId}")]
  public async Task<ActionResult<ThirdPartyIntegrationResponse>> UpdateIntegration(string integrationId, UpdateThirdPartyIntegrationRequest request)
  {
    try
    {
      var integration = await _integrationService.UpdateIntegrationAsync(integrationId, request);
      return Ok(integration);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to update integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to update integration");
    }
  }

  [HttpDelete("{integrationId}")]
  public async Task<ActionResult> DeleteIntegration(string integrationId)
  {
    try
    {
      var success = await _integrationService.DeleteIntegrationAsync(integrationId);
      if (!success)
        return NotFound("Integration not found");

      return NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to delete integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to delete integration");
    }
  }

  [HttpPost("{integrationId}/test")]
  public async Task<ActionResult<TestIntegrationResponse>> TestIntegration(string integrationId, TestIntegrationRequest request)
  {
    try
    {
      request.IntegrationId = integrationId;
      var result = await _integrationService.TestIntegrationAsync(request);
      return Ok(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to test integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to test integration");
    }
  }

  [HttpGet("{integrationId}/stats")]
  public async Task<ActionResult<IntegrationStatsResponse>> GetIntegrationStats(string integrationId)
  {
    try
    {
      var stats = await _integrationService.GetIntegrationStatsAsync(integrationId);
      return Ok(stats);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get integration stats {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to retrieve integration stats");
    }
  }

  // Sync operations
  [HttpPost("{integrationId}/sync")]
  public async Task<ActionResult<IntegrationSyncResponse>> SyncIntegration(string integrationId, SyncIntegrationRequest request)
  {
    try
    {
      request.IntegrationId = integrationId;
      var sync = await _integrationService.SyncIntegrationAsync(request);
      return Ok(sync);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to sync integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to sync integration");
    }
  }

  [HttpGet("{integrationId}/syncs")]
  public async Task<ActionResult<List<IntegrationSyncResponse>>> GetIntegrationSyncs(string integrationId)
  {
    try
    {
      var syncs = await _integrationService.GetIntegrationSyncsAsync(integrationId);
      return Ok(syncs);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get integration syncs {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to retrieve integration syncs");
    }
  }

  [HttpGet("{integrationId}/events")]
  public async Task<ActionResult<List<IntegrationEventResponse>>> GetIntegrationEvents(string integrationId)
  {
    try
    {
      var events = await _integrationService.GetIntegrationEventsAsync(integrationId);
      return Ok(events);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get integration events {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to retrieve integration events");
    }
  }

  // GitHub operations
  [HttpGet("{integrationId}/github/repositories")]
  public async Task<ActionResult<List<GitHubRepositoryResponse>>> GetGitHubRepositories(string integrationId)
  {
    try
    {
      var repositories = await _integrationService.GetGitHubRepositoriesAsync(integrationId);
      return Ok(repositories);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get GitHub repositories for integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to retrieve GitHub repositories");
    }
  }

  [HttpGet("github/repositories/{repositoryId}")]
  public async Task<ActionResult<GitHubRepositoryResponse>> GetGitHubRepository(string repositoryId)
  {
    try
    {
      var repository = await _integrationService.GetGitHubRepositoryAsync(repositoryId);
      return Ok(repository);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get GitHub repository {RepositoryId}", repositoryId);
      return StatusCode(500, "Failed to retrieve GitHub repository");
    }
  }

  [HttpGet("github/repositories/{repositoryId}/issues")]
  public async Task<ActionResult<List<GitHubIssueResponse>>> GetGitHubIssues(string repositoryId)
  {
    try
    {
      var issues = await _integrationService.GetGitHubIssuesAsync(repositoryId);
      return Ok(issues);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get GitHub issues for repository {RepositoryId}", repositoryId);
      return StatusCode(500, "Failed to retrieve GitHub issues");
    }
  }

  [HttpGet("github/repositories/{repositoryId}/pull-requests")]
  public async Task<ActionResult<List<GitHubPullRequestResponse>>> GetGitHubPullRequests(string repositoryId)
  {
    try
    {
      var pullRequests = await _integrationService.GetGitHubPullRequestsAsync(repositoryId);
      return Ok(pullRequests);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get GitHub pull requests for repository {RepositoryId}", repositoryId);
      return StatusCode(500, "Failed to retrieve GitHub pull requests");
    }
  }

  [HttpGet("github/issues/{issueId}")]
  public async Task<ActionResult<GitHubIssueResponse>> GetGitHubIssue(string issueId)
  {
    try
    {
      var issue = await _integrationService.GetGitHubIssueAsync(issueId);
      return Ok(issue);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get GitHub issue {IssueId}", issueId);
      return StatusCode(500, "Failed to retrieve GitHub issue");
    }
  }

  [HttpGet("github/pull-requests/{pullRequestId}")]
  public async Task<ActionResult<GitHubPullRequestResponse>> GetGitHubPullRequest(string pullRequestId)
  {
    try
    {
      var pullRequest = await _integrationService.GetGitHubPullRequestAsync(pullRequestId);
      return Ok(pullRequest);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get GitHub pull request {PullRequestId}", pullRequestId);
      return StatusCode(500, "Failed to retrieve GitHub pull request");
    }
  }

  // Slack operations
  [HttpGet("{integrationId}/slack/workspaces")]
  public async Task<ActionResult<List<SlackWorkspaceResponse>>> GetSlackWorkspaces(string integrationId)
  {
    try
    {
      var workspaces = await _integrationService.GetSlackWorkspacesAsync(integrationId);
      return Ok(workspaces);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Slack workspaces for integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to retrieve Slack workspaces");
    }
  }

  [HttpGet("slack/workspaces/{workspaceId}")]
  public async Task<ActionResult<SlackWorkspaceResponse>> GetSlackWorkspace(string workspaceId)
  {
    try
    {
      var workspace = await _integrationService.GetSlackWorkspaceAsync(workspaceId);
      return Ok(workspace);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Slack workspace {WorkspaceId}", workspaceId);
      return StatusCode(500, "Failed to retrieve Slack workspace");
    }
  }

  [HttpGet("slack/workspaces/{workspaceId}/channels")]
  public async Task<ActionResult<List<SlackChannelResponse>>> GetSlackChannels(string workspaceId)
  {
    try
    {
      var channels = await _integrationService.GetSlackChannelsAsync(workspaceId);
      return Ok(channels);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Slack channels for workspace {WorkspaceId}", workspaceId);
      return StatusCode(500, "Failed to retrieve Slack channels");
    }
  }

  [HttpGet("slack/workspaces/{workspaceId}/users")]
  public async Task<ActionResult<List<SlackUserResponse>>> GetSlackUsers(string workspaceId)
  {
    try
    {
      var users = await _integrationService.GetSlackUsersAsync(workspaceId);
      return Ok(users);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Slack users for workspace {WorkspaceId}", workspaceId);
      return StatusCode(500, "Failed to retrieve Slack users");
    }
  }

  [HttpGet("slack/channels/{channelId}/messages")]
  public async Task<ActionResult<List<SlackMessageResponse>>> GetSlackMessages(string channelId)
  {
    try
    {
      var messages = await _integrationService.GetSlackMessagesAsync(channelId);
      return Ok(messages);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Slack messages for channel {ChannelId}", channelId);
      return StatusCode(500, "Failed to retrieve Slack messages");
    }
  }

  // Google Workspace operations
  [HttpGet("{integrationId}/google/domains")]
  public async Task<ActionResult<List<GoogleWorkspaceDomainResponse>>> GetGoogleWorkspaceDomains(string integrationId)
  {
    try
    {
      var domains = await _integrationService.GetGoogleWorkspaceDomainsAsync(integrationId);
      return Ok(domains);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Google Workspace domains for integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to retrieve Google Workspace domains");
    }
  }

  [HttpGet("google/domains/{domainId}")]
  public async Task<ActionResult<GoogleWorkspaceDomainResponse>> GetGoogleWorkspaceDomain(string domainId)
  {
    try
    {
      var domain = await _integrationService.GetGoogleWorkspaceDomainAsync(domainId);
      return Ok(domain);
    }
    catch (InvalidOperationException ex)
    {
      return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Google Workspace domain {DomainId}", domainId);
      return StatusCode(500, "Failed to retrieve Google Workspace domain");
    }
  }

  [HttpGet("google/domains/{domainId}/users")]
  public async Task<ActionResult<List<GoogleWorkspaceUserResponse>>> GetGoogleWorkspaceUsers(string domainId)
  {
    try
    {
      var users = await _integrationService.GetGoogleWorkspaceUsersAsync(domainId);
      return Ok(users);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Google Workspace users for domain {DomainId}", domainId);
      return StatusCode(500, "Failed to retrieve Google Workspace users");
    }
  }

  [HttpGet("google/domains/{domainId}/groups")]
  public async Task<ActionResult<List<GoogleWorkspaceGroupResponse>>> GetGoogleWorkspaceGroups(string domainId)
  {
    try
    {
      var groups = await _integrationService.GetGoogleWorkspaceGroupsAsync(domainId);
      return Ok(groups);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get Google Workspace groups for domain {DomainId}", domainId);
      return StatusCode(500, "Failed to retrieve Google Workspace groups");
    }
  }

  // Webhook endpoint (no authentication required for webhooks)
  [HttpPost("webhook/{integrationId}")]
  [AllowAnonymous]
  public async Task<ActionResult> ProcessWebhook(string integrationId, [FromBody] WebhookPayload payload)
  {
    try
    {
      var success = await _integrationService.ProcessWebhookAsync(integrationId, payload);
      if (success)
        return Ok(new { message = "Webhook processed successfully" });
      else
        return BadRequest(new { message = "Failed to process webhook" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to process webhook for integration {IntegrationId}", integrationId);
      return StatusCode(500, "Failed to process webhook");
    }
  }
}
