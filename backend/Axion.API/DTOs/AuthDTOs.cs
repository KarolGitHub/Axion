using System.ComponentModel.DataAnnotations;

namespace Axion.API.DTOs;

public class LoginRequest
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  public string Password { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
  public string LastName { get; set; } = string.Empty;

  [Required]
  public string Role { get; set; } = "Employee";
}

public class AuthResponse
{
  public string Token { get; set; } = string.Empty;
  public UserDto User { get; set; } = new();
}

public class UserDto
{
  public string Id { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public string? Avatar { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}

