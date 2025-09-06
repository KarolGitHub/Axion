using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Axion.Shared.Models;
using Axion.Shared.DTOs;

namespace Axion.UserService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class UsersController : ControllerBase
  {
    private readonly UserDbContext _context;

    public UsersController(UserDbContext context)
    {
      _context = context;
    }

    private int GetOrganizationId()
    {
      var organizationIdClaim = User.FindFirst("OrganizationId")?.Value;
      if (int.TryParse(organizationIdClaim, out int organizationId))
      {
        return organizationId;
      }
      throw new UnauthorizedAccessException("Organization ID not found in token");
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetUsers()
    {
      try
      {
        var organizationId = GetOrganizationId();
        var users = await _context.Users
            .Include(u => u.Organization)
            .Where(u => u.OrganizationId == organizationId)
            .Select(u => new UserResponse
            {
              Id = u.Id,
              FirstName = u.FirstName,
              LastName = u.LastName,
              Email = u.Email,
              Role = u.Role,
              IsActive = u.IsActive,
              CreatedAt = u.CreatedAt,
              UpdatedAt = u.UpdatedAt,
              OrganizationId = u.OrganizationId,
              OrganizationName = u.Organization.Name
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<UserResponse>>
        {
          Success = true,
          Data = users
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<UserResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUser(int id)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var user = await _context.Users
            .Include(u => u.Organization)
            .FirstOrDefaultAsync(u => u.Id == id && u.OrganizationId == organizationId);

        if (user == null)
        {
          return NotFound(new ApiResponse<UserResponse>
          {
            Success = false,
            Message = "User not found"
          });
        }

        var userResponse = new UserResponse
        {
          Id = user.Id,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Email = user.Email,
          Role = user.Role,
          IsActive = user.IsActive,
          CreatedAt = user.CreatedAt,
          UpdatedAt = user.UpdatedAt,
          OrganizationId = user.OrganizationId,
          OrganizationName = user.Organization.Name
        };

        return Ok(new ApiResponse<UserResponse>
        {
          Success = true,
          Data = userResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<UserResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser(CreateUserRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();

        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.OrganizationId == organizationId);

        if (existingUser != null)
        {
          return BadRequest(new ApiResponse<UserResponse>
          {
            Success = false,
            Message = "User with this email already exists"
          });
        }

        var user = new User
        {
          FirstName = request.FirstName,
          LastName = request.LastName,
          Email = request.Email,
          Role = request.Role,
          OrganizationId = organizationId,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userResponse = new UserResponse
        {
          Id = user.Id,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Email = user.Email,
          Role = user.Role,
          IsActive = user.IsActive,
          CreatedAt = user.CreatedAt,
          UpdatedAt = user.UpdatedAt,
          OrganizationId = user.OrganizationId,
          OrganizationName = (await _context.Organizations.FindAsync(organizationId))?.Name ?? ""
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new ApiResponse<UserResponse>
        {
          Success = true,
          Data = userResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<UserResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser(int id, UpdateUserRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.OrganizationId == organizationId);

        if (user == null)
        {
          return NotFound(new ApiResponse<UserResponse>
          {
            Success = false,
            Message = "User not found"
          });
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var userResponse = new UserResponse
        {
          Id = user.Id,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Email = user.Email,
          Role = user.Role,
          IsActive = user.IsActive,
          CreatedAt = user.CreatedAt,
          UpdatedAt = user.UpdatedAt,
          OrganizationId = user.OrganizationId,
          OrganizationName = (await _context.Organizations.FindAsync(organizationId))?.Name ?? ""
        };

        return Ok(new ApiResponse<UserResponse>
        {
          Success = true,
          Data = userResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<UserResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.OrganizationId == organizationId);

        if (user == null)
        {
          return NotFound(new ApiResponse<object>
          {
            Success = false,
            Message = "User not found"
          });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
          Success = true,
          Message = "User deleted successfully"
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<object>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("by-role/{role}")]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetUsersByRole(string role)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var users = await _context.Users
            .Include(u => u.Organization)
            .Where(u => u.OrganizationId == organizationId && u.Role == role)
            .Select(u => new UserResponse
            {
              Id = u.Id,
              FirstName = u.FirstName,
              LastName = u.LastName,
              Email = u.Email,
              Role = u.Role,
              IsActive = u.IsActive,
              CreatedAt = u.CreatedAt,
              UpdatedAt = u.UpdatedAt,
              OrganizationId = u.OrganizationId,
              OrganizationName = u.Organization.Name
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<UserResponse>>
        {
          Success = true,
          Data = users
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<UserResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }
  }
}
