using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axion.API.Data;
using Axion.API.Models;
using Axion.API.DTOs;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
  private readonly AxionDbContext _context;

  public ProjectsController(AxionDbContext context)
  {
    _context = context;
  }

  // GET: api/projects
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
  {
    return await _context.Projects
        .Include(p => p.CreatedBy)
        .Include(p => p.AssignedUsers)
        .Include(p => p.Tasks)
        .ToListAsync();
  }

  // GET: api/projects/5
  [HttpGet("{id}")]
  public async Task<ActionResult<Project>> GetProject(string id)
  {
    var project = await _context.Projects
        .Include(p => p.CreatedBy)
        .Include(p => p.AssignedUsers)
        .Include(p => p.Tasks)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (project == null)
    {
      return NotFound();
    }

    return project;
  }

  // POST: api/projects
  [HttpPost]
  public async Task<ActionResult<Project>> CreateProject([FromBody] CreateProjectRequest request)
  {
    var project = new Project
    {
      Name = request.Name,
      Description = request.Description,
      Status = request.Status,
      Priority = request.Priority,
      StartDate = request.StartDate,
      EndDate = request.EndDate,
      CreatedById = request.CreatedById
    };

    _context.Projects.Add(project);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
  }

  // PUT: api/projects/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateProject(string id, [FromBody] UpdateProjectRequest request)
  {
    var project = await _context.Projects.FindAsync(id);
    if (project == null)
    {
      return NotFound();
    }

    project.Name = request.Name ?? project.Name;
    project.Description = request.Description ?? project.Description;
    project.Status = request.Status ?? project.Status;
    project.Priority = request.Priority ?? project.Priority;
    project.StartDate = request.StartDate ?? project.StartDate;
    project.EndDate = request.EndDate;
    project.UpdatedAt = DateTime.UtcNow;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!ProjectExists(id))
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

  // DELETE: api/projects/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteProject(string id)
  {
    var project = await _context.Projects.FindAsync(id);
    if (project == null)
    {
      return NotFound();
    }

    _context.Projects.Remove(project);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private bool ProjectExists(string id)
  {
    return _context.Projects.Any(e => e.Id == id);
  }
}

// DTOs for Project requests
public class CreateProjectRequest
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public ProjectStatus Status { get; set; } = ProjectStatus.Active;
  public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;
  public DateTime StartDate { get; set; } = DateTime.UtcNow;
  public DateTime? EndDate { get; set; }
  public string CreatedById { get; set; } = string.Empty;
}

public class UpdateProjectRequest
{
  public string? Name { get; set; }
  public string? Description { get; set; }
  public ProjectStatus? Status { get; set; }
  public ProjectPriority? Priority { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
}
