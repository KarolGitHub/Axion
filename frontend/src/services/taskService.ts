import { api, endpoints } from '../utils/api';
import { Task } from '../types';

export interface CreateTaskRequest {
  title: string;
  description: string;
  status: 'Todo' | 'In Progress' | 'Review' | 'Done';
  priority: 'Low' | 'Medium' | 'High';
  projectId: string;
  assignedToId: string;
  createdById: string;
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  status?: 'Todo' | 'In Progress' | 'Review' | 'Done';
  priority?: 'Low' | 'Medium' | 'High';
  projectId?: string;
  assignedToId?: string;
  dueDate?: string;
}

export interface UpdateTaskStatusRequest {
  status: 'Todo' | 'In Progress' | 'Review' | 'Done';
}

export const taskService = {
  // Get all tasks
  getAll: async (): Promise<Task[]> => {
    try {
      const response = await api.get<Task[]>(endpoints.tasks.getAll);
      return response;
    } catch (error) {
      console.error('Error fetching tasks:', error);
      throw error;
    }
  },

  // Get task by ID
  getById: async (id: string): Promise<Task> => {
    try {
      const response = await api.get<Task>(endpoints.tasks.getById(id));
      return response;
    } catch (error) {
      console.error(`Error fetching task ${id}:`, error);
      throw error;
    }
  },

  // Get tasks by project
  getByProject: async (projectId: string): Promise<Task[]> => {
    try {
      const response = await api.get<Task[]>(
        endpoints.tasks.getByProject(projectId)
      );
      return response;
    } catch (error) {
      console.error(`Error fetching tasks for project ${projectId}:`, error);
      throw error;
    }
  },

  // Create new task
  create: async (taskData: CreateTaskRequest): Promise<Task> => {
    try {
      const response = await api.post<Task>(endpoints.tasks.create, taskData);
      return response;
    } catch (error) {
      console.error('Error creating task:', error);
      throw error;
    }
  },

  // Update task
  update: async (id: string, taskData: UpdateTaskRequest): Promise<void> => {
    try {
      await api.put(endpoints.tasks.update(id), taskData);
    } catch (error) {
      console.error(`Error updating task ${id}:`, error);
      throw error;
    }
  },

  // Update task status (for drag-and-drop)
  updateStatus: async (
    id: string,
    status: UpdateTaskStatusRequest
  ): Promise<void> => {
    try {
      await api.patch(endpoints.tasks.updateStatus(id), status);
    } catch (error) {
      console.error(`Error updating task status ${id}:`, error);
      throw error;
    }
  },

  // Delete task
  delete: async (id: string): Promise<void> => {
    try {
      await api.delete(endpoints.tasks.delete(id));
    } catch (error) {
      console.error(`Error deleting task ${id}:`, error);
      throw error;
    }
  },
};
