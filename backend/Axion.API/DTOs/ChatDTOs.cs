namespace Axion.API.DTOs;

public class CreateMessageRequest
{
  public string Content { get; set; } = string.Empty;
  public string? RoomId { get; set; }
  public string? ProjectId { get; set; }
  public string? TaskId { get; set; }
  public string? ReplyToMessageId { get; set; }
  public List<string> Mentions { get; set; } = new List<string>();
  public List<string> Attachments { get; set; } = new List<string>();
}

public class UpdateMessageRequest
{
  public string Content { get; set; } = string.Empty;
}

public class MessageResponse
{
  public string Id { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public string SenderId { get; set; } = string.Empty;
  public string? SenderName { get; set; }
  public string? SenderAvatar { get; set; }
  public string Type { get; set; } = string.Empty;
  public string? RoomId { get; set; }
  public string? ProjectId { get; set; }
  public string? TaskId { get; set; }
  public string? ReplyToMessageId { get; set; }
  public List<string> Mentions { get; set; } = new List<string>();
  public List<string> Attachments { get; set; } = new List<string>();
  public bool IsEdited { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public MessageResponse? ReplyToMessage { get; set; }
  public List<MessageResponse> Replies { get; set; } = new List<MessageResponse>();
}

public class CreateChatRoomRequest
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public string? ProjectId { get; set; }
  public bool IsPrivate { get; set; } = false;
  public List<string> ParticipantIds { get; set; } = new List<string>();
}

public class UpdateChatRoomRequest
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public bool IsPrivate { get; set; }
  public bool IsArchived { get; set; }
}

public class ChatRoomResponse
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public string? ProjectId { get; set; }
  public string? ProjectName { get; set; }
  public bool IsPrivate { get; set; }
  public bool IsArchived { get; set; }
  public int ParticipantCount { get; set; }
  public int MessageCount { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public MessageResponse? LastMessage { get; set; }
  public List<UserResponse> Participants { get; set; } = new List<UserResponse>();
}

public class ChatRoomListResponse
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public string? ProjectId { get; set; }
  public string? ProjectName { get; set; }
  public bool IsPrivate { get; set; }
  public int ParticipantCount { get; set; }
  public int UnreadCount { get; set; }
  public DateTime? LastMessageAt { get; set; }
  public MessageResponse? LastMessage { get; set; }
}

public class JoinRoomRequest
{
  public string RoomId { get; set; } = string.Empty;
}

public class LeaveRoomRequest
{
  public string RoomId { get; set; } = string.Empty;
}

public class TypingNotification
{
  public string UserId { get; set; } = string.Empty;
  public string UserName { get; set; } = string.Empty;
  public string RoomId { get; set; } = string.Empty;
  public bool IsTyping { get; set; }
}

public class ChatSearchRequest
{
  public string Query { get; set; } = string.Empty;
  public string? RoomId { get; set; }
  public string? ProjectId { get; set; }
  public DateTime? FromDate { get; set; }
  public DateTime? ToDate { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 20;
}

public class ChatSearchResponse
{
  public List<MessageResponse> Messages { get; set; } = new List<MessageResponse>();
  public int TotalCount { get; set; }
  public int Page { get; set; }
  public int PageSize { get; set; }
  public int TotalPages { get; set; }
}
