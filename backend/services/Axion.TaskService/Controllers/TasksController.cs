using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Axion.Shared.Models;
using Axion.Shared.DTOs;

namespace Axion.TaskService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class TasksController : ControllerBase
  {
    private readonly TaskDbContext _context;

    public TasksController(TaskDbContext context)
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
    public async Task<ActionResult<ApiResponse<List<TaskResponse>>>> GetTasks()
    {
      try
      {
        var organizationId = GetOrganizationId();
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .Where(t => t.Project.OrganizationId == organizationId)
            .Select(t => new TaskResponse
            {
              Id = t.Id,
              Title = t.Title,
              Description = t.Description,
              Status = t.Status,
              Priority = t.Priority,
              DueDate = t.DueDate,
              EstimatedHours = t.EstimatedHours,
              ActualHours = t.ActualHours,
              CreatedAt = t.CreatedAt,
              UpdatedAt = t.UpdatedAt,
              ProjectId = t.ProjectId,
              ProjectName = t.Project.Name,
              AssignedTo = t.AssignedTo,
              AssignedToName = t.AssignedToUser.FirstName + " " + t.AssignedToUser.LastName,
              CreatedBy = t.CreatedBy,
              CreatedByName = t.CreatedByUser.FirstName + " " + t.CreatedByUser.LastName
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<TaskResponse>>
        {
          Success = true,
          Data = tasks
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<TaskResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> GetTask(int id)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var task = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.OrganizationId == organizationId);

        if (task == null)
        {
          return NotFound(new ApiResponse<TaskResponse>
          {
            Success = false,
            Message = "Task not found"
          });
        }

        var taskResponse = new TaskResponse
        {
          Id = task.Id,
          Title = task.Title,
          Description = task.Description,
          Status = task.Status,
          Priority = task.Priority,
          DueDate = task.DueDate,
          EstimatedHours = task.EstimatedHours,
          ActualHours = task.ActualHours,
          CreatedAt = task.CreatedAt,
          UpdatedAt = task.UpdatedAt,
          ProjectId = task.ProjectId,
          ProjectName = task.Project.Name,
          AssignedTo = task.AssignedTo,
          AssignedToName = task.AssignedToUser.FirstName + " " + task.AssignedToUser.LastName,
          CreatedBy = task.CreatedBy,
          CreatedByName = task.CreatedByUser.FirstName + " " + task.CreatedByUser.LastName
        };

        return Ok(new ApiResponse<TaskResponse>
        {
          Success = true,
          Data = taskResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<TaskResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> CreateTask(CreateTaskRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var userId = GetUserId();

        // Verify project exists and belongs to organization
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.OrganizationId == organizationId);

        if (project == null)
        {
          return BadRequest(new ApiResponse<TaskResponse>
          {
            Success = false,
            Message = "Project not found"
          });
        }

        var task = new ProjectTask
        {
          Title = request.Title,
          Description = request.Description,
          Status = "Pending",
          Priority = request.Priority,
          DueDate = request.DueDate,
          EstimatedHours = request.EstimatedHours,
          ProjectId = request.ProjectId,
          AssignedTo = request.AssignedTo,
          CreatedBy = userId,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var taskResponse = new TaskResponse
        {
          Id = task.Id,
          Title = task.Title,
          Description = task.Description,
          Status = task.Status,
          Priority = task.Priority,
          DueDate = task.DueDate,
          EstimatedHours = task.EstimatedHours,
          ActualHours = task.ActualHours,
          CreatedAt = task.CreatedAt,
          UpdatedAt = task.UpdatedAt,
          ProjectId = task.ProjectId,
          ProjectName = project.Name,
          AssignedTo = task.AssignedTo,
          AssignedToName = "", // Will be populated by user service
          CreatedBy = task.CreatedBy,
          CreatedByName = "" // Will be populated by user service
        };

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new ApiResponse<TaskResponse>
        {
          Success = true,
          Data = taskResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<TaskResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> UpdateTask(int id, UpdateTaskRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.OrganizationId == organizationId);

        if (task == null)
        {
          return NotFound(new ApiResponse<TaskResponse>
          {
            Success = false,
            Message = "Task not found"
          });
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.EstimatedHours = request.EstimatedHours;
        task.ActualHours = request.ActualHours;
        task.AssignedTo = request.AssignedTo;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var taskResponse = new TaskResponse
        {
          Id = task.Id,
          Title = task.Title,
          Description = task.Description,
          Status = task.Status,
          Priority = task.Priority,
          DueDate = task.DueDate,
          EstimatedHours = task.EstimatedHours,
          ActualHours = task.ActualHours,
          CreatedAt = task.CreatedAt,
          UpdatedAt = task.UpdatedAt,
          ProjectId = task.ProjectId,
          ProjectName = task.Project.Name,
          AssignedTo = task.AssignedTo,
          AssignedToName = "", // Will be populated by user service
          CreatedBy = task.CreatedBy,
          CreatedByName = "" // Will be populated by user service
        };

        return Ok(new ApiResponse<TaskResponse>
        {
          Success = true,
          Data = taskResponse
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<TaskResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTask(int id)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id && t.Project.OrganizationId == organizationId);

        if (task == null)
        {
          return NotFound(new ApiResponse<object>
          {
            Success = false,
            Message = "Task not found"
          });
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
          Success = true,
          Message = "Task deleted successfully"
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

    [HttpGet("by-project/{projectId}")]
    public async Task<ActionResult<ApiResponse<List<TaskResponse>>>> GetTasksByProject(int projectId)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .Where(t => t.ProjectId == projectId && t.Project.OrganizationId == organizationId)
            .Select(t => new TaskResponse
            {
              Id = t.Id,
              Title = t.Title,
              Description = t.Description,
              Status = t.Status,
              Priority = t.Priority,
              DueDate = t.DueDate,
              EstimatedHours = t.EstimatedHours,
              ActualHours = t.ActualHours,
              CreatedAt = t.CreatedAt,
              UpdatedAt = t.UpdatedAt,
              ProjectId = t.ProjectId,
              ProjectName = t.Project.Name,
              AssignedTo = t.AssignedTo,
              AssignedToName = t.AssignedToUser.FirstName + " " + t.AssignedToUser.LastName,
              CreatedBy = t.CreatedBy,
              CreatedByName = t.CreatedByUser.FirstName + " " + t.CreatedByUser.LastName
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<TaskResponse>>
        {
          Success = true,
          Data = tasks
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<TaskResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<TaskResponse>>>> GetTasksByUser(int userId)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .Where(t => t.AssignedTo == userId && t.Project.OrganizationId == organizationId)
            .Select(t => new TaskResponse
            {
              Id = t.Id,
              Title = t.Title,
              Description = t.Description,
              Status = t.Status,
              Priority = t.Priority,
              DueDate = t.DueDate,
              EstimatedHours = t.EstimatedHours,
              ActualHours = t.ActualHours,
              CreatedAt = t.CreatedAt,
              UpdatedAt = t.UpdatedAt,
              ProjectId = t.ProjectId,
              ProjectName = t.Project.Name,
              AssignedTo = t.AssignedTo,
              AssignedToName = t.AssignedToUser.FirstName + " " + t.AssignedToUser.LastName,
              CreatedBy = t.CreatedBy,
              CreatedByName = t.CreatedByUser.FirstName + " " + t.CreatedByUser.LastName
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<TaskResponse>>
        {
          Success = true,
          Data = tasks
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new ApiResponse<List<TaskResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }
  }
}
