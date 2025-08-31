import React, { useState, useEffect } from 'react';
import {
  CalendarIcon,
  CloudArrowUpIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  CogIcon,
  ArrowPathIcon,
  DocumentArrowDownIcon,
} from '@heroicons/react/24/outline';
import { Task, Project, Booking } from '../types';
import {
  calendarService,
  CalendarEvent,
  CalendarSyncSettings,
  CalendarConflict,
} from '../services/calendarService';

interface CalendarIntegrationProps {
  tasks: Task[];
  projects: Project[];
  bookings: Booking[];
}

export const CalendarIntegration: React.FC<CalendarIntegrationProps> = ({
  tasks,
  projects,
  bookings,
}) => {
  const [syncSettings, setSyncSettings] = useState<CalendarSyncSettings>({
    syncTasks: true,
    syncBookings: true,
    syncDirection: 'two-way',
    autoSync: true,
  });
  const [syncedEvents, setSyncedEvents] = useState<CalendarEvent[]>([]);
  const [conflicts, setConflicts] = useState<CalendarConflict[]>([]);
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<
    'overview' | 'events' | 'conflicts' | 'settings'
  >('overview');
  const [googleConnected, setGoogleConnected] = useState(false);
  const [outlookConnected, setOutlookConnected] = useState(false);

  useEffect(() => {
    loadSyncSettings();
  }, []);

  const loadSyncSettings = () => {
    const saved = localStorage.getItem('calendarSyncSettings');
    if (saved) {
      setSyncSettings(JSON.parse(saved));
    }
  };

  const handleConnectGoogle = async () => {
    setLoading(true);
    try {
      const success = await calendarService.initializeCalendar('google');
      setGoogleConnected(success);
      if (success) {
        await syncCalendarData();
      }
    } catch (error) {
      console.error('Failed to connect Google Calendar:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleConnectOutlook = async () => {
    setLoading(true);
    try {
      const success = await calendarService.initializeCalendar('outlook');
      setOutlookConnected(success);
      if (success) {
        await syncCalendarData();
      }
    } catch (error) {
      console.error('Failed to connect Outlook Calendar:', error);
    } finally {
      setLoading(false);
    }
  };

  const syncCalendarData = async () => {
    setLoading(true);
    try {
      const result = await calendarService.syncWithExternalCalendars(
        tasks,
        bookings,
        projects
      );
      setSyncedEvents(result.syncedEvents);
      setConflicts(result.conflicts);
    } catch (error) {
      console.error('Failed to sync calendar data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleExportEvents = async (format: 'ics' | 'csv' | 'json') => {
    try {
      const data = await calendarService.exportCalendarEvents(
        syncedEvents,
        format
      );

      // Create and download file
      const blob = new Blob([data], {
        type: format === 'json' ? 'application/json' : 'text/plain',
      });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `calendar-events.${format}`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Failed to export events:', error);
    }
  };

  const handleUpdateSettings = async (
    updates: Partial<CalendarSyncSettings>
  ) => {
    const newSettings = { ...syncSettings, ...updates };
    setSyncSettings(newSettings);
    await calendarService.updateSyncSettings(newSettings);
  };

  const getConflictSeverityColor = (severity: string) => {
    switch (severity) {
      case 'high':
        return 'text-red-600 bg-red-100';
      case 'medium':
        return 'text-yellow-600 bg-yellow-100';
      case 'low':
        return 'text-green-600 bg-green-100';
      default:
        return 'text-gray-600 bg-gray-100';
    }
  };

  const getEventSourceIcon = (source: string) => {
    switch (source) {
      case 'google':
        return '🔵';
      case 'outlook':
        return '🟠';
      case 'local':
        return '🟢';
      default:
        return '⚪';
    }
  };

  if (loading) {
    return (
      <div className='card'>
        <div className='flex items-center space-x-2 mb-4'>
          <CalendarIcon className='h-5 w-5 text-blue-500' />
          <h3 className='text-lg font-medium text-gray-900'>
            Calendar Integration
          </h3>
        </div>
        <div className='animate-pulse space-y-3'>
          <div className='h-4 bg-gray-200 rounded w-3/4'></div>
          <div className='h-4 bg-gray-200 rounded w-1/2'></div>
          <div className='h-4 bg-gray-200 rounded w-2/3'></div>
        </div>
      </div>
    );
  }

  return (
    <div className='card'>
      <div className='flex items-center space-x-2 mb-4'>
        <CalendarIcon className='h-5 w-5 text-blue-500' />
        <h3 className='text-lg font-medium text-gray-900'>
          Calendar Integration
        </h3>
      </div>

      {/* Tab Navigation */}
      <div className='flex space-x-1 mb-4'>
        <button
          onClick={() => setActiveTab('overview')}
          className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            activeTab === 'overview'
              ? 'bg-blue-100 text-blue-700'
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Overview
        </button>
        <button
          onClick={() => setActiveTab('events')}
          className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            activeTab === 'events'
              ? 'bg-blue-100 text-blue-700'
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Events
        </button>
        <button
          onClick={() => setActiveTab('conflicts')}
          className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            activeTab === 'conflicts'
              ? 'bg-blue-100 text-blue-700'
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Conflicts
        </button>
        <button
          onClick={() => setActiveTab('settings')}
          className={`px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            activeTab === 'settings'
              ? 'bg-blue-100 text-blue-700'
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Settings
        </button>
      </div>

      {/* Tab Content */}
      <div className='space-y-4'>
        {activeTab === 'overview' && (
          <div>
            <h4 className='text-sm font-medium text-gray-700 mb-3'>
              Calendar Overview
            </h4>

            {/* Connection Status */}
            <div className='grid grid-cols-1 md:grid-cols-2 gap-4 mb-6'>
              <div className='p-4 border rounded-lg'>
                <div className='flex items-center justify-between mb-2'>
                  <span className='text-sm font-medium text-gray-700'>
                    Google Calendar
                  </span>
                  {googleConnected ? (
                    <CheckCircleIcon className='h-5 w-5 text-green-500' />
                  ) : (
                    <ExclamationTriangleIcon className='h-5 w-5 text-yellow-500' />
                  )}
                </div>
                <button
                  onClick={handleConnectGoogle}
                  disabled={loading}
                  className='btn-primary text-sm'
                >
                  {googleConnected ? 'Reconnect' : 'Connect'}
                </button>
              </div>

              <div className='p-4 border rounded-lg'>
                <div className='flex items-center justify-between mb-2'>
                  <span className='text-sm font-medium text-gray-700'>
                    Outlook Calendar
                  </span>
                  {outlookConnected ? (
                    <CheckCircleIcon className='h-5 w-5 text-green-500' />
                  ) : (
                    <ExclamationTriangleIcon className='h-5 w-5 text-yellow-500' />
                  )}
                </div>
                <button
                  onClick={handleConnectOutlook}
                  disabled={loading}
                  className='btn-primary text-sm'
                >
                  {outlookConnected ? 'Reconnect' : 'Connect'}
                </button>
              </div>
            </div>

            {/* Sync Stats */}
            <div className='grid grid-cols-1 md:grid-cols-3 gap-4 mb-6'>
              <div className='text-center p-4 bg-blue-50 rounded-lg'>
                <div className='text-2xl font-bold text-blue-600'>
                  {syncedEvents.length}
                </div>
                <div className='text-sm text-gray-600'>Total Events</div>
              </div>
              <div className='text-center p-4 bg-yellow-50 rounded-lg'>
                <div className='text-2xl font-bold text-yellow-600'>
                  {conflicts.length}
                </div>
                <div className='text-sm text-gray-600'>Conflicts</div>
              </div>
              <div className='text-center p-4 bg-green-50 rounded-lg'>
                <div className='text-2xl font-bold text-green-600'>
                  {syncedEvents.filter((e) => e.source === 'local').length}
                </div>
                <div className='text-sm text-gray-600'>Local Events</div>
              </div>
            </div>

            {/* Quick Actions */}
            <div className='flex space-x-3'>
              <button
                onClick={syncCalendarData}
                disabled={loading}
                className='btn-primary flex items-center'
              >
                <ArrowPathIcon className='h-4 w-4 mr-2' />
                Sync Now
              </button>
              <button
                onClick={() => handleExportEvents('ics')}
                className='btn-secondary flex items-center'
              >
                <DocumentArrowDownIcon className='h-4 w-4 mr-2' />
                Export ICS
              </button>
            </div>
          </div>
        )}

        {activeTab === 'events' && (
          <div>
            <h4 className='text-sm font-medium text-gray-700 mb-3'>
              Calendar Events
            </h4>
            <div className='space-y-3'>
              {syncedEvents.length > 0 ? (
                syncedEvents.map((event) => (
                  <div key={event.id} className='p-3 bg-gray-50 rounded-lg'>
                    <div className='flex items-start justify-between'>
                      <div className='flex-1'>
                        <div className='flex items-center space-x-2 mb-1'>
                          <span className='text-lg'>
                            {getEventSourceIcon(event.source)}
                          </span>
                          <h5 className='text-sm font-medium text-gray-900'>
                            {event.title}
                          </h5>
                        </div>
                        {event.description && (
                          <p className='text-xs text-gray-600 mb-2'>
                            {event.description}
                          </p>
                        )}
                        <div className='flex items-center space-x-4 text-xs text-gray-500'>
                          <span>
                            {event.startTime.toLocaleDateString()}{' '}
                            {event.startTime.toLocaleTimeString()}
                          </span>
                          {event.location && <span>📍 {event.location}</span>}
                          {event.attendees && event.attendees.length > 0 && (
                            <span>👥 {event.attendees.length} attendees</span>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className='text-center py-8 text-gray-500'>
                  <CalendarIcon className='h-8 w-8 mx-auto mb-2 text-gray-400' />
                  <p>
                    No calendar events found. Connect your calendars to get
                    started.
                  </p>
                </div>
              )}
            </div>
          </div>
        )}

        {activeTab === 'conflicts' && (
          <div>
            <h4 className='text-sm font-medium text-gray-700 mb-3'>
              Calendar Conflicts
            </h4>
            <div className='space-y-3'>
              {conflicts.length > 0 ? (
                conflicts.map((conflict, index) => (
                  <div
                    key={index}
                    className='p-3 bg-red-50 border border-red-200 rounded-lg'
                  >
                    <div className='flex items-start justify-between'>
                      <div className='flex-1'>
                        <div className='flex items-center space-x-2 mb-1'>
                          <ExclamationTriangleIcon className='h-4 w-4 text-red-500' />
                          <span
                            className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getConflictSeverityColor(
                              conflict.severity
                            )}`}
                          >
                            {conflict.severity.toUpperCase()}{' '}
                            {conflict.conflictType}
                          </span>
                        </div>
                        <p className='text-sm text-gray-900 mb-1'>
                          Conflict between events
                        </p>
                        <div className='text-xs text-gray-600'>
                          <p>Event 1: {conflict.conflictingEvent.title}</p>
                          <p>Event 2: {conflict.conflictingEvent.title}</p>
                        </div>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className='text-center py-8 text-gray-500'>
                  <CheckCircleIcon className='h-8 w-8 mx-auto mb-2 text-green-500' />
                  <p>No calendar conflicts detected.</p>
                </div>
              )}
            </div>
          </div>
        )}

        {activeTab === 'settings' && (
          <div>
            <h4 className='text-sm font-medium text-gray-700 mb-3'>
              Sync Settings
            </h4>
            <div className='space-y-4'>
              <div>
                <label className='flex items-center'>
                  <input
                    type='checkbox'
                    checked={syncSettings.syncTasks}
                    onChange={(e) =>
                      handleUpdateSettings({ syncTasks: e.target.checked })
                    }
                    className='mr-2'
                  />
                  <span className='text-sm text-gray-700'>
                    Sync tasks to calendar
                  </span>
                </label>
              </div>

              <div>
                <label className='flex items-center'>
                  <input
                    type='checkbox'
                    checked={syncSettings.syncBookings}
                    onChange={(e) =>
                      handleUpdateSettings({ syncBookings: e.target.checked })
                    }
                    className='mr-2'
                  />
                  <span className='text-sm text-gray-700'>
                    Sync bookings to calendar
                  </span>
                </label>
              </div>

              <div>
                <label className='flex items-center'>
                  <input
                    type='checkbox'
                    checked={syncSettings.autoSync}
                    onChange={(e) =>
                      handleUpdateSettings({ autoSync: e.target.checked })
                    }
                    className='mr-2'
                  />
                  <span className='text-sm text-gray-700'>
                    Auto-sync every 15 minutes
                  </span>
                </label>
              </div>

              <div>
                <label className='block text-sm font-medium text-gray-700 mb-1'>
                  Sync Direction
                </label>
                <select
                  value={syncSettings.syncDirection}
                  onChange={(e) =>
                    handleUpdateSettings({
                      syncDirection: e.target.value as 'one-way' | 'two-way',
                    })
                  }
                  className='input-field'
                >
                  <option value='one-way'>One-way (Axion → Calendar)</option>
                  <option value='two-way'>Two-way (Axion ↔ Calendar)</option>
                </select>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
