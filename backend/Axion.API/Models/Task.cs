using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class ProjectTask
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  [StringLength(200)]
  public string Title { get; set; } = string.Empty;

  [StringLength(1000)]
  public string Description { get; set; } = string.Empty;

  [Required]
  public TaskStatus Status { get; set; } = TaskStatus.Todo;

  [Required]
  public TaskPriority Priority { get; set; } = TaskPriority.Medium;

  [Required]
  public string ProjectId { get; set; } = string.Empty;

  [Required]
  public string AssignedToId { get; set; } = string.Empty;

  [Required]
  public string CreatedById { get; set; } = string.Empty;

  public DateTime? DueDate { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual Project Project { get; set; } = null!;
  public virtual User AssignedTo { get; set; } = null!;
  public virtual User CreatedBy { get; set; } = null!;
  public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public enum TaskStatus
{
  Todo,
  InProgress,
  Review,
  Done
}

public enum TaskPriority
{
  Low,
  Medium,
  High
}
