using System.ComponentModel.DataAnnotations;

namespace Axion.Shared.Models
{
  public class User
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int OrganizationId { get; set; }

    // Navigation properties
    public virtual Organization Organization { get; set; } = null!;
  }

  public class Organization
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

    [MaxLength(50)]
    public string? Size { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
  }

  public class Project
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal Budget { get; set; } = 0;

    [MaxLength(50)]
    public string? Priority { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int OrganizationId { get; set; }

    public int CreatedBy { get; set; }

    // Navigation properties
    public virtual Organization Organization { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
  }

  public class ProjectTask
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public decimal EstimatedHours { get; set; } = 0;

    public decimal ActualHours { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int ProjectId { get; set; }

    public int AssignedTo { get; set; }

    public int CreatedBy { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual User AssignedToUser { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
  }

  public class Notification
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Message { get; set; }

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    public int OrganizationId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Organization Organization { get; set; } = null!;
  }
}
