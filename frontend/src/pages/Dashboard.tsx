import React from 'react';
import {
  FolderIcon,
  CheckCircleIcon,
  CalendarIcon,
  UsersIcon,
  ClockIcon,
  ExclamationTriangleIcon,
} from '@heroicons/react/24/outline';

const stats = [
  {
    name: 'Active Projects',
    value: '12',
    change: '+2.5%',
    changeType: 'positive',
    icon: FolderIcon,
  },
  {
    name: 'Tasks Completed',
    value: '89',
    change: '+12.3%',
    changeType: 'positive',
    icon: CheckCircleIcon,
  },
  {
    name: 'Resources Booked',
    value: '5',
    change: '-1.2%',
    changeType: 'negative',
    icon: CalendarIcon,
  },
  {
    name: 'Team Members',
    value: '24',
    change: '+0%',
    changeType: 'neutral',
    icon: UsersIcon,
  },
];

const recentActivity = [
  {
    id: 1,
    type: 'task',
    message: 'Task "Update user interface" was completed',
    time: '2 hours ago',
    user: 'John Doe',
  },
  {
    id: 2,
    type: 'project',
    message: 'Project "E-commerce redesign" was created',
    time: '4 hours ago',
    user: 'Jane Smith',
  },
  {
    id: 3,
    type: 'booking',
    message: 'Meeting room "Conference A" was booked',
    time: '6 hours ago',
    user: 'Mike Johnson',
  },
  {
    id: 4,
    type: 'task',
    message: 'Task "Fix login bug" was assigned to you',
    time: '1 day ago',
    user: 'Sarah Wilson',
  },
];

export const Dashboard: React.FC = () => {
  return (
    <div className='space-y-6'>
      {/* Header */}
      <div>
        <h1 className='text-3xl font-bold text-gray-900'>Dashboard</h1>
        <p className='mt-2 text-gray-600'>
          Welcome back! Here's what's happening with your projects today.
        </p>
      </div>

      {/* Stats Grid */}
      <div className='grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4'>
        {stats.map((stat) => (
          <div key={stat.name} className='card'>
            <div className='flex items-center'>
              <div className='flex-shrink-0'>
                <stat.icon className='h-8 w-8 text-gray-400' />
              </div>
              <div className='ml-4 flex-1'>
                <p className='text-sm font-medium text-gray-500'>{stat.name}</p>
                <p className='text-2xl font-semibold text-gray-900'>
                  {stat.value}
                </p>
              </div>
            </div>
            <div className='mt-4'>
              <span
                className={`inline-flex items-center text-sm font-medium ${
                  stat.changeType === 'positive'
                    ? 'text-green-600'
                    : stat.changeType === 'negative'
                    ? 'text-red-600'
                    : 'text-gray-500'
                }`}
              >
                {stat.change}
              </span>
              <span className='text-sm text-gray-500'> from last month</span>
            </div>
          </div>
        ))}
      </div>

      {/* Recent Activity */}
      <div className='card'>
        <div className='flex items-center justify-between'>
          <h2 className='text-lg font-medium text-gray-900'>Recent Activity</h2>
          <button className='text-sm text-blue-600 hover:text-blue-700'>
            View all
          </button>
        </div>
        <div className='mt-6 flow-root'>
          <ul className='-mb-8'>
            {recentActivity.map((activity, activityIdx) => (
              <li key={activity.id}>
                <div className='relative pb-8'>
                  {activityIdx !== recentActivity.length - 1 ? (
                    <span
                      className='absolute left-4 top-4 -ml-px h-full w-0.5 bg-gray-200'
                      aria-hidden='true'
                    />
                  ) : null}
                  <div className='relative flex space-x-3'>
                    <div>
                      <span className='h-8 w-8 rounded-full bg-blue-500 flex items-center justify-center ring-8 ring-white'>
                        {activity.type === 'task' && (
                          <CheckCircleIcon className='h-5 w-5 text-white' />
                        )}
                        {activity.type === 'project' && (
                          <FolderIcon className='h-5 w-5 text-white' />
                        )}
                        {activity.type === 'booking' && (
                          <CalendarIcon className='h-5 w-5 text-white' />
                        )}
                      </span>
                    </div>
                    <div className='flex min-w-0 flex-1 justify-between space-x-4 pt-1.5'>
                      <div>
                        <p className='text-sm text-gray-500'>
                          {activity.message}{' '}
                          <span className='font-medium text-gray-900'>
                            by {activity.user}
                          </span>
                        </p>
                      </div>
                      <div className='whitespace-nowrap text-right text-sm text-gray-500'>
                        <time dateTime={activity.time}>{activity.time}</time>
                      </div>
                    </div>
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </div>

      {/* Quick Actions */}
      <div className='grid grid-cols-1 gap-6 lg:grid-cols-2'>
        <div className='card'>
          <h3 className='text-lg font-medium text-gray-900 mb-4'>
            Quick Actions
          </h3>
          <div className='space-y-3'>
            <button className='w-full btn-primary'>Create New Project</button>
            <button className='w-full btn-secondary'>Book a Resource</button>
            <button className='w-full btn-secondary'>Add New Task</button>
          </div>
        </div>

        <div className='card'>
          <h3 className='text-lg font-medium text-gray-900 mb-4'>
            Upcoming Deadlines
          </h3>
          <div className='space-y-3'>
            <div className='flex items-center justify-between p-3 bg-yellow-50 rounded-lg'>
              <div className='flex items-center'>
                <ExclamationTriangleIcon className='h-5 w-5 text-yellow-600 mr-3' />
                <div>
                  <p className='text-sm font-medium text-gray-900'>
                    Project Review
                  </p>
                  <p className='text-xs text-gray-500'>Due tomorrow</p>
                </div>
              </div>
              <ClockIcon className='h-4 w-4 text-yellow-600' />
            </div>
            <div className='flex items-center justify-between p-3 bg-red-50 rounded-lg'>
              <div className='flex items-center'>
                <ExclamationTriangleIcon className='h-5 w-5 text-red-600 mr-3' />
                <div>
                  <p className='text-sm font-medium text-gray-900'>
                    Bug Fix Deadline
                  </p>
                  <p className='text-xs text-gray-500'>Overdue</p>
                </div>
              </div>
              <ClockIcon className='h-4 w-4 text-red-600' />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

