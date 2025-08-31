using System.ComponentModel.DataAnnotations;

namespace Axion.API.Models;

public class Message
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  [Required, StringLength(1000)]
  public string Content { get; set; } = string.Empty;

  [Required]
  public string SenderId { get; set; } = string.Empty;

  public string? SenderName { get; set; }

  public string? SenderAvatar { get; set; }

  [Required]
  public MessageType Type { get; set; } = MessageType.Text;

  public string? RoomId { get; set; }

  public string? ProjectId { get; set; }

  public string? TaskId { get; set; }

  public string? ReplyToMessageId { get; set; }

  public List<string> Mentions { get; set; } = new List<string>();

  public List<string> Attachments { get; set; } = new List<string>();

  public bool IsEdited { get; set; } = false;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  public DateTime? DeletedAt { get; set; }

  // Navigation properties
  public virtual User Sender { get; set; } = null!;
  public virtual Project? Project { get; set; }
  public virtual ProjectTask? Task { get; set; }
  public virtual Message? ReplyToMessage { get; set; }
  public virtual ICollection<Message> Replies { get; set; } = new List<Message>();
}

public enum MessageType
{
  Text,
  Image,
  File,
  System,
  TaskUpdate,
  ProjectUpdate,
  Mention
}
