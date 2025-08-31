import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuthStore } from '../state/authStore';
import { useTheme } from '../contexts/ThemeContext';
import { Chat } from './Chat';
import {
  HomeIcon,
  FolderIcon,
  CheckCircleIcon,
  CalendarIcon,
  UsersIcon,
  CogIcon,
  ArrowRightOnRectangleIcon,
  ChartBarIcon,
  SunIcon,
  MoonIcon,
  ChatBubbleLeftIcon,
  PuzzlePieceIcon,
} from '@heroicons/react/24/outline';

interface LayoutProps {
  children: React.ReactNode;
}

const navigation = [
  { name: 'Dashboard', href: '/', icon: HomeIcon },
  { name: 'Projects', href: '/projects', icon: FolderIcon },
  { name: 'Tasks', href: '/tasks', icon: CheckCircleIcon },
  { name: 'Resources', href: '/resources', icon: CalendarIcon },
  { name: 'Bookings', href: '/bookings', icon: CalendarIcon },
  { name: 'Analytics', href: '/analytics', icon: ChartBarIcon },
  { name: 'Calendar', href: '/calendar', icon: CalendarIcon },
  { name: 'Integrations', href: '/integrations', icon: PuzzlePieceIcon },
  { name: 'Users', href: '/users', icon: UsersIcon },
  { name: 'Settings', href: '/settings', icon: CogIcon },
];

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const location = useLocation();
  const { user, logout } = useAuthStore();
  const { isDarkMode, toggleDarkMode } = useTheme();
  const [isChatOpen, setIsChatOpen] = useState(false);

  return (
    <div className='flex h-screen bg-gray-100 dark:bg-gray-900'>
      {/* Sidebar */}
      <div className='w-64 bg-white dark:bg-gray-800 shadow-lg'>
        <div className='flex items-center justify-center h-16 bg-blue-600 dark:bg-blue-700'>
          <h1 className='text-xl font-bold text-white'>Axion</h1>
        </div>

        {/* Navigation */}
        <nav className='mt-8'>
          <div className='px-4 space-y-1'>
            {navigation.map((item) => {
              const isActive = location.pathname === item.href;
              return (
                <Link
                  key={item.name}
                  to={item.href}
                  className={`group flex items-center px-2 py-2 text-sm font-medium rounded-md transition-colors duration-200 ${
                    isActive
                      ? 'bg-blue-100 text-blue-900 dark:bg-blue-900 dark:text-blue-100'
                      : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900 dark:text-gray-300 dark:hover:bg-gray-700 dark:hover:text-gray-200'
                  }`}
                >
                  <item.icon
                    className={`mr-3 h-5 w-5 ${
                      isActive
                        ? 'text-blue-500 dark:text-blue-400'
                        : 'text-gray-400 group-hover:text-gray-500 dark:group-hover:text-gray-400'
                    }`}
                  />
                  {item.name}
                </Link>
              );
            })}
          </div>
        </nav>

        {/* User Info */}
        <div className='absolute bottom-0 w-64 p-4 border-t border-gray-200 dark:border-gray-700'>
          <div className='flex items-center justify-between'>
            <div className='flex items-center'>
              <div className='flex-shrink-0'>
                <div className='h-8 w-8 bg-blue-500 rounded-full flex items-center justify-center'>
                  <span className='text-white font-medium'>
                    {user?.firstName?.charAt(0) || 'U'}
                  </span>
                </div>
              </div>
              <div className='ml-3 flex-1'>
                <p className='text-sm font-medium text-gray-700 dark:text-gray-200'>
                  {user?.firstName} {user?.lastName}
                </p>
                <p className='text-xs text-gray-500 dark:text-gray-400'>
                  {user?.email}
                </p>
              </div>
            </div>
            <div className='flex items-center space-x-2'>
              <button
                onClick={() => setIsChatOpen(true)}
                className='text-gray-400 hover:text-gray-600 dark:text-gray-400 dark:hover:text-gray-200'
                title='Open Chat'
              >
                <ChatBubbleLeftIcon className='h-5 w-5' />
              </button>
              <button
                onClick={toggleDarkMode}
                className='text-gray-400 hover:text-gray-600 dark:text-gray-400 dark:hover:text-gray-200'
                title={
                  isDarkMode ? 'Switch to light mode' : 'Switch to dark mode'
                }
              >
                {isDarkMode ? (
                  <SunIcon className='h-5 w-5' />
                ) : (
                  <MoonIcon className='h-5 w-5' />
                )}
              </button>
              <button
                onClick={logout}
                className='text-gray-400 hover:text-gray-600 dark:text-gray-400 dark:hover:text-gray-200'
                title='Logout'
              >
                <ArrowRightOnRectangleIcon className='h-5 w-5' />
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className='flex-1 overflow-auto'>
        <main className='p-6'>{children}</main>
      </div>

      {/* Chat Component */}
      <Chat isOpen={isChatOpen} onClose={() => setIsChatOpen(false)} />
    </div>
  );
};
