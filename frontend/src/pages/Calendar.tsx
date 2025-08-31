import React, { useState, useEffect } from 'react';
import { CalendarIcon } from '@heroicons/react/24/outline';
import { Task, Project, Booking } from '../types';
import { taskService } from '../services/taskService';
import { projectService } from '../services/projectService';
import { bookingService } from '../services/bookingService';
import { CalendarIntegration } from '../components/CalendarIntegration';

export const Calendar: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    setError(null);

    try {
      const [tasksData, projectsData, bookingsData] = await Promise.all([
        taskService.getTasks(),
        projectService.getProjects(),
        bookingService.getBookings(),
      ]);

      setTasks(tasksData);
      setProjects(projectsData);
      setBookings(bookingsData);
    } catch (err) {
      setError('Failed to load calendar data');
      console.error('Error loading calendar data:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className='container mx-auto px-4 py-8'>
        <div className='animate-pulse space-y-4'>
          <div className='h-8 bg-gray-200 rounded w-1/4'></div>
          <div className='h-64 bg-gray-200 rounded'></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className='container mx-auto px-4 py-8'>
        <div className='text-center'>
          <CalendarIcon className='h-12 w-12 text-red-500 mx-auto mb-4' />
          <h2 className='text-xl font-semibold text-gray-900 mb-2'>
            Error Loading Calendar
          </h2>
          <p className='text-gray-600 mb-4'>{error}</p>
          <button onClick={loadData} className='btn-primary'>
            Try Again
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className='container mx-auto px-4 py-8'>
      <div className='mb-8'>
        <div className='flex items-center space-x-2 mb-2'>
          <CalendarIcon className='h-6 w-6 text-blue-500' />
          <h1 className='text-2xl font-bold text-gray-900'>
            Calendar Integration
          </h1>
        </div>
        <p className='text-gray-600'>
          Connect your external calendars and manage event synchronization.
        </p>
      </div>

      <CalendarIntegration
        tasks={tasks}
        projects={projects}
        bookings={bookings}
      />
    </div>
  );
};
