using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class Project
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  [StringLength(100)]
  public string Name { get; set; } = string.Empty;

  [StringLength(500)]
  public string Description { get; set; } = string.Empty;

  [Required]
  public ProjectStatus Status { get; set; } = ProjectStatus.Active;

  [Required]
  public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

  [Required]
  public DateTime StartDate { get; set; } = DateTime.UtcNow;

  public DateTime? EndDate { get; set; }

  [Required]
  public string CreatedById { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual User CreatedBy { get; set; } = null!;
  public virtual ICollection<User> AssignedUsers { get; set; } = new List<User>();
  public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}

public enum ProjectStatus
{
  Active,
  Completed,
  OnHold
}

public enum ProjectPriority
{
  Low,
  Medium,
  High
}
