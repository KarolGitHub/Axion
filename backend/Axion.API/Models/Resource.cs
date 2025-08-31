using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class Resource
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required]
  [StringLength(100)]
  public string Name { get; set; } = string.Empty;

  [Required]
  public ResourceType Type { get; set; }

  public int? Capacity { get; set; }

  [Required]
  [StringLength(100)]
  public string Location { get; set; } = string.Empty;

  [Required]
  public bool IsAvailable { get; set; } = true;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public enum ResourceType
{
  MeetingRoom,
  Desk,
  Equipment
}

