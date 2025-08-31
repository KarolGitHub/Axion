using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class Booking
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  public string ResourceId { get; set; } = string.Empty;

  [Required]
  public string UserId { get; set; } = string.Empty;

  [Required]
  public DateTime StartTime { get; set; }

  [Required]
  public DateTime EndTime { get; set; }

  [Required]
  [StringLength(200)]
  public string Purpose { get; set; } = string.Empty;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual Resource Resource { get; set; } = null!;
  public virtual User User { get; set; } = null!;
}

