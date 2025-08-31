using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class TwoFactorAuth
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string UserId { get; set; } = string.Empty;

  [Required]
  public string SecretKey { get; set; } = string.Empty;

  public bool IsEnabled { get; set; } = false;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? EnabledAt { get; set; }

  public DateTime? LastUsedAt { get; set; }

  public int FailedAttempts { get; set; } = 0;

  public DateTime? LockedUntil { get; set; }

  // Navigation properties
  public virtual User User { get; set; } = null!;
  public virtual ICollection<TwoFactorCode> Codes { get; set; } = new List<TwoFactorCode>();
}

public class TwoFactorCode
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string TwoFactorAuthId { get; set; } = string.Empty;

  [Required, StringLength(6)]
  public string Code { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime ExpiresAt { get; set; }

  public bool IsUsed { get; set; } = false;

  public DateTime? UsedAt { get; set; }

  public string? IpAddress { get; set; }

  public string? UserAgent { get; set; }

  // Navigation properties
  public virtual TwoFactorAuth TwoFactorAuth { get; set; } = null!;
}

public class BackupCode
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string TwoFactorAuthId { get; set; } = string.Empty;

  [Required, StringLength(10)]
  public string Code { get; set; } = string.Empty;

  public bool IsUsed { get; set; } = false;

  public DateTime? UsedAt { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual TwoFactorAuth TwoFactorAuth { get; set; } = null!;
}
