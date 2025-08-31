using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class Organization
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required, StringLength(100)]
  public string Name { get; set; } = string.Empty;

  [StringLength(500)]
  public string Description { get; set; } = string.Empty;

  [Required, StringLength(100)]
  public string Domain { get; set; } = string.Empty;

  [Required]
  public OrganizationStatus Status { get; set; } = OrganizationStatus.Active;

  [Required]
  public OrganizationPlan Plan { get; set; } = OrganizationPlan.Free;

  public int MaxUsers { get; set; } = 10;
  public int MaxProjects { get; set; } = 5;
  public int MaxStorageGB { get; set; } = 1;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? SubscriptionEndDate { get; set; }

  // Navigation properties
  public virtual ICollection<User> Users { get; set; } = new List<User>();
  public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
  public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
}

public enum OrganizationStatus
{
  Active,
  Suspended,
  Cancelled
}

public enum OrganizationPlan
{
  Free,
  Basic,
  Professional,
  Enterprise
}
