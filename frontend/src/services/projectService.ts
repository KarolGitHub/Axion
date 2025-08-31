import { api, endpoints } from '../utils/api';
import { Project } from '../types';

export interface CreateProjectRequest {
  name: string;
  description: string;
  status: 'Active' | 'Completed' | 'On Hold';
  priority: 'Low' | 'Medium' | 'High';
  startDate: string;
  endDate?: string;
  createdById: string;
}

export interface UpdateProjectRequest {
  name?: string;
  description?: string;
  status?: 'Active' | 'Completed' | 'On Hold';
  priority?: 'Low' | 'Medium' | 'High';
  startDate?: string;
  endDate?: string;
}

export const projectService = {
  // Get all projects
  getAll: async (): Promise<Project[]> => {
    try {
      const response = await api.get<Project[]>(endpoints.projects.getAll);
      return response;
    } catch (error) {
      console.error('Error fetching projects:', error);
      throw error;
    }
  },

  // Get project by ID
  getById: async (id: string): Promise<Project> => {
    try {
      const response = await api.get<Project>(endpoints.projects.getById(id));
      return response;
    } catch (error) {
      console.error(`Error fetching project ${id}:`, error);
      throw error;
    }
  },

  // Create new project
  create: async (projectData: CreateProjectRequest): Promise<Project> => {
    try {
      const response = await api.post<Project>(
        endpoints.projects.create,
        projectData
      );
      return response;
    } catch (error) {
      console.error('Error creating project:', error);
      throw error;
    }
  },

  // Update project
  update: async (
    id: string,
    projectData: UpdateProjectRequest
  ): Promise<void> => {
    try {
      await api.put(endpoints.projects.update(id), projectData);
    } catch (error) {
      console.error(`Error updating project ${id}:`, error);
      throw error;
    }
  },

  // Delete project
  delete: async (id: string): Promise<void> => {
    try {
      await api.delete(endpoints.projects.delete(id));
    } catch (error) {
      console.error(`Error deleting project ${id}:`, error);
      throw error;
    }
  },
};
