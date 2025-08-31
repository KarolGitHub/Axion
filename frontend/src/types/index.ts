export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'Employee' | 'Manager' | 'Admin';
  avatar?: string;
  createdAt: string;
  updatedAt: string;
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
  assignedTo: string[];
  createdAt: string;
  updatedAt: string;
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
  createdAt: string;
  updatedAt: string;
}

export interface Comment {
  id: string;
  content: string;
  taskId: string;
  userId: string;
  createdAt: string;
  updatedAt: string;
}

export interface Resource {
  id: string;
  name: string;
  type: 'Meeting Room' | 'Desk' | 'Equipment';
  capacity?: number;
  location: string;
  isAvailable: boolean;
}

export interface Booking {
  id: string;
  resourceId: string;
  userId: string;
  startTime: string;
  endTime: string;
  purpose: string;
  createdAt: string;
}

export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterData {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: 'Employee' | 'Manager' | 'Admin';
}

