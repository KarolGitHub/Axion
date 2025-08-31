import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuthStore } from '../state/authStore';
import {
  HomeIcon,
  FolderIcon,
  CheckCircleIcon,
  CalendarIcon,
  UsersIcon,
  CogIcon,
  ArrowRightOnRectangleIcon,
} from '@heroicons/react/24/outline';

interface LayoutProps {
  children: React.ReactNode;
}

const navigation = [
  { name: 'Dashboard', href: '/', icon: HomeIcon },
  { name: 'Projects', href: '/projects', icon: FolderIcon },
  { name: 'Tasks', href: '/tasks', icon: CheckCircleIcon },
  { name: 'Resources', href: '/resources', icon: CalendarIcon },
  { name: 'Users', href: '/users', icon: UsersIcon },
  { name: 'Settings', href: '/settings', icon: CogIcon },
];

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const location = useLocation();
  const { user, logout } = useAuthStore();

  const handleLogout = () => {
    logout();
  };

  return (
    <div className='min-h-screen bg-gray-50'>
      {/* Sidebar */}
      <div className='fixed inset-y-0 left-0 z-50 w-64 bg-white shadow-lg'>
        <div className='flex h-full flex-col'>
          {/* Logo */}
          <div className='flex h-16 items-center justify-center border-b border-gray-200'>
            <h1 className='text-2xl font-bold text-blue-600'>Axion</h1>
          </div>

          {/* Navigation */}
          <nav className='flex-1 space-y-1 px-4 py-4'>
            {navigation.map((item) => {
              const isActive = location.pathname === item.href;
              return (
                <Link
                  key={item.name}
                  to={item.href}
                  className={`group flex items-center rounded-md px-3 py-2 text-sm font-medium transition-colors ${
                    isActive
                      ? 'bg-blue-100 text-blue-700'
                      : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                  }`}
                >
                  <item.icon
                    className={`mr-3 h-5 w-5 flex-shrink-0 ${
                      isActive ? 'text-blue-700' : 'text-gray-400'
                    }`}
                  />
                  {item.name}
                </Link>
              );
            })}
          </nav>

          {/* User info and logout */}
          <div className='border-t border-gray-200 p-4'>
            <div className='flex items-center'>
              <div className='flex-shrink-0'>
                <div className='h-8 w-8 rounded-full bg-blue-600 flex items-center justify-center'>
                  <span className='text-sm font-medium text-white'>
                    {user?.firstName?.[0]}
                    {user?.lastName?.[0]}
                  </span>
                </div>
              </div>
              <div className='ml-3 flex-1'>
                <p className='text-sm font-medium text-gray-900'>
                  {user?.firstName} {user?.lastName}
                </p>
                <p className='text-xs text-gray-500'>{user?.role}</p>
              </div>
              <button
                onClick={handleLogout}
                className='ml-2 rounded-md p-1 text-gray-400 hover:text-gray-600'
                title='Logout'
              >
                <ArrowRightOnRectangleIcon className='h-5 w-5' />
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Main content */}
      <div className='pl-64'>
        <main className='min-h-screen p-6'>{children}</main>
      </div>
    </div>
  );
};

