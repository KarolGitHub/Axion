import { api, endpoints } from '../utils/api';
import { Resource, Booking } from '../types';

export interface CreateResourceRequest {
  name: string;
  type: 'Meeting Room' | 'Desk' | 'Equipment';
  capacity?: number;
  location: string;
  isAvailable: boolean;
}

export interface UpdateResourceRequest {
  name?: string;
  type?: 'Meeting Room' | 'Desk' | 'Equipment';
  capacity?: number;
  location?: string;
  isAvailable?: boolean;
}

export interface CreateBookingRequest {
  resourceId: string;
  userId: string;
  startTime: string;
  endTime: string;
  purpose: string;
}

export interface UpdateBookingRequest {
  resourceId?: string;
  userId?: string;
  startTime?: string;
  endTime?: string;
  purpose?: string;
}

export const resourceService = {
  // Get all resources
  getAll: async (): Promise<Resource[]> => {
    try {
      const response = await api.get<Resource[]>(endpoints.resources.getAll);
      return response;
    } catch (error) {
      console.error('Error fetching resources:', error);
      throw error;
    }
  },

  // Get resource by ID
  getById: async (id: string): Promise<Resource> => {
    try {
      const response = await api.get<Resource>(endpoints.resources.getById(id));
      return response;
    } catch (error) {
      console.error(`Error fetching resource ${id}:`, error);
      throw error;
    }
  },

  // Get available resources
  getAvailable: async (): Promise<Resource[]> => {
    try {
      const response = await api.get<Resource[]>(
        endpoints.resources.getAvailable
      );
      return response;
    } catch (error) {
      console.error('Error fetching available resources:', error);
      throw error;
    }
  },

  // Create new resource
  create: async (resourceData: CreateResourceRequest): Promise<Resource> => {
    try {
      const response = await api.post<Resource>(
        endpoints.resources.create,
        resourceData
      );
      return response;
    } catch (error) {
      console.error('Error creating resource:', error);
      throw error;
    }
  },

  // Update resource
  update: async (
    id: string,
    resourceData: UpdateResourceRequest
  ): Promise<void> => {
    try {
      await api.put(endpoints.resources.update(id), resourceData);
    } catch (error) {
      console.error(`Error updating resource ${id}:`, error);
      throw error;
    }
  },

  // Delete resource
  delete: async (id: string): Promise<void> => {
    try {
      await api.delete(endpoints.resources.delete(id));
    } catch (error) {
      console.error(`Error deleting resource ${id}:`, error);
      throw error;
    }
  },
};

export const bookingService = {
  // Get all bookings
  getAll: async (): Promise<Booking[]> => {
    try {
      const response = await api.get<Booking[]>(endpoints.bookings.getAll);
      return response;
    } catch (error) {
      console.error('Error fetching bookings:', error);
      throw error;
    }
  },

  // Get booking by ID
  getById: async (id: string): Promise<Booking> => {
    try {
      const response = await api.get<Booking>(endpoints.bookings.getById(id));
      return response;
    } catch (error) {
      console.error(`Error fetching booking ${id}:`, error);
      throw error;
    }
  },

  // Get bookings by resource
  getByResource: async (resourceId: string): Promise<Booking[]> => {
    try {
      const response = await api.get<Booking[]>(
        endpoints.bookings.getByResource(resourceId)
      );
      return response;
    } catch (error) {
      console.error(
        `Error fetching bookings for resource ${resourceId}:`,
        error
      );
      throw error;
    }
  },

  // Get bookings by user
  getByUser: async (userId: string): Promise<Booking[]> => {
    try {
      const response = await api.get<Booking[]>(
        endpoints.bookings.getByUser(userId)
      );
      return response;
    } catch (error) {
      console.error(`Error fetching bookings for user ${userId}:`, error);
      throw error;
    }
  },

  // Create new booking
  create: async (bookingData: CreateBookingRequest): Promise<Booking> => {
    try {
      const response = await api.post<Booking>(
        endpoints.bookings.create,
        bookingData
      );
      return response;
    } catch (error) {
      console.error('Error creating booking:', error);
      throw error;
    }
  },

  // Update booking
  update: async (
    id: string,
    bookingData: UpdateBookingRequest
  ): Promise<void> => {
    try {
      await api.put(endpoints.bookings.update(id), bookingData);
    } catch (error) {
      console.error(`Error updating booking ${id}:`, error);
      throw error;
    }
  },

  // Delete booking
  delete: async (id: string): Promise<void> => {
    try {
      await api.delete(endpoints.bookings.delete(id));
    } catch (error) {
      console.error(`Error deleting booking ${id}:`, error);
      throw error;
    }
  },

  // Check availability
  checkAvailability: async (
    resourceId: string,
    startTime: string,
    endTime: string
  ): Promise<{ isAvailable: boolean; hasConflict: boolean }> => {
    try {
      const params = new URLSearchParams({
        resourceId,
        startTime,
        endTime,
      });
      const response = await api.get<{
        isAvailable: boolean;
        hasConflict: boolean;
      }>(`${endpoints.bookings.checkAvailability}?${params}`);
      return response;
    } catch (error) {
      console.error('Error checking availability:', error);
      throw error;
    }
  },
};
