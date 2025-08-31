using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class User
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string PasswordHash { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  [StringLength(50)]
  public string LastName { get; set; } = string.Empty;

  [Required]
  public UserRole Role { get; set; } = UserRole.Employee;

  public string? Avatar { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public string? OrganizationId { get; set; }
  public virtual Organization? Organization { get; set; }
  public virtual ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
  public virtual ICollection<Project> AssignedProjects { get; set; } = new List<Project>();
  public virtual ICollection<ProjectTask> CreatedTasks { get; set; } = new List<ProjectTask>();
  public virtual ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
  public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
  public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public enum UserRole
{
  Employee,
  Manager,
  Admin
}
