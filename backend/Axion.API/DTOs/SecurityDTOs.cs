namespace Axion.API.DTOs;

public class SSOProviderResponse
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string ProviderType { get; set; } = string.Empty;
  public bool IsEnabled { get; set; }
  public bool AutoProvisionUsers { get; set; }
  public string? DefaultRole { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}

public class CreateSSOProviderRequest
{
  public string Name { get; set; } = string.Empty;
  public string ProviderType { get; set; } = string.Empty;
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
  public string AuthorizationEndpoint { get; set; } = string.Empty;
  public string TokenEndpoint { get; set; } = string.Empty;
  public string UserInfoEndpoint { get; set; } = string.Empty;
  public string? Scopes { get; set; }
  public bool AutoProvisionUsers { get; set; } = false;
  public string? DefaultRole { get; set; }
}

public class UpdateSSOProviderRequest
{
  public string Name { get; set; } = string.Empty;
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
  public string AuthorizationEndpoint { get; set; } = string.Empty;
  public string TokenEndpoint { get; set; } = string.Empty;
  public string UserInfoEndpoint { get; set; } = string.Empty;
  public string? Scopes { get; set; }
  public bool IsEnabled { get; set; }
  public bool AutoProvisionUsers { get; set; }
  public string? DefaultRole { get; set; }
}

public class SSOAuthRequest
{
  public string ProviderId { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
}

public class SSOAuthResponse
{
  public string Token { get; set; } = string.Empty;
  public string RefreshToken { get; set; } = string.Empty;
  public DateTime ExpiresAt { get; set; }
  public UserResponse User { get; set; } = null!;
  public bool IsNewUser { get; set; }
}

public class TwoFactorSetupRequest
{
  public string UserId { get; set; } = string.Empty;
}

public class TwoFactorSetupResponse
{
  public string SecretKey { get; set; } = string.Empty;
  public string QrCodeUrl { get; set; } = string.Empty;
  public List<string> BackupCodes { get; set; } = new List<string>();
}

public class EnableTwoFactorRequest
{
  public string Code { get; set; } = string.Empty;
}

public class VerifyTwoFactorRequest
{
  public string Code { get; set; } = string.Empty;
}

public class VerifyTwoFactorResponse
{
  public bool Success { get; set; }
  public string? Token { get; set; }
  public string? ErrorMessage { get; set; }
}

public class DisableTwoFactorRequest
{
  public string Code { get; set; } = string.Empty;
}

public class GenerateBackupCodesResponse
{
  public List<string> BackupCodes { get; set; } = new List<string>();
}

public class AuditLogResponse
{
  public string Id { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
  public string? UserEmail { get; set; }
  public string? UserName { get; set; }
  public string Action { get; set; } = string.Empty;
  public string EntityType { get; set; } = string.Empty;
  public string? EntityId { get; set; }
  public string? EntityName { get; set; }
  public string? OldValues { get; set; }
  public string? NewValues { get; set; }
  public string? IpAddress { get; set; }
  public string? UserAgent { get; set; }
  public DateTime Timestamp { get; set; }
  public bool Success { get; set; }
  public string? ErrorMessage { get; set; }
  public int? ResponseTimeMs { get; set; }
}

public class AuditLogSearchRequest
{
  public string? UserId { get; set; }
  public string? Action { get; set; }
  public string? EntityType { get; set; }
  public string? EntityId { get; set; }
  public DateTime? FromDate { get; set; }
  public DateTime? ToDate { get; set; }
  public bool? Success { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 50;
}

public class AuditLogSearchResponse
{
  public List<AuditLogResponse> Logs { get; set; } = new List<AuditLogResponse>();
  public int TotalCount { get; set; }
  public int Page { get; set; }
  public int PageSize { get; set; }
  public int TotalPages { get; set; }
}

public class SecuritySettingsResponse
{
  public bool TwoFactorEnabled { get; set; }
  public bool TwoFactorRequired { get; set; }
  public List<SSOProviderResponse> SSOProviders { get; set; } = new List<SSOProviderResponse>();
  public int PasswordMinLength { get; set; } = 8;
  public bool RequireUppercase { get; set; } = true;
  public bool RequireLowercase { get; set; } = true;
  public bool RequireNumbers { get; set; } = true;
  public bool RequireSpecialChars { get; set; } = true;
  public int SessionTimeoutMinutes { get; set; } = 480; // 8 hours
  public int MaxFailedLoginAttempts { get; set; } = 5;
  public int LockoutDurationMinutes { get; set; } = 30;
}
