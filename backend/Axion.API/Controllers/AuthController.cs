using Microsoft.AspNetCore.Mvc;
using Axion.API.Services;
using Axion.API.DTOs;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("login")]
  public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var response = await _authService.LoginAsync(request);

    if (response == null)
    {
      return Unauthorized(new { message = "Invalid email or password" });
    }

    return Ok(response);
  }

  [HttpPost("register")]
  public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var response = await _authService.RegisterAsync(request);

    if (response == null)
    {
      return BadRequest(new { message = "User with this email already exists" });
    }

    return CreatedAtAction(nameof(Login), response);
  }
}

