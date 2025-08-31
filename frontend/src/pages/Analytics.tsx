import React, { useState, useEffect } from 'react';
import {
  ChartBarIcon,
  DocumentArrowDownIcon,
  CalendarIcon,
  UsersIcon,
  ClockIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  FolderIcon,
} from '@heroicons/react/24/outline';
import { Project, Task, Resource, Booking } from '../types';
import { projectService } from '../services/projectService';
import { taskService } from '../services/taskService';
import { resourceService } from '../services/resourceService';
import { bookingService } from '../services/bookingService';
import { useAuthStore } from '../state/authStore';

interface AnalyticsData {
  projects: Project[];
  tasks: Task[];
  resources: Resource[];
  bookings: Booking[];
  loading: boolean;
  error: string | null;
}

export const Analytics: React.FC = () => {
  const [data, setData] = useState<AnalyticsData>({
    projects: [],
    tasks: [],
    resources: [],
    bookings: [],
    loading: true,
    error: null,
  });
  const [selectedPeriod, setSelectedPeriod] = useState('30'); // days
  const [exporting, setExporting] = useState(false);

  const { user } = useAuthStore();

  useEffect(() => {
    loadAnalyticsData();
  }, [selectedPeriod]);

  const loadAnalyticsData = async () => {
    try {
      setData((prev) => ({ ...prev, loading: true, error: null }));

      const [projects, tasks, resources, bookings] = await Promise.all([
        projectService.getAll(),
        taskService.getAll(),
        resourceService.getAll(),
        bookingService.getAll(),
      ]);

      setData({
        projects,
        tasks,
        resources,
        bookings,
        loading: false,
        error: null,
      });
    } catch (err) {
      setData((prev) => ({
        ...prev,
        loading: false,
        error: 'Failed to load analytics data',
      }));
      console.error('Error loading analytics:', err);
    }
  };

  // Calculate statistics
  const getProjectStats = () => {
    const total = data.projects.length;
    const active = data.projects.filter((p) => p.status === 'Active').length;
    const completed = data.projects.filter(
      (p) => p.status === 'Completed'
    ).length;
    const onHold = data.projects.filter((p) => p.status === 'OnHold').length;

    return { total, active, completed, onHold };
  };

  const getTaskStats = () => {
    const total = data.tasks.length;
    const todo = data.tasks.filter((t) => t.status === 'Todo').length;
    const inProgress = data.tasks.filter(
      (t) => t.status === 'In Progress'
    ).length;
    const review = data.tasks.filter((t) => t.status === 'Review').length;
    const done = data.tasks.filter((t) => t.status === 'Done').length;

    return { total, todo, inProgress, review, done };
  };

  const getResourceStats = () => {
    const total = data.resources.length;
    const available = data.resources.filter((r) => r.isAvailable).length;
    const meetingRooms = data.resources.filter(
      (r) => r.type === 'Meeting Room'
    ).length;
    const desks = data.resources.filter((r) => r.type === 'Desk').length;
    const equipment = data.resources.filter(
      (r) => r.type === 'Equipment'
    ).length;

    return { total, available, meetingRooms, desks, equipment };
  };

  const getBookingStats = () => {
    const total = data.bookings.length;
    const now = new Date();
    const upcoming = data.bookings.filter(
      (b) => new Date(b.startTime) > now
    ).length;
    const ongoing = data.bookings.filter((b) => {
      const start = new Date(b.startTime);
      const end = new Date(b.endTime);
      return now >= start && now <= end;
    }).length;
    const completed = data.bookings.filter(
      (b) => new Date(b.endTime) < now
    ).length;

    return { total, upcoming, ongoing, completed };
  };

  const getRecentActivity = () => {
    const allItems = [
      ...data.projects.map((p) => ({
        ...p,
        type: 'project',
        date: p.createdAt,
      })),
      ...data.tasks.map((t) => ({ ...t, type: 'task', date: t.createdAt })),
      ...data.bookings.map((b) => ({
        ...b,
        type: 'booking',
        date: b.createdAt,
      })),
    ];

    return allItems
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
      .slice(0, 10);
  };

  const exportToPDF = async () => {
    setExporting(true);
    try {
      // Simulate PDF export
      await new Promise((resolve) => setTimeout(resolve, 2000));
      alert('PDF export functionality will be implemented with a PDF library');
    } catch (err) {
      console.error('Error exporting PDF:', err);
    } finally {
      setExporting(false);
    }
  };

  const exportToExcel = async () => {
    setExporting(true);
    try {
      // Simulate Excel export
      await new Promise((resolve) => setTimeout(resolve, 2000));
      alert(
        'Excel export functionality will be implemented with an Excel library'
      );
    } catch (err) {
      console.error('Error exporting Excel:', err);
    } finally {
      setExporting(false);
    }
  };

  if (data.loading) {
    return (
      <div className='space-y-6'>
        <div className='flex items-center justify-between'>
          <div>
            <h1 className='text-3xl font-bold text-gray-900'>Analytics</h1>
            <p className='mt-2 text-gray-600'>Loading analytics data...</p>
          </div>
        </div>
        <div className='grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4'>
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className='card'>
              <div className='animate-pulse'>
                <div className='h-4 bg-gray-200 rounded w-3/4 mb-4'></div>
                <div className='h-8 bg-gray-200 rounded w-1/2'></div>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  const projectStats = getProjectStats();
  const taskStats = getTaskStats();
  const resourceStats = getResourceStats();
  const bookingStats = getBookingStats();
  const recentActivity = getRecentActivity();

  return (
    <div className='space-y-6'>
      {/* Header */}
      <div className='flex items-center justify-between'>
        <div>
          <h1 className='text-3xl font-bold text-gray-900'>
            Analytics Dashboard
          </h1>
          <p className='mt-2 text-gray-600'>
            Comprehensive insights into your project performance and resource
            utilization.
          </p>
        </div>
        <div className='flex space-x-3'>
          <select
            value={selectedPeriod}
            onChange={(e) => setSelectedPeriod(e.target.value)}
            className='input-field w-auto'
          >
            <option value='7'>Last 7 days</option>
            <option value='30'>Last 30 days</option>
            <option value='90'>Last 90 days</option>
            <option value='365'>Last year</option>
          </select>
          <button
            onClick={exportToPDF}
            disabled={exporting}
            className='btn-secondary flex items-center'
          >
            <DocumentArrowDownIcon className='h-4 w-4 mr-2' />
            {exporting ? 'Exporting...' : 'Export PDF'}
          </button>
          <button
            onClick={exportToExcel}
            disabled={exporting}
            className='btn-primary flex items-center'
          >
            <DocumentArrowDownIcon className='h-4 w-4 mr-2' />
            {exporting ? 'Exporting...' : 'Export Excel'}
          </button>
        </div>
      </div>

      {/* Error Message */}
      {data.error && (
        <div className='bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded'>
          {data.error}
        </div>
      )}

      {/* Key Metrics */}
      <div className='grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4'>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-blue-500 rounded-full flex items-center justify-center'>
                <FolderIcon className='h-5 w-5 text-white' />
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>
                Total Projects
              </p>
              <p className='text-2xl font-semibold text-gray-900'>
                {projectStats.total}
              </p>
            </div>
          </div>
          <div className='mt-4'>
            <div className='flex justify-between text-sm'>
              <span className='text-green-600'>
                Active: {projectStats.active}
              </span>
              <span className='text-blue-600'>
                Completed: {projectStats.completed}
              </span>
            </div>
          </div>
        </div>

        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-green-500 rounded-full flex items-center justify-center'>
                <CheckCircleIcon className='h-5 w-5 text-white' />
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Total Tasks</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {taskStats.total}
              </p>
            </div>
          </div>
          <div className='mt-4'>
            <div className='flex justify-between text-sm'>
              <span className='text-blue-600'>Done: {taskStats.done}</span>
              <span className='text-yellow-600'>
                In Progress: {taskStats.inProgress}
              </span>
            </div>
          </div>
        </div>

        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-purple-500 rounded-full flex items-center justify-center'>
                <CalendarIcon className='h-5 w-5 text-white' />
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>
                Total Resources
              </p>
              <p className='text-2xl font-semibold text-gray-900'>
                {resourceStats.total}
              </p>
            </div>
          </div>
          <div className='mt-4'>
            <div className='flex justify-between text-sm'>
              <span className='text-green-600'>
                Available: {resourceStats.available}
              </span>
              <span className='text-blue-600'>
                Rooms: {resourceStats.meetingRooms}
              </span>
            </div>
          </div>
        </div>

        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-yellow-500 rounded-full flex items-center justify-center'>
                <ClockIcon className='h-5 w-5 text-white' />
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>
                Total Bookings
              </p>
              <p className='text-2xl font-semibold text-gray-900'>
                {bookingStats.total}
              </p>
            </div>
          </div>
          <div className='mt-4'>
            <div className='flex justify-between text-sm'>
              <span className='text-blue-600'>
                Upcoming: {bookingStats.upcoming}
              </span>
              <span className='text-green-600'>
                Ongoing: {bookingStats.ongoing}
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Charts Section */}
      <div className='grid grid-cols-1 gap-6 lg:grid-cols-2'>
        {/* Project Status Chart */}
        <div className='card'>
          <h3 className='text-lg font-medium text-gray-900 mb-4'>
            Project Status Distribution
          </h3>
          <div className='space-y-3'>
            <div className='flex items-center justify-between'>
              <span className='text-sm text-gray-600'>Active</span>
              <div className='flex items-center'>
                <div className='w-32 bg-gray-200 rounded-full h-2 mr-3'>
                  <div
                    className='bg-green-500 h-2 rounded-full'
                    style={{
                      width: `${
                        (projectStats.active / projectStats.total) * 100
                      }%`,
                    }}
                  ></div>
                </div>
                <span className='text-sm font-medium'>
                  {projectStats.active}
                </span>
              </div>
            </div>
            <div className='flex items-center justify-between'>
              <span className='text-sm text-gray-600'>Completed</span>
              <div className='flex items-center'>
                <div className='w-32 bg-gray-200 rounded-full h-2 mr-3'>
                  <div
                    className='bg-blue-500 h-2 rounded-full'
                    style={{
                      width: `${
                        (projectStats.completed / projectStats.total) * 100
                      }%`,
                    }}
                  ></div>
                </div>
                <span className='text-sm font-medium'>
                  {projectStats.completed}
                </span>
              </div>
            </div>
            <div className='flex items-center justify-between'>
              <span className='text-sm text-gray-600'>On Hold</span>
              <div className='flex items-center'>
                <div className='w-32 bg-gray-200 rounded-full h-2 mr-3'>
                  <div
                    className='bg-yellow-500 h-2 rounded-full'
                    style={{
                      width: `${
                        (projectStats.onHold / projectStats.total) * 100
                      }%`,
                    }}
                  ></div>
                </div>
                <span className='text-sm font-medium'>
                  {projectStats.onHold}
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Task Status Chart */}
        <div className='card'>
          <h3 className='text-lg font-medium text-gray-900 mb-4'>
            Task Status Distribution
          </h3>
          <div className='space-y-3'>
            <div className='flex items-center justify-between'>
              <span className='text-sm text-gray-600'>Done</span>
              <div className='flex items-center'>
                <div className='w-32 bg-gray-200 rounded-full h-2 mr-3'>
                  <div
                    className='bg-green-500 h-2 rounded-full'
                    style={{
                      width: `${(taskStats.done / taskStats.total) * 100}%`,
                    }}
                  ></div>
                </div>
                <span className='text-sm font-medium'>{taskStats.done}</span>
              </div>
            </div>
            <div className='flex items-center justify-between'>
              <span className='text-sm text-gray-600'>In Progress</span>
              <div className='flex items-center'>
                <div className='w-32 bg-gray-200 rounded-full h-2 mr-3'>
                  <div
                    className='bg-blue-500 h-2 rounded-full'
                    style={{
                      width: `${
                        (taskStats.inProgress / taskStats.total) * 100
                      }%`,
                    }}
                  ></div>
                </div>
                <span className='text-sm font-medium'>
                  {taskStats.inProgress}
                </span>
              </div>
            </div>
            <div className='flex items-center justify-between'>
              <span className='text-sm text-gray-600'>Todo</span>
              <div className='flex items-center'>
                <div className='w-32 bg-gray-200 rounded-full h-2 mr-3'>
                  <div
                    className='bg-gray-500 h-2 rounded-full'
                    style={{
                      width: `${(taskStats.todo / taskStats.total) * 100}%`,
                    }}
                  ></div>
                </div>
                <span className='text-sm font-medium'>{taskStats.todo}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Activity */}
      <div className='card'>
        <h3 className='text-lg font-medium text-gray-900 mb-4'>
          Recent Activity
        </h3>
        <div className='space-y-3'>
          {recentActivity.map((item, index) => (
            <div
              key={index}
              className='flex items-center space-x-3 p-3 bg-gray-50 rounded-lg'
            >
              <div className='flex-shrink-0'>
                {item.type === 'project' && (
                  <FolderIcon className='h-5 w-5 text-blue-500' />
                )}
                {item.type === 'task' && (
                  <CheckCircleIcon className='h-5 w-5 text-green-500' />
                )}
                {item.type === 'booking' && (
                  <CalendarIcon className='h-5 w-5 text-purple-500' />
                )}
              </div>
              <div className='flex-1 min-w-0'>
                <p className='text-sm font-medium text-gray-900 truncate'>
                  {item.type === 'project' && `Project "${item.name}" created`}
                  {item.type === 'task' && `Task "${item.title}" created`}
                  {item.type === 'booking' &&
                    `Booking "${item.purpose}" created`}
                </p>
                <p className='text-xs text-gray-500'>
                  {new Date(item.date).toLocaleDateString()} at{' '}
                  {new Date(item.date).toLocaleTimeString()}
                </p>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Resource Utilization */}
      <div className='card'>
        <h3 className='text-lg font-medium text-gray-900 mb-4'>
          Resource Utilization
        </h3>
        <div className='grid grid-cols-1 gap-4 md:grid-cols-3'>
          <div className='text-center'>
            <div className='text-2xl font-bold text-blue-600'>
              {resourceStats.meetingRooms}
            </div>
            <div className='text-sm text-gray-600'>Meeting Rooms</div>
          </div>
          <div className='text-center'>
            <div className='text-2xl font-bold text-green-600'>
              {resourceStats.desks}
            </div>
            <div className='text-sm text-gray-600'>Desks</div>
          </div>
          <div className='text-center'>
            <div className='text-2xl font-bold text-purple-600'>
              {resourceStats.equipment}
            </div>
            <div className='text-sm text-gray-600'>Equipment</div>
          </div>
        </div>
      </div>
    </div>
  );
};
