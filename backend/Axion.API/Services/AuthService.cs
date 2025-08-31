using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Axion.API.Data;
using Axion.API.Models;
using Axion.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Axion.API.Services;

public interface IAuthService
{
  Task<AuthResponse?> LoginAsync(LoginRequest request);
  Task<AuthResponse?> RegisterAsync(RegisterRequest request);
  string GenerateJwtToken(User user);
  string HashPassword(string password);
  bool VerifyPassword(string password, string hash);
}

public class AuthService : IAuthService
{
  private readonly AxionDbContext _context;
  private readonly IConfiguration _configuration;

  public AuthService(AxionDbContext context, IConfiguration configuration)
  {
    _context = context;
    _configuration = configuration;
  }

  public async Task<AuthResponse?> LoginAsync(LoginRequest request)
  {
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == request.Email);

    if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
    {
      return null;
    }

    var token = GenerateJwtToken(user);
    var userDto = MapToUserDto(user);

    return new AuthResponse
    {
      Token = token,
      User = userDto
    };
  }

  public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
  {
    // Check if user already exists
    if (await _context.Users.AnyAsync(u => u.Email == request.Email))
    {
      return null;
    }

    // Parse role
    if (!Enum.TryParse<UserRole>(request.Role, out var role))
    {
      role = UserRole.Employee;
    }

    var user = new User
    {
      Email = request.Email,
      PasswordHash = HashPassword(request.Password),
      FirstName = request.FirstName,
      LastName = request.LastName,
      Role = role
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    var token = GenerateJwtToken(user);
    var userDto = MapToUserDto(user);

    return new AuthResponse
    {
      Token = token,
      User = userDto
    };
  }

  public string GenerateJwtToken(User user)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "default-secret-key"));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddDays(7),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public string HashPassword(string password)
  {
    using var sha256 = SHA256.Create();
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
  }

  public bool VerifyPassword(string password, string hash)
  {
    var hashedPassword = HashPassword(password);
    return hashedPassword == hash;
  }

  private static UserDto MapToUserDto(User user)
  {
    return new UserDto
    {
      Id = user.Id,
      Email = user.Email,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Role = user.Role.ToString(),
      Avatar = user.Avatar,
      CreatedAt = user.CreatedAt,
      UpdatedAt = user.UpdatedAt
    };
  }
}

