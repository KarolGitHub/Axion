using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class SSOProvider
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required, StringLength(50)]
  public string Name { get; set; } = string.Empty;

  [Required, StringLength(100)]
  public string ProviderType { get; set; } = string.Empty; // Google, Microsoft, GitHub, etc.

  [Required]
  public string ClientId { get; set; } = string.Empty;

  [Required]
  public string ClientSecret { get; set; } = string.Empty;

  [Required]
  public string AuthorizationEndpoint { get; set; } = string.Empty;

  [Required]
  public string TokenEndpoint { get; set; } = string.Empty;

  [Required]
  public string UserInfoEndpoint { get; set; } = string.Empty;

  public string? Scopes { get; set; }

  public bool IsEnabled { get; set; } = true;

  public bool AutoProvisionUsers { get; set; } = false;

  public string? DefaultRole { get; set; }

  public string? OrganizationId { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual Organization? Organization { get; set; }
  public virtual ICollection<SSOLogin> Logins { get; set; } = new List<SSOLogin>();
}

public class SSOLogin
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string ProviderId { get; set; } = string.Empty;

  [Required]
  public string UserId { get; set; } = string.Empty;

  [Required]
  public string ExternalUserId { get; set; } = string.Empty;

  public string? Email { get; set; }

  public string? Name { get; set; }

  public string? Picture { get; set; }

  public DateTime LoginAt { get; set; } = DateTime.UtcNow;

  public string? IpAddress { get; set; }

  public string? UserAgent { get; set; }

  public bool Success { get; set; } = true;

  public string? ErrorMessage { get; set; }

  // Navigation properties
  public virtual SSOProvider Provider { get; set; } = null!;
  public virtual User User { get; set; } = null!;
}
