using System.Security.Cryptography;
using System.Text;
using Axion.API.Data;
using Axion.API.Models;
using Axion.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Axion.API.Services;

public interface ITwoFactorService
{
  Task<TwoFactorSetupResponse> SetupAsync(string userId);
  Task<bool> EnableAsync(string userId, string code);
  Task<bool> DisableAsync(string userId, string code);
  Task<VerifyTwoFactorResponse> VerifyAsync(string userId, string code);
  Task<GenerateBackupCodesResponse> GenerateBackupCodesAsync(string userId);
  Task<bool> VerifyBackupCodeAsync(string userId, string code);
  Task<bool> IsEnabledAsync(string userId);
}

public class TwoFactorService : ITwoFactorService
{
  private readonly AxionDbContext _context;
  private readonly IAuditService _auditService;
  private readonly ILogger<TwoFactorService> _logger;

  public TwoFactorService(
      AxionDbContext context,
      IAuditService auditService,
      ILogger<TwoFactorService> logger)
  {
    _context = context;
    _auditService = auditService;
    _logger = logger;
  }

  public async Task<TwoFactorSetupResponse> SetupAsync(string userId)
  {
    var user = await _context.Users.FindAsync(userId);
    if (user == null)
      throw new InvalidOperationException("User not found");

    // Generate a new secret key
    var secretKey = GenerateSecretKey();

    // Create or update 2FA record
    var twoFactorAuth = await _context.TwoFactorAuths
        .FirstOrDefaultAsync(t => t.UserId == userId);

    if (twoFactorAuth == null)
    {
      twoFactorAuth = new TwoFactorAuth
      {
        UserId = userId,
        SecretKey = secretKey,
        IsEnabled = false
      };
      _context.TwoFactorAuths.Add(twoFactorAuth);
    }
    else
    {
      twoFactorAuth.SecretKey = secretKey;
      twoFactorAuth.IsEnabled = false;
      twoFactorAuth.FailedAttempts = 0;
      twoFactorAuth.LockedUntil = null;
    }

    await _context.SaveChangesAsync();

    // Generate backup codes
    var backupCodes = GenerateBackupCodes();

    // Store backup codes
    foreach (var code in backupCodes)
    {
      var backupCode = new BackupCode
      {
        TwoFactorAuthId = twoFactorAuth.Id,
        Code = HashBackupCode(code)
      };
      _context.BackupCodes.Add(backupCode);
    }

    await _context.SaveChangesAsync();

    // Generate QR code URL
    var qrCodeUrl = GenerateQrCodeUrl(user.Email, secretKey, "Axion");

    await _auditService.LogAsync(
        userId,
        AuditAction.TwoFactorEnabled,
        AuditEntityType.Security,
        userId,
        user.Email
    );

    return new TwoFactorSetupResponse
    {
      SecretKey = secretKey,
      QrCodeUrl = qrCodeUrl,
      BackupCodes = backupCodes
    };
  }

  public async Task<bool> EnableAsync(string userId, string code)
  {
    var twoFactorAuth = await _context.TwoFactorAuths
        .FirstOrDefaultAsync(t => t.UserId == userId);

    if (twoFactorAuth == null)
      throw new InvalidOperationException("Two-factor authentication not set up");

    if (twoFactorAuth.IsEnabled)
      throw new InvalidOperationException("Two-factor authentication is already enabled");

    // Check if account is locked
    if (twoFactorAuth.LockedUntil.HasValue && twoFactorAuth.LockedUntil.Value > DateTime.UtcNow)
      throw new InvalidOperationException("Account is temporarily locked due to too many failed attempts");

    // Verify the code
    if (!VerifyTOTP(twoFactorAuth.SecretKey, code))
    {
      twoFactorAuth.FailedAttempts++;

      // Lock account after 5 failed attempts
      if (twoFactorAuth.FailedAttempts >= 5)
      {
        twoFactorAuth.LockedUntil = DateTime.UtcNow.AddMinutes(30);
      }

      await _context.SaveChangesAsync();

      await _auditService.LogAsync(
          userId,
          AuditAction.TwoFactorFailed,
          AuditEntityType.Security,
          userId,
          null,
          success: false,
          errorMessage: "Invalid verification code"
      );

      return false;
    }

    // Enable 2FA
    twoFactorAuth.IsEnabled = true;
    twoFactorAuth.EnabledAt = DateTime.UtcNow;
    twoFactorAuth.FailedAttempts = 0;
    twoFactorAuth.LockedUntil = null;

    await _context.SaveChangesAsync();

    await _auditService.LogAsync(
        userId,
        AuditAction.TwoFactorEnabled,
        AuditEntityType.Security,
        userId
    );

    return true;
  }

  public async Task<bool> DisableAsync(string userId, string code)
  {
    var twoFactorAuth = await _context.TwoFactorAuths
        .FirstOrDefaultAsync(t => t.UserId == userId);

    if (twoFactorAuth == null || !twoFactorAuth.IsEnabled)
      throw new InvalidOperationException("Two-factor authentication is not enabled");

    // Verify the code
    if (!VerifyTOTP(twoFactorAuth.SecretKey, code))
    {
      await _auditService.LogAsync(
          userId,
          AuditAction.TwoFactorFailed,
          AuditEntityType.Security,
          userId,
          null,
          success: false,
          errorMessage: "Invalid verification code for disable"
      );

      return false;
    }

    // Disable 2FA
    twoFactorAuth.IsEnabled = false;
    twoFactorAuth.EnabledAt = null;

    await _context.SaveChangesAsync();

    await _auditService.LogAsync(
        userId,
        AuditAction.TwoFactorDisabled,
        AuditEntityType.Security,
        userId
    );

    return true;
  }

  public async Task<VerifyTwoFactorResponse> VerifyAsync(string userId, string code)
  {
    var twoFactorAuth = await _context.TwoFactorAuths
        .FirstOrDefaultAsync(t => t.UserId == userId);

    if (twoFactorAuth == null || !twoFactorAuth.IsEnabled)
      return new VerifyTwoFactorResponse { Success = false, ErrorMessage = "Two-factor authentication not enabled" };

    // Check if account is locked
    if (twoFactorAuth.LockedUntil.HasValue && twoFactorAuth.LockedUntil.Value > DateTime.UtcNow)
      return new VerifyTwoFactorResponse { Success = false, ErrorMessage = "Account is temporarily locked" };

    // Verify the code
    if (!VerifyTOTP(twoFactorAuth.SecretKey, code))
    {
      twoFactorAuth.FailedAttempts++;
      twoFactorAuth.LastUsedAt = DateTime.UtcNow;

      // Lock account after 5 failed attempts
      if (twoFactorAuth.FailedAttempts >= 5)
      {
        twoFactorAuth.LockedUntil = DateTime.UtcNow.AddMinutes(30);
      }

      await _context.SaveChangesAsync();

      await _auditService.LogAsync(
          userId,
          AuditAction.TwoFactorFailed,
          AuditEntityType.Security,
          userId,
          null,
          success: false,
          errorMessage: "Invalid verification code"
      );

      return new VerifyTwoFactorResponse { Success = false, ErrorMessage = "Invalid verification code" };
    }

    // Reset failed attempts on successful verification
    twoFactorAuth.FailedAttempts = 0;
    twoFactorAuth.LockedUntil = null;
    twoFactorAuth.LastUsedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    await _auditService.LogAsync(
        userId,
        AuditAction.TwoFactorVerified,
        AuditEntityType.Security,
        userId
    );

    return new VerifyTwoFactorResponse { Success = true };
  }

  public async Task<GenerateBackupCodesResponse> GenerateBackupCodesAsync(string userId)
  {
    var twoFactorAuth = await _context.TwoFactorAuths
        .FirstOrDefaultAsync(t => t.UserId == userId);

    if (twoFactorAuth == null)
      throw new InvalidOperationException("Two-factor authentication not set up");

    // Remove existing backup codes
    var existingCodes = await _context.BackupCodes
        .Where(b => b.TwoFactorAuthId == twoFactorAuth.Id)
        .ToListAsync();

    _context.BackupCodes.RemoveRange(existingCodes);

    // Generate new backup codes
    var backupCodes = GenerateBackupCodes();

    // Store new backup codes
    foreach (var code in backupCodes)
    {
      var backupCode = new BackupCode
      {
        TwoFactorAuthId = twoFactorAuth.Id,
        Code = HashBackupCode(code)
      };
      _context.BackupCodes.Add(backupCode);
    }

    await _context.SaveChangesAsync();

    await _auditService.LogAsync(
        userId,
        AuditAction.SettingsChanged,
        AuditEntityType.Security,
        userId,
        null,
        additionalData: "Backup codes regenerated"
    );

    return new GenerateBackupCodesResponse
    {
      BackupCodes = backupCodes
    };
  }

  public async Task<bool> VerifyBackupCodeAsync(string userId, string code)
  {
    var twoFactorAuth = await _context.TwoFactorAuths
        .FirstOrDefaultAsync(t => t.UserId == userId);

    if (twoFactorAuth == null || !twoFactorAuth.IsEnabled)
      return false;

    var hashedCode = HashBackupCode(code);
    var backupCode = await _context.BackupCodes
        .FirstOrDefaultAsync(b => b.TwoFactorAuthId == twoFactorAuth.Id &&
                                b.Code == hashedCode &&
                                !b.IsUsed);

    if (backupCode == null)
      return false;

    // Mark backup code as used
    backupCode.IsUsed = true;
    backupCode.UsedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    await _auditService.LogAsync(
        userId,
        AuditAction.TwoFactorVerified,
        AuditEntityType.Security,
        userId,
        null,
        additionalData: "Backup code used"
    );

    return true;
  }

  public async Task<bool> IsEnabledAsync(string userId)
  {
    var twoFactorAuth = await _context.TwoFactorAuths
        .FirstOrDefaultAsync(t => t.UserId == userId);

    return twoFactorAuth?.IsEnabled ?? false;
  }

  private string GenerateSecretKey()
  {
    var random = new byte[20];
    using (var rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(random);
    }
    return Convert.ToBase64String(random);
  }

  private List<string> GenerateBackupCodes()
  {
    var codes = new List<string>();
    var random = new Random();

    for (int i = 0; i < 10; i++)
    {
      var code = random.Next(100000, 999999).ToString();
      codes.Add(code);
    }

    return codes;
  }

  private string HashBackupCode(string code)
  {
    using (var sha256 = SHA256.Create())
    {
      var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(code));
      return Convert.ToBase64String(hash);
    }
  }

  private string GenerateQrCodeUrl(string email, string secretKey, string issuer)
  {
    var otpauth = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}?secret={secretKey}&issuer={Uri.EscapeDataString(issuer)}";
    return $"https://chart.googleapis.com/chart?cht=qr&chs=200x200&chl={Uri.EscapeDataString(otpauth)}";
  }

  private bool VerifyTOTP(string secretKey, string code)
  {
    // This is a simplified TOTP verification
    // In a real implementation, you would use a proper TOTP library
    // For now, we'll use a simple time-based verification

    var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    var timeStep = 30; // 30 seconds per code
    var window = 1; // Allow 1 code before and after

    for (int i = -window; i <= window; i++)
    {
      var time = currentTime + (i * timeStep);
      var expectedCode = GenerateTOTP(secretKey, time);

      if (code == expectedCode)
        return true;
    }

    return false;
  }

  private string GenerateTOTP(string secretKey, long time)
  {
    // This is a simplified TOTP generation
    // In a real implementation, you would use a proper TOTP library
    using (var hmac = new HMACSHA1(Convert.FromBase64String(secretKey)))
    {
      var timeBytes = BitConverter.GetBytes(time);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(timeBytes);

      var hash = hmac.ComputeHash(timeBytes);
      var offset = hash[hash.Length - 1] & 0xf;

      var code = ((hash[offset] & 0x7f) << 24) |
                ((hash[offset + 1] & 0xff) << 16) |
                ((hash[offset + 2] & 0xff) << 8) |
                (hash[offset + 3] & 0xff);

      return (code % 1000000).ToString("D6");
    }
  }
}
