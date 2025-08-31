using System.Net.Http;
using System.Text.Json;
using Axion.API.Data;
using Axion.API.Models;
using Axion.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Axion.API.Services;

public interface ISSOService
{
  Task<string> GetAuthorizationUrlAsync(string providerId, string redirectUri, string state);
  Task<SSOAuthResponse> AuthenticateAsync(SSOAuthRequest request, string ipAddress, string userAgent);
  Task<SSOProviderResponse> GetProviderAsync(string providerId);
  Task<List<SSOProviderResponse>> GetEnabledProvidersAsync();
}

public class SSOService : ISSOService
{
  private readonly AxionDbContext _context;
  private readonly IAuthService _authService;
  private readonly IAuditService _auditService;
  private readonly HttpClient _httpClient;
  private readonly ILogger<SSOService> _logger;

  public SSOService(
      AxionDbContext context,
      IAuthService authService,
      IAuditService auditService,
      HttpClient httpClient,
      ILogger<SSOService> logger)
  {
    _context = context;
    _authService = authService;
    _auditService = auditService;
    _httpClient = httpClient;
    _logger = logger;
  }

  public async Task<string> GetAuthorizationUrlAsync(string providerId, string redirectUri, string state)
  {
    var provider = await _context.SSOProviders
        .FirstOrDefaultAsync(p => p.Id == providerId && p.IsEnabled);

    if (provider == null)
      throw new InvalidOperationException("SSO provider not found or disabled");

    var scopes = string.IsNullOrEmpty(provider.Scopes) ? "openid email profile" : provider.Scopes;

    var queryParams = new Dictionary<string, string>
    {
      ["client_id"] = provider.ClientId,
      ["redirect_uri"] = redirectUri,
      ["response_type"] = "code",
      ["scope"] = scopes,
      ["state"] = state
    };

    var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
    return $"{provider.AuthorizationEndpoint}?{queryString}";
  }

  public async Task<SSOAuthResponse> AuthenticateAsync(SSOAuthRequest request, string ipAddress, string userAgent)
  {
    var provider = await _context.SSOProviders
        .FirstOrDefaultAsync(p => p.Id == request.ProviderId && p.IsEnabled);

    if (provider == null)
      throw new InvalidOperationException("SSO provider not found or disabled");

    try
    {
      // Exchange authorization code for access token
      var tokenResponse = await ExchangeCodeForTokenAsync(provider, request.Code, request.State);

      // Get user info from provider
      var userInfo = await GetUserInfoAsync(provider, tokenResponse.AccessToken);

      // Find or create user
      var user = await FindOrCreateUserAsync(provider, userInfo);

      // Log the SSO login
      await LogSSOLoginAsync(provider, user, userInfo, ipAddress, userAgent, true);

      // Generate JWT token
      var token = _authService.GenerateJwtToken(user);

      return new SSOAuthResponse
      {
        Token = token,
        RefreshToken = Guid.NewGuid().ToString(), // In a real implementation, store refresh tokens
        ExpiresAt = DateTime.UtcNow.AddHours(8),
        User = new UserResponse
        {
          Id = user.Id,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Email = user.Email,
          Role = user.Role.ToString()
        },
        IsNewUser = user.CreatedAt > DateTime.UtcNow.AddMinutes(-5) // Rough check for new user
      };
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "SSO authentication failed for provider {ProviderId}", request.ProviderId);
      await LogSSOLoginAsync(provider, null, null, ipAddress, userAgent, false, ex.Message);
      throw;
    }
  }

  private async Task<TokenResponse> ExchangeCodeForTokenAsync(SSOProvider provider, string code, string state)
  {
    var tokenRequest = new FormUrlEncodedContent(new[]
    {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("client_id", provider.ClientId),
            new KeyValuePair<string, string>("client_secret", provider.ClientSecret),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", "http://localhost:3000/auth/callback") // Should be configurable
        });

    var response = await _httpClient.PostAsync(provider.TokenEndpoint, tokenRequest);
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<TokenResponse>(content) ?? throw new InvalidOperationException("Invalid token response");
  }

  private async Task<UserInfo> GetUserInfoAsync(SSOProvider provider, string accessToken)
  {
    var request = new HttpRequestMessage(HttpMethod.Get, provider.UserInfoEndpoint);
    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

    var response = await _httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<UserInfo>(content) ?? throw new InvalidOperationException("Invalid user info response");
  }

  private async Task<User> FindOrCreateUserAsync(SSOProvider provider, UserInfo userInfo)
  {
    // Try to find existing user by email
    var existingUser = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == userInfo.Email);

    if (existingUser != null)
    {
      // Update user info if needed
      if (string.IsNullOrEmpty(existingUser.FirstName) && !string.IsNullOrEmpty(userInfo.GivenName))
        existingUser.FirstName = userInfo.GivenName;

      if (string.IsNullOrEmpty(existingUser.LastName) && !string.IsNullOrEmpty(userInfo.FamilyName))
        existingUser.LastName = userInfo.FamilyName;

      await _context.SaveChangesAsync();
      return existingUser;
    }

    // Create new user if auto-provisioning is enabled
    if (provider.AutoProvisionUsers)
    {
      var newUser = new User
      {
        FirstName = userInfo.GivenName ?? userInfo.Name?.Split(' ').FirstOrDefault() ?? "User",
        LastName = userInfo.FamilyName ?? userInfo.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
        Email = userInfo.Email,
        Role = Enum.Parse<UserRole>(provider.DefaultRole ?? "User"),
        OrganizationId = provider.OrganizationId,
        IsActive = true
      };

      _context.Users.Add(newUser);
      await _context.SaveChangesAsync();
      return newUser;
    }

    throw new InvalidOperationException("User not found and auto-provisioning is disabled");
  }

  private async Task LogSSOLoginAsync(SSOProvider provider, User? user, UserInfo? userInfo, string ipAddress, string userAgent, bool success, string? errorMessage = null)
  {
    var login = new SSOLogin
    {
      ProviderId = provider.Id,
      UserId = user?.Id ?? "unknown",
      ExternalUserId = userInfo?.Sub ?? "unknown",
      Email = userInfo?.Email,
      Name = userInfo?.Name,
      Picture = userInfo?.Picture,
      IpAddress = ipAddress,
      UserAgent = userAgent,
      Success = success,
      ErrorMessage = errorMessage
    };

    _context.SSOLogins.Add(login);
    await _context.SaveChangesAsync();

    // Log to audit trail
    await _auditService.LogAsync(
        user?.Id ?? "system",
        success ? AuditAction.SSOLogin : AuditAction.SSOLoginFailed,
        AuditEntityType.User,
        user?.Id,
        user?.Email,
        success: success,
        errorMessage: errorMessage,
        ipAddress: ipAddress,
        userAgent: userAgent
    );
  }

  public async Task<SSOProviderResponse> GetProviderAsync(string providerId)
  {
    var provider = await _context.SSOProviders
        .FirstOrDefaultAsync(p => p.Id == providerId);

    if (provider == null)
      throw new InvalidOperationException("SSO provider not found");

    return new SSOProviderResponse
    {
      Id = provider.Id,
      Name = provider.Name,
      ProviderType = provider.ProviderType,
      IsEnabled = provider.IsEnabled,
      AutoProvisionUsers = provider.AutoProvisionUsers,
      DefaultRole = provider.DefaultRole,
      CreatedAt = provider.CreatedAt,
      UpdatedAt = provider.UpdatedAt
    };
  }

  public async Task<List<SSOProviderResponse>> GetEnabledProvidersAsync()
  {
    var providers = await _context.SSOProviders
        .Where(p => p.IsEnabled)
        .Select(p => new SSOProviderResponse
        {
          Id = p.Id,
          Name = p.Name,
          ProviderType = p.ProviderType,
          IsEnabled = p.IsEnabled,
          AutoProvisionUsers = p.AutoProvisionUsers,
          DefaultRole = p.DefaultRole,
          CreatedAt = p.CreatedAt,
          UpdatedAt = p.UpdatedAt
        })
        .ToListAsync();

    return providers;
  }
}

// Helper classes for OAuth responses
public class TokenResponse
{
  public string AccessToken { get; set; } = string.Empty;
  public string TokenType { get; set; } = string.Empty;
  public int ExpiresIn { get; set; }
  public string? RefreshToken { get; set; }
  public string? Scope { get; set; }
}

public class UserInfo
{
  public string Sub { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string GivenName { get; set; } = string.Empty;
  public string FamilyName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Picture { get; set; } = string.Empty;
}
