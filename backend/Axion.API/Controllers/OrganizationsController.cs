using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axion.API.Data;
using Axion.API.Models;

namespace Axion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
  private readonly AxionDbContext _context;

  public OrganizationsController(AxionDbContext context)
  {
    _context = context;
  }

  // GET: api/organizations
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizations()
  {
    return await _context.Organizations
        .Include(o => o.Users)
        .Include(o => o.Projects)
        .Include(o => o.Resources)
        .ToListAsync();
  }

  // GET: api/organizations/{id}
  [HttpGet("{id}")]
  public async Task<ActionResult<Organization>> GetOrganization(string id)
  {
    var organization = await _context.Organizations
        .Include(o => o.Users)
        .Include(o => o.Projects)
        .Include(o => o.Resources)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (organization == null)
    {
      return NotFound();
    }

    return organization;
  }

  // GET: api/organizations/domain/{domain}
  [HttpGet("domain/{domain}")]
  public async Task<ActionResult<Organization>> GetOrganizationByDomain(string domain)
  {
    var organization = await _context.Organizations
        .Include(o => o.Users)
        .FirstOrDefaultAsync(o => o.Domain == domain);

    if (organization == null)
    {
      return NotFound();
    }

    return organization;
  }

  // POST: api/organizations
  [HttpPost]
  public async Task<ActionResult<Organization>> CreateOrganization([FromBody] CreateOrganizationRequest request)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    // Check if domain is already taken
    var existingOrg = await _context.Organizations
        .FirstOrDefaultAsync(o => o.Domain == request.Domain);

    if (existingOrg != null)
    {
      return BadRequest("Domain is already taken");
    }

    var organization = new Organization
    {
      Name = request.Name,
      Description = request.Description,
      Domain = request.Domain,
      Status = OrganizationStatus.Active,
      Plan = request.Plan,
      MaxUsers = GetMaxUsersForPlan(request.Plan),
      MaxProjects = GetMaxProjectsForPlan(request.Plan),
      MaxStorageGB = GetMaxStorageForPlan(request.Plan),
      SubscriptionEndDate = request.Plan == OrganizationPlan.Free ? null : DateTime.UtcNow.AddYears(1)
    };

    _context.Organizations.Add(organization);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetOrganization), new { id = organization.Id }, organization);
  }

  // PUT: api/organizations/{id}
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateOrganization(string id, [FromBody] UpdateOrganizationRequest request)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var organization = await _context.Organizations.FindAsync(id);
    if (organization == null)
    {
      return NotFound();
    }

    // Check if domain is already taken by another organization
    if (request.Domain != organization.Domain)
    {
      var existingOrg = await _context.Organizations
          .FirstOrDefaultAsync(o => o.Domain == request.Domain && o.Id != id);

      if (existingOrg != null)
      {
        return BadRequest("Domain is already taken");
      }
    }

    organization.Name = request.Name;
    organization.Description = request.Description;
    organization.Domain = request.Domain;
    organization.Status = request.Status;
    organization.Plan = request.Plan;
    organization.MaxUsers = GetMaxUsersForPlan(request.Plan);
    organization.MaxProjects = GetMaxProjectsForPlan(request.Plan);
    organization.MaxStorageGB = GetMaxStorageForPlan(request.Plan);
    organization.UpdatedAt = DateTime.UtcNow;

    if (request.Plan != OrganizationPlan.Free)
    {
      organization.SubscriptionEndDate = DateTime.UtcNow.AddYears(1);
    }

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!OrganizationExists(id))
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

  // DELETE: api/organizations/{id}
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteOrganization(string id)
  {
    var organization = await _context.Organizations.FindAsync(id);
    if (organization == null)
    {
      return NotFound();
    }

    // Soft delete - set status to cancelled
    organization.Status = OrganizationStatus.Cancelled;
    organization.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return NoContent();
  }

  // GET: api/organizations/{id}/stats
  [HttpGet("{id}/stats")]
  public async Task<ActionResult<OrganizationStats>> GetOrganizationStats(string id)
  {
    var organization = await _context.Organizations
        .Include(o => o.Users)
        .Include(o => o.Projects)
        .Include(o => o.Resources)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (organization == null)
    {
      return NotFound();
    }

    var stats = new OrganizationStats
    {
      OrganizationId = organization.Id,
      TotalUsers = organization.Users.Count,
      TotalProjects = organization.Projects.Count,
      TotalResources = organization.Resources.Count,
      MaxUsers = organization.MaxUsers,
      MaxProjects = organization.MaxProjects,
      MaxStorageGB = organization.MaxStorageGB,
      Plan = organization.Plan,
      Status = organization.Status,
      UsagePercentage = CalculateUsagePercentage(organization)
    };

    return stats;
  }

  // POST: api/organizations/{id}/upgrade
  [HttpPost("{id}/upgrade")]
  public async Task<IActionResult> UpgradeOrganization(string id, [FromBody] UpgradeOrganizationRequest request)
  {
    var organization = await _context.Organizations.FindAsync(id);
    if (organization == null)
    {
      return NotFound();
    }

    organization.Plan = request.NewPlan;
    organization.MaxUsers = GetMaxUsersForPlan(request.NewPlan);
    organization.MaxProjects = GetMaxProjectsForPlan(request.NewPlan);
    organization.MaxStorageGB = GetMaxStorageForPlan(request.NewPlan);
    organization.SubscriptionEndDate = DateTime.UtcNow.AddYears(1);
    organization.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return NoContent();
  }

  private bool OrganizationExists(string id)
  {
    return _context.Organizations.Any(e => e.Id == id);
  }

  private int GetMaxUsersForPlan(OrganizationPlan plan)
  {
    return plan switch
    {
      OrganizationPlan.Free => 10,
      OrganizationPlan.Basic => 25,
      OrganizationPlan.Professional => 100,
      OrganizationPlan.Enterprise => 1000,
      _ => 10
    };
  }

  private int GetMaxProjectsForPlan(OrganizationPlan plan)
  {
    return plan switch
    {
      OrganizationPlan.Free => 5,
      OrganizationPlan.Basic => 15,
      OrganizationPlan.Professional => 50,
      OrganizationPlan.Enterprise => 500,
      _ => 5
    };
  }

  private int GetMaxStorageForPlan(OrganizationPlan plan)
  {
    return plan switch
    {
      OrganizationPlan.Free => 1,
      OrganizationPlan.Basic => 10,
      OrganizationPlan.Professional => 100,
      OrganizationPlan.Enterprise => 1000,
      _ => 1
    };
  }

  private double CalculateUsagePercentage(Organization organization)
  {
    var userUsage = (double)organization.Users.Count / organization.MaxUsers;
    var projectUsage = (double)organization.Projects.Count / organization.MaxProjects;

    return Math.Max(userUsage, projectUsage) * 100;
  }
}

public class CreateOrganizationRequest
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Domain { get; set; } = string.Empty;
  public OrganizationPlan Plan { get; set; } = OrganizationPlan.Free;
}

public class UpdateOrganizationRequest
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Domain { get; set; } = string.Empty;
  public OrganizationStatus Status { get; set; }
  public OrganizationPlan Plan { get; set; }
}

public class UpgradeOrganizationRequest
{
  public OrganizationPlan NewPlan { get; set; }
}

public class OrganizationStats
{
  public string OrganizationId { get; set; } = string.Empty;
  public int TotalUsers { get; set; }
  public int TotalProjects { get; set; }
  public int TotalResources { get; set; }
  public int MaxUsers { get; set; }
  public int MaxProjects { get; set; }
  public int MaxStorageGB { get; set; }
  public OrganizationPlan Plan { get; set; }
  public OrganizationStatus Status { get; set; }
  public double UsagePercentage { get; set; }
}
