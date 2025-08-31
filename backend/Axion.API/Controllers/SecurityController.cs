using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axion.API.Data;
using Axion.API.Models;
using Axion.API.DTOs;
using Axion.API.Services;
using System.Security.Claims;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecurityController : ControllerBase
{
  private readonly AxionDbContext _context;
  private readonly ISSOService _ssoService;
  private readonly ITwoFactorService _twoFactorService;
  private readonly IAuditService _auditService;
  private readonly ILogger<SecurityController> _logger;

  public SecurityController(
      AxionDbContext context,
      ISSOService ssoService,
      ITwoFactorService twoFactorService,
      IAuditService auditService,
      ILogger<SecurityController> logger)
  {
    _context = context;
    _ssoService = ssoService;
    _twoFactorService = twoFactorService;
    _auditService = auditService;
    _logger = logger;
  }

  // SSO Endpoints
  [HttpGet("sso/providers")]
  public async Task<ActionResult<List<SSOProviderResponse>>> GetSSOProviders()
  {
    try
    {
      var providers = await _ssoService.GetEnabledProvidersAsync();
      return providers;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get SSO providers");
      return StatusCode(500, "Failed to retrieve SSO providers");
    }
  }

  [HttpGet("sso/providers/{providerId}/auth-url")]
  public async Task<ActionResult<string>> GetSSOAuthUrl(string providerId, [FromQuery] string redirectUri, [FromQuery] string state)
  {
    try
    {
      var authUrl = await _ssoService.GetAuthorizationUrlAsync(providerId, redirectUri, state);
      return Ok(new { authUrl });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to generate SSO auth URL for provider {ProviderId}", providerId);
      return BadRequest("Failed to generate authorization URL");
    }
  }

  [HttpPost("sso/authenticate")]
  public async Task<ActionResult<SSOAuthResponse>> AuthenticateSSO([FromBody] SSOAuthRequest request)
  {
    try
    {
      var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
      var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

      var response = await _ssoService.AuthenticateAsync(request, ipAddress, userAgent);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "SSO authentication failed");
      return BadRequest("SSO authentication failed");
    }
  }

  // Two-Factor Authentication Endpoints
  [HttpPost("2fa/setup")]
  public async Task<ActionResult<TwoFactorSetupResponse>> SetupTwoFactor([FromBody] TwoFactorSetupRequest request)
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      var response = await _twoFactorService.SetupAsync(userId);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to setup 2FA for user");
      return BadRequest("Failed to setup two-factor authentication");
    }
  }

  [HttpPost("2fa/enable")]
  public async Task<ActionResult<bool>> EnableTwoFactor([FromBody] EnableTwoFactorRequest request)
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      var success = await _twoFactorService.EnableAsync(userId, request.Code);
      return Ok(success);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to enable 2FA for user");
      return BadRequest("Failed to enable two-factor authentication");
    }
  }

  [HttpPost("2fa/disable")]
  public async Task<ActionResult<bool>> DisableTwoFactor([FromBody] DisableTwoFactorRequest request)
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      var success = await _twoFactorService.DisableAsync(userId, request.Code);
      return Ok(success);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to disable 2FA for user");
      return BadRequest("Failed to disable two-factor authentication");
    }
  }

  [HttpPost("2fa/verify")]
  public async Task<ActionResult<VerifyTwoFactorResponse>> VerifyTwoFactor([FromBody] VerifyTwoFactorRequest request)
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      var response = await _twoFactorService.VerifyAsync(userId, request.Code);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to verify 2FA code");
      return BadRequest("Failed to verify two-factor authentication code");
    }
  }

  [HttpPost("2fa/backup-codes/generate")]
  public async Task<ActionResult<GenerateBackupCodesResponse>> GenerateBackupCodes()
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      var response = await _twoFactorService.GenerateBackupCodesAsync(userId);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to generate backup codes");
      return BadRequest("Failed to generate backup codes");
    }
  }

  [HttpPost("2fa/backup-codes/verify")]
  public async Task<ActionResult<bool>> VerifyBackupCode([FromBody] VerifyTwoFactorRequest request)
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      var success = await _twoFactorService.VerifyBackupCodeAsync(userId, request.Code);
      return Ok(success);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to verify backup code");
      return BadRequest("Failed to verify backup code");
    }
  }

  [HttpGet("2fa/status")]
  public async Task<ActionResult<bool>> GetTwoFactorStatus()
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      var isEnabled = await _twoFactorService.IsEnabledAsync(userId);
      return Ok(isEnabled);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get 2FA status");
      return BadRequest("Failed to get two-factor authentication status");
    }
  }

  // Audit Logging Endpoints
  [HttpPost("audit/search")]
  public async Task<ActionResult<AuditLogSearchResponse>> SearchAuditLogs([FromBody] AuditLogSearchRequest request)
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      // Check if user has admin privileges
      var user = await _context.Users.FindAsync(userId);
      if (user?.Role != UserRole.Admin)
        return Forbid();

      var response = await _auditService.SearchAsync(request);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to search audit logs");
      return BadRequest("Failed to search audit logs");
    }
  }

  [HttpGet("audit/user/{userId}")]
  public async Task<ActionResult<List<AuditLogResponse>>> GetUserActivity(string userId, [FromQuery] int limit = 50)
  {
    try
    {
      var currentUserId = GetUserId();
      if (currentUserId == null) return Unauthorized();

      // Users can only view their own activity, or admins can view any user's activity
      var currentUser = await _context.Users.FindAsync(currentUserId);
      if (currentUser?.Role != UserRole.Admin && currentUserId != userId)
        return Forbid();

      var logs = await _auditService.GetUserActivityAsync(userId, limit);
      return logs;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get user activity for user {UserId}", userId);
      return BadRequest("Failed to get user activity");
    }
  }

  [HttpGet("audit/entity/{entityType}/{entityId}")]
  public async Task<ActionResult<List<AuditLogResponse>>> GetEntityHistory(string entityType, string entityId, [FromQuery] int limit = 50)
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      // Check if user has admin privileges
      var user = await _context.Users.FindAsync(userId);
      if (user?.Role != UserRole.Admin)
        return Forbid();

      var logs = await _auditService.GetEntityHistoryAsync(entityType, entityId, limit);
      return logs;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get entity history for {EntityType} {EntityId}", entityType, entityId);
      return BadRequest("Failed to get entity history");
    }
  }

  // Security Settings Endpoints
  [HttpGet("settings")]
  public async Task<ActionResult<SecuritySettingsResponse>> GetSecuritySettings()
  {
    try
    {
      var userId = GetUserId();
      if (userId == null) return Unauthorized();

      var twoFactorEnabled = await _twoFactorService.IsEnabledAsync(userId);
      var ssoProviders = await _ssoService.GetEnabledProvidersAsync();

      var settings = new SecuritySettingsResponse
      {
        TwoFactorEnabled = twoFactorEnabled,
        TwoFactorRequired = false, // Could be configurable per organization
        SSOProviders = ssoProviders,
        PasswordMinLength = 8,
        RequireUppercase = true,
        RequireLowercase = true,
        RequireNumbers = true,
        RequireSpecialChars = true,
        SessionTimeoutMinutes = 480,
        MaxFailedLoginAttempts = 5,
        LockoutDurationMinutes = 30
      };

      return settings;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get security settings");
      return BadRequest("Failed to get security settings");
    }
  }

  private string? GetUserId()
  {
    return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  }
}
