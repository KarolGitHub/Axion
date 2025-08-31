using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class ChatRoom
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required, StringLength(100)]
  public string Name { get; set; } = string.Empty;

  [StringLength(500)]
  public string Description { get; set; } = string.Empty;

  [Required]
  public RoomType Type { get; set; } = RoomType.General;

  public string? ProjectId { get; set; }

  public string? OrganizationId { get; set; }

  public bool IsPrivate { get; set; } = false;

  public bool IsArchived { get; set; } = false;

  public int MaxParticipants { get; set; } = 100;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public virtual Project? Project { get; set; }
  public virtual Organization? Organization { get; set; }
  public virtual ICollection<User> Participants { get; set; } = new List<User>();
  public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}

public enum RoomType
{
  General,
  Project,
  Team,
  Direct,
  Announcements,
  Support
}
