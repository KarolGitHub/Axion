using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class Comment
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  [StringLength(1000)]
  public string Content { get; set; } = string.Empty;

  [Required]
  public string TaskId { get; set; } = string.Empty;

  [Required]
  public string UserId { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual ProjectTask Task { get; set; } = null!;
  public virtual User User { get; set; } = null!;
}
