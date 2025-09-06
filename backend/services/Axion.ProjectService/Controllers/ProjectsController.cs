using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Axion.Shared.Models;
using Axion.Shared.DTOs;

namespace Axion.ProjectService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class ProjectsController : ControllerBase
  {
    private readonly ProjectDbContext _context;

    public ProjectsController(ProjectDbContext context)
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

    private int GetUserId()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdClaim, out int userId))
      {
        return userId;
      }
      throw new UnauthorizedAccessException("User ID not found in token");
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProjectResponse>>>> GetProjects()
    {
      try
      {
        var organizationId = GetOrganizationId();
        var projects = await _context.Projects
            .Include(p => p.CreatedByUser)
            .Where(p => p.OrganizationId == organizationId)
            .Select(p => new ProjectResponse
            {
              Id = p.Id,
              Name = p.Name,
              Description = p.Description,
              Status = p.Status,
              StartDate = p.StartDate,
              EndDate = p.EndDate,
              Budget = p.Budget,
              Priority = p.Priority,
              CreatedAt = p.CreatedAt,
              UpdatedAt = p.UpdatedAt,
              OrganizationId = p.OrganizationId,
              CreatedBy = p.CreatedBy,
              CreatedByName = p.CreatedByUser.FirstName + " " + p.CreatedByUser.LastName,
              TaskCount = _context.Tasks.Count(t => t.ProjectId == p.Id),
              CompletedTaskCount = _context.Tasks.Count(t => t.ProjectId == p.Id && t.Status == "Completed")
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<ProjectResponse>>
        {
          Success = true,
          Data = projects
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<ProjectResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> GetProject(int id)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var project = await _context.Projects
            .Include(p => p.CreatedByUser)
            .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == organizationId);

        if (project == null)
        {
          return NotFound(new ApiResponse<ProjectResponse>
          {
            Success = false,
            Message = "Project not found"
          });
        }

        var projectResponse = new ProjectResponse
        {
          Id = project.Id,
          Name = project.Name,
          Description = project.Description,
          Status = project.Status,
          StartDate = project.StartDate,
          EndDate = project.EndDate,
          Budget = project.Budget,
          Priority = project.Priority,
          CreatedAt = project.CreatedAt,
          UpdatedAt = project.UpdatedAt,
          OrganizationId = project.OrganizationId,
          CreatedBy = project.CreatedBy,
          CreatedByName = project.CreatedByUser.FirstName + " " + project.CreatedByUser.LastName,
          TaskCount = await _context.Tasks.CountAsync(t => t.ProjectId == project.Id),
          CompletedTaskCount = await _context.Tasks.CountAsync(t => t.ProjectId == project.Id && t.Status == "Completed")
        };

        return Ok(new ApiResponse<ProjectResponse>
        {
          Success = true,
          Data = projectResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<ProjectResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> CreateProject(CreateProjectRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var userId = GetUserId();

        var project = new Project
        {
          Name = request.Name,
          Description = request.Description,
          Status = "Active",
          StartDate = request.StartDate,
          EndDate = request.EndDate,
          Budget = request.Budget,
          Priority = request.Priority,
          OrganizationId = organizationId,
          CreatedBy = userId,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var projectResponse = new ProjectResponse
        {
          Id = project.Id,
          Name = project.Name,
          Description = project.Description,
          Status = project.Status,
          StartDate = project.StartDate,
          EndDate = project.EndDate,
          Budget = project.Budget,
          Priority = project.Priority,
          CreatedAt = project.CreatedAt,
          UpdatedAt = project.UpdatedAt,
          OrganizationId = project.OrganizationId,
          CreatedBy = project.CreatedBy,
          CreatedByName = "", // Will be populated by the user service
          TaskCount = 0,
          CompletedTaskCount = 0
        };

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, new ApiResponse<ProjectResponse>
        {
          Success = true,
          Data = projectResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<ProjectResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> UpdateProject(int id, UpdateProjectRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == organizationId);

        if (project == null)
        {
          return NotFound(new ApiResponse<ProjectResponse>
          {
            Success = false,
            Message = "Project not found"
          });
        }

        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = request.Status;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.Budget = request.Budget;
        project.Priority = request.Priority;
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var projectResponse = new ProjectResponse
        {
          Id = project.Id,
          Name = project.Name,
          Description = project.Description,
          Status = project.Status,
          StartDate = project.StartDate,
          EndDate = project.EndDate,
          Budget = project.Budget,
          Priority = project.Priority,
          CreatedAt = project.CreatedAt,
          UpdatedAt = project.UpdatedAt,
          OrganizationId = project.OrganizationId,
          CreatedBy = project.CreatedBy,
          CreatedByName = "", // Will be populated by the user service
          TaskCount = await _context.Tasks.CountAsync(t => t.ProjectId == project.Id),
          CompletedTaskCount = await _context.Tasks.CountAsync(t => t.ProjectId == project.Id && t.Status == "Completed")
        };

        return Ok(new ApiResponse<ProjectResponse>
        {
          Success = true,
          Data = projectResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<ProjectResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProject(int id)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == organizationId);

        if (project == null)
        {
          return NotFound(new ApiResponse<object>
          {
            Success = false,
            Message = "Project not found"
          });
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
          Success = true,
          Message = "Project deleted successfully"
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

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<ApiResponse<List<ProjectResponse>>>> GetProjectsByStatus(string status)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var projects = await _context.Projects
            .Include(p => p.CreatedByUser)
            .Where(p => p.OrganizationId == organizationId && p.Status == status)
            .Select(p => new ProjectResponse
            {
              Id = p.Id,
              Name = p.Name,
              Description = p.Description,
              Status = p.Status,
              StartDate = p.StartDate,
              EndDate = p.EndDate,
              Budget = p.Budget,
              Priority = p.Priority,
              CreatedAt = p.CreatedAt,
              UpdatedAt = p.UpdatedAt,
              OrganizationId = p.OrganizationId,
              CreatedBy = p.CreatedBy,
              CreatedByName = p.CreatedByUser.FirstName + " " + p.CreatedByUser.LastName,
              TaskCount = _context.Tasks.Count(t => t.ProjectId == p.Id),
              CompletedTaskCount = _context.Tasks.Count(t => t.ProjectId == p.Id && t.Status == "Completed")
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<ProjectResponse>>
        {
          Success = true,
          Data = projects
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<ProjectResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }
  }
}
