using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axion.API.Data;
using Axion.API.Models;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
  private readonly AxionDbContext _context;

  public TasksController(AxionDbContext context)
  {
    _context = context;
  }

  // GET: api/tasks
  [HttpGet]
  public async Task<ActionResult<IEnumerable<ProjectTask>>> GetTasks()
  {
    return await _context.Tasks
        .Include(t => t.Project)
        .Include(t => t.AssignedTo)
        .Include(t => t.CreatedBy)
        .Include(t => t.Comments)
        .ToListAsync();
  }

  // GET: api/tasks/5
  [HttpGet("{id}")]
  public async Task<ActionResult<ProjectTask>> GetTask(string id)
  {
    var task = await _context.Tasks
        .Include(t => t.Project)
        .Include(t => t.AssignedTo)
        .Include(t => t.CreatedBy)
        .Include(t => t.Comments)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (task == null)
    {
      return NotFound();
    }

    return task;
  }

  // GET: api/tasks/project/5
  [HttpGet("project/{projectId}")]
  public async Task<ActionResult<IEnumerable<ProjectTask>>> GetTasksByProject(string projectId)
  {
    return await _context.Tasks
        .Include(t => t.Project)
        .Include(t => t.AssignedTo)
        .Include(t => t.CreatedBy)
        .Where(t => t.ProjectId == projectId)
        .ToListAsync();
  }

  // POST: api/tasks
  [HttpPost]
  public async Task<ActionResult<ProjectTask>> CreateTask([FromBody] CreateTaskRequest request)
  {
    var task = new ProjectTask
    {
      Title = request.Title,
      Description = request.Description,
      Status = request.Status,
      Priority = request.Priority,
      ProjectId = request.ProjectId,
      AssignedToId = request.AssignedToId,
      CreatedById = request.CreatedById,
      DueDate = request.DueDate
    };

    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
  }

  // PUT: api/tasks/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateTask(string id, [FromBody] UpdateTaskRequest request)
  {
    var task = await _context.Tasks.FindAsync(id);
    if (task == null)
    {
      return NotFound();
    }

    task.Title = request.Title ?? task.Title;
    task.Description = request.Description ?? task.Description;
    task.Status = request.Status ?? task.Status;
    task.Priority = request.Priority ?? task.Priority;
    task.ProjectId = request.ProjectId ?? task.ProjectId;
    task.AssignedToId = request.AssignedToId ?? task.AssignedToId;
    task.DueDate = request.DueDate;
    task.UpdatedAt = DateTime.UtcNow;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!TaskExists(id))
      {
        return NotFound();
      }
      else
      {
        throw;
      }
    }

    return NoContent();
  }

  // PATCH: api/tasks/5/status
  [HttpPatch("{id}/status")]
  public async Task<IActionResult> UpdateTaskStatus(string id, [FromBody] UpdateTaskStatusRequest request)
  {
    var task = await _context.Tasks.FindAsync(id);
    if (task == null)
    {
      return NotFound();
    }

    task.Status = request.Status;
    task.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return NoContent();
  }

  // DELETE: api/tasks/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteTask(string id)
  {
    var task = await _context.Tasks.FindAsync(id);
    if (task == null)
    {
      return NotFound();
    }

    _context.Tasks.Remove(task);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private bool TaskExists(string id)
  {
    return _context.Tasks.Any(e => e.Id == id);
  }
}

// DTOs for Task requests
public class CreateTaskRequest
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskStatus Status { get; set; } = TaskStatus.Todo;
  public TaskPriority Priority { get; set; } = TaskPriority.Medium;
  public string ProjectId { get; set; } = string.Empty;
  public string AssignedToId { get; set; } = string.Empty;
  public string CreatedById { get; set; } = string.Empty;
  public DateTime? DueDate { get; set; }
}

public class UpdateTaskRequest
{
  public string? Title { get; set; }
  public string? Description { get; set; }
  public TaskStatus? Status { get; set; }
  public TaskPriority? Priority { get; set; }
  public string? ProjectId { get; set; }
  public string? AssignedToId { get; set; }
  public DateTime? DueDate { get; set; }
}

public class UpdateTaskStatusRequest
{
  public TaskStatus Status { get; set; }
}
