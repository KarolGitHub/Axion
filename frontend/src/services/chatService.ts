import { api } from '../utils/api';

export interface Message {
  id: string;
  content: string;
  senderId: string;
  senderName?: string;
  type:
    | 'Text'
    | 'Image'
    | 'File'
    | 'System'
    | 'TaskUpdate'
    | 'ProjectUpdate'
    | 'Mention';
  roomId?: string;
  projectId?: string;
  taskId?: string;
  replyToMessageId?: string;
  mentions: string[];
  attachments: string[];
  isEdited: boolean;
  createdAt: string;
  updatedAt?: string;
  replyToMessage?: Message;
  replies: Message[];
}

export interface ChatRoom {
  id: string;
  name: string;
  description: string;
  type: 'General' | 'Project' | 'Team' | 'Direct' | 'Announcements' | 'Support';
  projectId?: string;
  projectName?: string;
  isPrivate: boolean;
  isArchived: boolean;
  participantCount: number;
  messageCount: number;
  createdAt: string;
  updatedAt?: string;
  lastMessage?: Message;
  participants: User[];
}

export interface ChatRoomList {
  id: string;
  name: string;
  type: string;
  projectId?: string;
  projectName?: string;
  isPrivate: boolean;
  participantCount: number;
  unreadCount: number;
  lastMessageAt?: string;
  lastMessage?: Message;
}

export interface CreateMessageRequest {
  content: string;
  roomId?: string;
  projectId?: string;
  taskId?: string;
  replyToMessageId?: string;
  mentions: string[];
  attachments: string[];
}

export interface CreateChatRoomRequest {
  name: string;
  description: string;
  type: string;
  projectId?: string;
  isPrivate: boolean;
  participantIds: string[];
}

export interface UpdateChatRoomRequest {
  name: string;
  description: string;
  isPrivate: boolean;
  isArchived: boolean;
}

export interface ChatSearchRequest {
  query: string;
  roomId?: string;
  projectId?: string;
  fromDate?: string;
  toDate?: string;
  page: number;
  pageSize: number;
}

export interface ChatSearchResponse {
  messages: Message[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface TypingNotification {
  userId: string;
  userName: string;
  roomId: string;
  isTyping: boolean;
}

class ChatService {
  private connection: any = null;
  private messageHandlers: ((message: Message) => void)[] = [];
  private typingHandlers: ((notification: TypingNotification) => void)[] = [];
  private userStatusHandlers: ((userId: string, isOnline: boolean) => void)[] =
    [];
  private mentionHandlers: ((message: Message) => void)[] = [];

  // Initialize SignalR connection
  async initializeConnection(): Promise<void> {
    try {
      const { HubConnectionBuilder, LogLevel } = await import(
        '@microsoft/signalr'
      );

      this.connection = new HubConnectionBuilder()
        .withUrl(
          `${process.env.REACT_APP_API_URL?.replace('/api', '')}/chathub`,
          {
            accessTokenFactory: () => localStorage.getItem('token') || '',
          }
        )
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();

      this.setupEventHandlers();
      await this.connection.start();
      console.log('SignalR Connected');
    } catch (error) {
      console.error('SignalR Connection Error:', error);
    }
  }

  private setupEventHandlers(): void {
    if (!this.connection) return;

    this.connection.on('MessageReceived', (message: Message) => {
      this.messageHandlers.forEach((handler) => handler(message));
    });

    this.connection.on('MessageUpdated', (message: Message) => {
      this.messageHandlers.forEach((handler) => handler(message));
    });

    this.connection.on('MessageDeleted', (messageId: string) => {
      // Handle message deletion
      console.log('Message deleted:', messageId);
    });

    this.connection.on('TypingStarted', (notification: TypingNotification) => {
      this.typingHandlers.forEach((handler) => handler(notification));
    });

    this.connection.on('TypingStopped', (notification: TypingNotification) => {
      this.typingHandlers.forEach((handler) => handler(notification));
    });

    this.connection.on('UserConnected', (userId: string) => {
      this.userStatusHandlers.forEach((handler) => handler(userId, true));
    });

    this.connection.on('UserDisconnected', (userId: string) => {
      this.userStatusHandlers.forEach((handler) => handler(userId, false));
    });

    this.connection.on('MentionReceived', (message: Message) => {
      this.mentionHandlers.forEach((handler) => handler(message));
    });
  }

  // Event handlers
  onMessageReceived(handler: (message: Message) => void): void {
    this.messageHandlers.push(handler);
  }

  onTypingNotification(
    handler: (notification: TypingNotification) => void
  ): void {
    this.typingHandlers.push(handler);
  }

  onUserStatusChange(
    handler: (userId: string, isOnline: boolean) => void
  ): void {
    this.userStatusHandlers.push(handler);
  }

  onMentionReceived(handler: (message: Message) => void): void {
    this.mentionHandlers.push(handler);
  }

  // SignalR methods
  async joinRoom(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('JoinRoom', roomId);
    }
  }

  async leaveRoom(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('LeaveRoom', roomId);
    }
  }

  async sendMessage(request: CreateMessageRequest): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('SendMessage', request);
    }
  }

  async updateMessage(messageId: string, content: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('UpdateMessage', messageId, { content });
    }
  }

  async deleteMessage(messageId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('DeleteMessage', messageId);
    }
  }

  async startTyping(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('StartTyping', roomId);
    }
  }

  async stopTyping(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('StopTyping', roomId);
    }
  }

  async markAsRead(roomId: string): Promise<void> {
    if (this.connection) {
      await this.connection.invoke('MarkAsRead', roomId);
    }
  }

  // REST API methods
  async getChatRooms(): Promise<ChatRoomList[]> {
    const response = await api.get('/chat/rooms');
    return response.data;
  }

  async getChatRoom(roomId: string): Promise<ChatRoom> {
    const response = await api.get(`/chat/rooms/${roomId}`);
    return response.data;
  }

  async createChatRoom(request: CreateChatRoomRequest): Promise<ChatRoom> {
    const response = await api.post('/chat/rooms', request);
    return response.data;
  }

  async updateChatRoom(
    roomId: string,
    request: UpdateChatRoomRequest
  ): Promise<void> {
    await api.put(`/chat/rooms/${roomId}`, request);
  }

  async getRoomMessages(
    roomId: string,
    page: number = 1,
    pageSize: number = 50
  ): Promise<Message[]> {
    const response = await api.get(`/chat/rooms/${roomId}/messages`, {
      params: { page, pageSize },
    });
    return response.data;
  }

  async joinChatRoom(roomId: string): Promise<void> {
    await api.post(`/chat/rooms/${roomId}/join`);
  }

  async leaveChatRoom(roomId: string): Promise<void> {
    await api.post(`/chat/rooms/${roomId}/leave`);
  }

  async searchMessages(
    request: ChatSearchRequest
  ): Promise<ChatSearchResponse> {
    const response = await api.post('/chat/search', request);
    return response.data;
  }

  // Utility methods
  disconnect(): void {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }
  }

  isConnected(): boolean {
    return this.connection?.state === 'Connected';
  }

  // Parse mentions from message content
  parseMentions(content: string): string[] {
    const mentionRegex = /@(\w+)/g;
    const mentions: string[] = [];
    let match;

    while ((match = mentionRegex.exec(content)) !== null) {
      mentions.push(match[1]);
    }

    return mentions;
  }

  // Format message content with mentions
  formatMessageContent(content: string, mentions: string[]): string {
    let formattedContent = content;

    mentions.forEach((mention) => {
      const regex = new RegExp(`@${mention}`, 'g');
      formattedContent = formattedContent.replace(
        regex,
        `<span class="mention">@${mention}</span>`
      );
    });

    return formattedContent;
  }
}

export const chatService = new ChatService();
