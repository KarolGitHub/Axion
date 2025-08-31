export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: 'Admin' | 'Manager' | 'Member';
  organizationId?: string;
}

export interface Project {
  id: string;
  name: string;
  description: string;
  status: 'Active' | 'Completed' | 'On Hold';
  priority: 'Low' | 'Medium' | 'High';
  startDate: string;
  endDate?: string;
  createdBy: string;
  organizationId?: string;
}

export interface Task {
  id: string;
  title: string;
  description: string;
  status: 'Todo' | 'In Progress' | 'Review' | 'Done';
  priority: 'Low' | 'Medium' | 'High';
  projectId: string;
  assignedTo: string;
  createdBy: string;
  dueDate?: string;
  organizationId?: string;
}

export interface Resource {
  id: string;
  name: string;
  type: 'Equipment' | 'Room' | 'Vehicle' | 'Other';
  location: string;
  isAvailable: boolean;
  organizationId?: string;
}

export interface Booking {
  id: string;
  resourceId: string;
  resourceName: string;
  resourceLocation: string;
  userId: string;
  userName: string;
  purpose: string;
  startTime: string;
  endTime: string;
  organizationId?: string;
}

export interface Organization {
  id: string;
  name: string;
  description: string;
  domain: string;
  status: 'Active' | 'Suspended' | 'Cancelled';
  plan: 'Free' | 'Basic' | 'Professional' | 'Enterprise';
  maxUsers: number;
  maxProjects: number;
  maxStorageGB: number;
}

export interface CalendarEvent {
  id: string;
  title: string;
  description?: string;
  startTime: Date;
  endTime: Date;
  location?: string;
  attendees?: string[];
  isAllDay: boolean;
  source: 'google' | 'outlook' | 'local';
  sourceId?: string;
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  user: User;
  token: string;
}
