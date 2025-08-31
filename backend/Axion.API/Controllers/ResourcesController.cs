using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axion.API.Data;
using Axion.API.Models;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
  private readonly AxionDbContext _context;

  public ResourcesController(AxionDbContext context)
  {
    _context = context;
  }

  // GET: api/resources
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Resource>>> GetResources()
  {
    return await _context.Resources
        .Include(r => r.Bookings)
        .ToListAsync();
  }

  // GET: api/resources/5
  [HttpGet("{id}")]
  public async Task<ActionResult<Resource>> GetResource(string id)
  {
    var resource = await _context.Resources
        .Include(r => r.Bookings)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (resource == null)
    {
      return NotFound();
    }

    return resource;
  }

  // GET: api/resources/available
  [HttpGet("available")]
  public async Task<ActionResult<IEnumerable<Resource>>> GetAvailableResources()
  {
    var now = DateTime.UtcNow;
    var resources = await _context.Resources
        .Include(r => r.Bookings)
        .ToListAsync();

    var availableResources = resources.Where(r =>
        r.IsAvailable &&
        !r.Bookings.Any(b =>
            now >= b.StartTime && now <= b.EndTime
        )
    );

    return availableResources.ToList();
  }

  // POST: api/resources
  [HttpPost]
  public async Task<ActionResult<Resource>> CreateResource([FromBody] CreateResourceRequest request)
  {
    var resource = new Resource
    {
      Name = request.Name,
      Type = request.Type,
      Capacity = request.Capacity,
      Location = request.Location,
      IsAvailable = request.IsAvailable
    };

    _context.Resources.Add(resource);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, resource);
  }

  // PUT: api/resources/5
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateResource(string id, [FromBody] UpdateResourceRequest request)
  {
    var resource = await _context.Resources.FindAsync(id);
    if (resource == null)
    {
      return NotFound();
    }

    resource.Name = request.Name ?? resource.Name;
    resource.Type = request.Type ?? resource.Type;
    resource.Capacity = request.Capacity ?? resource.Capacity;
    resource.Location = request.Location ?? resource.Location;
    resource.IsAvailable = request.IsAvailable ?? resource.IsAvailable;
    resource.UpdatedAt = DateTime.UtcNow;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!ResourceExists(id))
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

  // DELETE: api/resources/5
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteResource(string id)
  {
    var resource = await _context.Resources.FindAsync(id);
    if (resource == null)
    {
      return NotFound();
    }

    _context.Resources.Remove(resource);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private bool ResourceExists(string id)
  {
    return _context.Resources.Any(e => e.Id == id);
  }
}

// DTOs for Resource requests
public class CreateResourceRequest
{
  public string Name { get; set; } = string.Empty;
  public ResourceType Type { get; set; }
  public int? Capacity { get; set; }
  public string Location { get; set; } = string.Empty;
  public bool IsAvailable { get; set; } = true;
}

public class UpdateResourceRequest
{
  public string? Name { get; set; }
  public ResourceType? Type { get; set; }
  public int? Capacity { get; set; }
  public string? Location { get; set; }
  public bool? IsAvailable { get; set; }
}
