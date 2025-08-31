import React, { useState, useEffect, useRef } from 'react';
import {
  ChatBubbleLeftIcon,
  PaperAirplaneIcon,
  EllipsisHorizontalIcon,
  UserIcon,
  MagnifyingGlassIcon,
  PlusIcon,
  XMarkIcon,
} from '@heroicons/react/24/outline';
import {
  chatService,
  Message,
  ChatRoom,
  ChatRoomList,
  TypingNotification,
} from '../services/chatService';
import { useAuthStore } from '../state/authStore';

interface ChatProps {
  isOpen: boolean;
  onClose: () => void;
}

export const Chat: React.FC<ChatProps> = ({ isOpen, onClose }) => {
  const [rooms, setRooms] = useState<ChatRoomList[]>([]);
  const [selectedRoom, setSelectedRoom] = useState<ChatRoom | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [isTyping, setIsTyping] = useState(false);
  const [typingUsers, setTypingUsers] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [showCreateRoom, setShowCreateRoom] = useState(false);

  const messagesEndRef = useRef<HTMLDivElement>(null);
  const typingTimeoutRef = useRef<NodeJS.Timeout>();
  const { user } = useAuthStore();

  useEffect(() => {
    if (isOpen) {
      initializeChat();
    }
  }, [isOpen]);

  useEffect(() => {
    if (selectedRoom) {
      loadRoomMessages();
      joinRoom();
    }
  }, [selectedRoom]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const initializeChat = async () => {
    setLoading(true);
    try {
      await chatService.initializeConnection();
      await loadChatRooms();

      // Set up event handlers
      chatService.onMessageReceived(handleNewMessage);
      chatService.onTypingNotification(handleTypingNotification);
      chatService.onMentionReceived(handleMentionReceived);
    } catch (error) {
      console.error('Failed to initialize chat:', error);
    } finally {
      setLoading(false);
    }
  };

  const loadChatRooms = async () => {
    try {
      const chatRooms = await chatService.getChatRooms();
      setRooms(chatRooms);
    } catch (error) {
      console.error('Failed to load chat rooms:', error);
    }
  };

  const loadRoomMessages = async () => {
    if (!selectedRoom) return;

    try {
      const roomMessages = await chatService.getRoomMessages(selectedRoom.id);
      setMessages(roomMessages.reverse()); // Show newest at bottom
    } catch (error) {
      console.error('Failed to load room messages:', error);
    }
  };

  const joinRoom = async () => {
    if (!selectedRoom) return;

    try {
      await chatService.joinRoom(selectedRoom.id);
    } catch (error) {
      console.error('Failed to join room:', error);
    }
  };

  const handleNewMessage = (message: Message) => {
    if (message.roomId === selectedRoom?.id) {
      setMessages((prev) => [...prev, message]);
    }
  };

  const handleTypingNotification = (notification: TypingNotification) => {
    if (notification.roomId === selectedRoom?.id) {
      if (notification.isTyping) {
        setTypingUsers((prev) => [
          ...prev.filter((u) => u !== notification.userName),
          notification.userName,
        ]);
      } else {
        setTypingUsers((prev) =>
          prev.filter((u) => u !== notification.userName)
        );
      }
    }
  };

  const handleMentionReceived = (message: Message) => {
    // Show notification for mentions
    if (message.senderId !== user?.id) {
      // You could show a toast notification here
      console.log('You were mentioned:', message.content);
    }
  };

  const handleSendMessage = async () => {
    if (!newMessage.trim() || !selectedRoom) return;

    const mentions = chatService.parseMentions(newMessage);

    try {
      await chatService.sendMessage({
        content: newMessage,
        roomId: selectedRoom.id,
        mentions,
        attachments: [],
      });
      setNewMessage('');
      setIsTyping(false);
    } catch (error) {
      console.error('Failed to send message:', error);
    }
  };

  const handleTyping = () => {
    if (!selectedRoom) return;

    if (!isTyping) {
      setIsTyping(true);
      chatService.startTyping(selectedRoom.id);
    }

    // Clear existing timeout
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    // Set new timeout
    typingTimeoutRef.current = setTimeout(() => {
      setIsTyping(false);
      chatService.stopTyping(selectedRoom.id);
    }, 2000);
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const formatMessageTime = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };

  const isOwnMessage = (message: Message) => message.senderId === user?.id;

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 z-50 flex'>
      {/* Backdrop */}
      <div
        className='absolute inset-0 bg-black bg-opacity-50'
        onClick={onClose}
      />

      {/* Chat Container */}
      <div className='relative w-full max-w-4xl mx-auto bg-white dark:bg-gray-800 rounded-lg shadow-xl flex flex-col h-[90vh] my-auto'>
        {/* Header */}
        <div className='flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700'>
          <div className='flex items-center space-x-3'>
            <ChatBubbleLeftIcon className='h-6 w-6 text-blue-500' />
            <h2 className='text-lg font-semibold text-gray-900 dark:text-white'>
              Team Chat
            </h2>
          </div>
          <div className='flex items-center space-x-2'>
            <button
              onClick={() => setShowCreateRoom(true)}
              className='p-2 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'
            >
              <PlusIcon className='h-5 w-5' />
            </button>
            <button
              onClick={onClose}
              className='p-2 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200'
            >
              <XMarkIcon className='h-5 w-5' />
            </button>
          </div>
        </div>

        <div className='flex flex-1 overflow-hidden'>
          {/* Sidebar - Chat Rooms */}
          <div className='w-80 border-r border-gray-200 dark:border-gray-700 flex flex-col'>
            {/* Search */}
            <div className='p-4 border-b border-gray-200 dark:border-gray-700'>
              <div className='relative'>
                <MagnifyingGlassIcon className='absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400' />
                <input
                  type='text'
                  placeholder='Search rooms...'
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className='w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-white'
                />
              </div>
            </div>

            {/* Room List */}
            <div className='flex-1 overflow-y-auto'>
              {loading ? (
                <div className='p-4 text-center text-gray-500'>Loading...</div>
              ) : (
                rooms
                  .filter(
                    (room) =>
                      room.name
                        .toLowerCase()
                        .includes(searchQuery.toLowerCase()) ||
                      room.projectName
                        ?.toLowerCase()
                        .includes(searchQuery.toLowerCase())
                  )
                  .map((room) => (
                    <div
                      key={room.id}
                      onClick={() => setSelectedRoom(room as ChatRoom)}
                      className={`p-4 border-b border-gray-100 dark:border-gray-700 cursor-pointer hover:bg-gray-50 dark:hover:bg-gray-700 ${
                        selectedRoom?.id === room.id
                          ? 'bg-blue-50 dark:bg-blue-900'
                          : ''
                      }`}
                    >
                      <div className='flex items-center justify-between'>
                        <div className='flex-1'>
                          <h3 className='font-medium text-gray-900 dark:text-white truncate'>
                            {room.name}
                          </h3>
                          {room.projectName && (
                            <p className='text-sm text-gray-500 dark:text-gray-400'>
                              {room.projectName}
                            </p>
                          )}
                          {room.lastMessage && (
                            <p className='text-sm text-gray-600 dark:text-gray-300 truncate'>
                              {room.lastMessage.content}
                            </p>
                          )}
                        </div>
                        <div className='text-right'>
                          <div className='text-xs text-gray-500 dark:text-gray-400'>
                            {room.participantCount} members
                          </div>
                          {room.unreadCount > 0 && (
                            <div className='mt-1'>
                              <span className='inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white bg-red-500 rounded-full'>
                                {room.unreadCount}
                              </span>
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                  ))
              )}
            </div>
          </div>

          {/* Main Chat Area */}
          <div className='flex-1 flex flex-col'>
            {selectedRoom ? (
              <>
                {/* Room Header */}
                <div className='p-4 border-b border-gray-200 dark:border-gray-700'>
                  <div className='flex items-center justify-between'>
                    <div>
                      <h3 className='font-semibold text-gray-900 dark:text-white'>
                        {selectedRoom.name}
                      </h3>
                      <p className='text-sm text-gray-500 dark:text-gray-400'>
                        {selectedRoom.participantCount} members
                      </p>
                    </div>
                    <div className='flex items-center space-x-2'>
                      <EllipsisHorizontalIcon className='h-5 w-5 text-gray-500' />
                    </div>
                  </div>
                </div>

                {/* Messages */}
                <div className='flex-1 overflow-y-auto p-4 space-y-4'>
                  {messages.map((message) => (
                    <div
                      key={message.id}
                      className={`flex ${
                        isOwnMessage(message) ? 'justify-end' : 'justify-start'
                      }`}
                    >
                      <div
                        className={`max-w-xs lg:max-w-md px-4 py-2 rounded-lg ${
                          isOwnMessage(message)
                            ? 'bg-blue-500 text-white'
                            : 'bg-gray-100 dark:bg-gray-700 text-gray-900 dark:text-white'
                        }`}
                      >
                        {!isOwnMessage(message) && (
                          <div className='text-xs font-medium mb-1 text-gray-600 dark:text-gray-400'>
                            {message.senderName}
                          </div>
                        )}
                        <div className='text-sm'>{message.content}</div>
                        <div className='text-xs mt-1 opacity-70'>
                          {formatMessageTime(message.createdAt)}
                          {message.isEdited && ' (edited)'}
                        </div>
                      </div>
                    </div>
                  ))}

                  {/* Typing indicator */}
                  {typingUsers.length > 0 && (
                    <div className='flex justify-start'>
                      <div className='bg-gray-100 dark:bg-gray-700 px-4 py-2 rounded-lg'>
                        <div className='text-sm text-gray-600 dark:text-gray-400'>
                          {typingUsers.join(', ')}{' '}
                          {typingUsers.length === 1 ? 'is' : 'are'} typing...
                        </div>
                      </div>
                    </div>
                  )}

                  <div ref={messagesEndRef} />
                </div>

                {/* Message Input */}
                <div className='p-4 border-t border-gray-200 dark:border-gray-700'>
                  <div className='flex space-x-2'>
                    <input
                      type='text'
                      value={newMessage}
                      onChange={(e) => {
                        setNewMessage(e.target.value);
                        handleTyping();
                      }}
                      onKeyPress={(e) => {
                        if (e.key === 'Enter' && !e.shiftKey) {
                          e.preventDefault();
                          handleSendMessage();
                        }
                      }}
                      placeholder='Type a message...'
                      className='flex-1 px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-white'
                    />
                    <button
                      onClick={handleSendMessage}
                      disabled={!newMessage.trim()}
                      className='px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed'
                    >
                      <PaperAirplaneIcon className='h-5 w-5' />
                    </button>
                  </div>
                </div>
              </>
            ) : (
              <div className='flex-1 flex items-center justify-center'>
                <div className='text-center'>
                  <ChatBubbleLeftIcon className='h-12 w-12 text-gray-400 mx-auto mb-4' />
                  <h3 className='text-lg font-medium text-gray-900 dark:text-white mb-2'>
                    Select a chat room
                  </h3>
                  <p className='text-gray-500 dark:text-gray-400'>
                    Choose a room from the sidebar to start chatting
                  </p>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};
