import { Task, Project, Booking } from '../types';

export interface CalendarEvent {
  id: string;
  title: string;
  description?: string;
  startTime: Date;
  endTime: Date;
  location?: string;
  attendees?: string[];
  isAllDay: boolean;
  source: 'google' | 'outlook' | 'local';
  sourceId?: string;
}

export interface CalendarSyncSettings {
  googleCalendarId?: string;
  outlookCalendarId?: string;
  syncTasks: boolean;
  syncBookings: boolean;
  syncDirection: 'one-way' | 'two-way';
  autoSync: boolean;
}

export interface CalendarConflict {
  eventId: string;
  conflictingEvent: CalendarEvent;
  conflictType: 'overlap' | 'double-booking' | 'time-conflict';
  severity: 'low' | 'medium' | 'high';
}

class CalendarService {
  private syncSettings: CalendarSyncSettings = {
    syncTasks: true,
    syncBookings: true,
    syncDirection: 'two-way',
    autoSync: true,
  };

  // Initialize calendar integration
  async initializeCalendar(provider: 'google' | 'outlook'): Promise<boolean> {
    try {
      if (provider === 'google') {
        return await this.initializeGoogleCalendar();
      } else {
        return await this.initializeOutlookCalendar();
      }
    } catch (error) {
      console.error(`Failed to initialize ${provider} calendar:`, error);
      return false;
    }
  }

  // Google Calendar Integration
  private async initializeGoogleCalendar(): Promise<boolean> {
    // Simulate Google Calendar API initialization
    await new Promise((resolve) => setTimeout(resolve, 1000));

    // In a real implementation, this would:
    // 1. Check if user is authenticated with Google
    // 2. Request calendar access permissions
    // 3. Get user's calendar list
    // 4. Store calendar IDs for sync

    console.log('Google Calendar initialized');
    return true;
  }

  // Outlook Calendar Integration
  private async initializeOutlookCalendar(): Promise<boolean> {
    // Simulate Outlook Calendar API initialization
    await new Promise((resolve) => setTimeout(resolve, 1000));

    // In a real implementation, this would:
    // 1. Check if user is authenticated with Microsoft
    // 2. Request calendar access permissions
    // 3. Get user's calendar list
    // 4. Store calendar IDs for sync

    console.log('Outlook Calendar initialized');
    return true;
  }

  // Sync tasks to calendar
  async syncTasksToCalendar(
    tasks: Task[],
    projects: Project[]
  ): Promise<CalendarEvent[]> {
    const events: CalendarEvent[] = [];

    for (const task of tasks) {
      if (task.dueDate && task.status !== 'Done') {
        const project = projects.find((p) => p.id === task.projectId);
        const event: CalendarEvent = {
          id: `task-${task.id}`,
          title: `Task: ${task.title}`,
          description: `${task.description}\n\nProject: ${
            project?.name || 'Unknown Project'
          }\nPriority: ${task.priority}`,
          startTime: new Date(task.dueDate),
          endTime: new Date(new Date(task.dueDate).getTime() + 60 * 60 * 1000), // 1 hour duration
          location: project?.name,
          isAllDay: false,
          source: 'local',
        };

        events.push(event);
      }
    }

    return events;
  }

  // Sync bookings to calendar
  async syncBookingsToCalendar(bookings: Booking[]): Promise<CalendarEvent[]> {
    const events: CalendarEvent[] = [];

    for (const booking of bookings) {
      const event: CalendarEvent = {
        id: `booking-${booking.id}`,
        title: `Booking: ${booking.purpose}`,
        description: `Resource: ${booking.resourceName}\nLocation: ${booking.resourceLocation}`,
        startTime: new Date(booking.startTime),
        endTime: new Date(booking.endTime),
        location: booking.resourceLocation,
        attendees: [booking.userName],
        isAllDay: false,
        source: 'local',
      };

      events.push(event);
    }

    return events;
  }

  // Get calendar events from external providers
  async getExternalCalendarEvents(
    provider: 'google' | 'outlook',
    startDate: Date,
    endDate: Date
  ): Promise<CalendarEvent[]> {
    // Simulate API call delay
    await new Promise((resolve) => setTimeout(resolve, 800));

    // Mock external calendar events
    const mockEvents: CalendarEvent[] = [
      {
        id: 'ext-1',
        title: 'Team Meeting',
        description: 'Weekly team sync',
        startTime: new Date(startDate.getTime() + 2 * 60 * 60 * 1000), // 2 hours from start
        endTime: new Date(startDate.getTime() + 3 * 60 * 60 * 1000), // 3 hours from start
        location: 'Conference Room A',
        attendees: ['john@company.com', 'jane@company.com'],
        isAllDay: false,
        source: provider,
        sourceId: 'ext-1',
      },
      {
        id: 'ext-2',
        title: 'Client Call',
        description: 'Project review with client',
        startTime: new Date(startDate.getTime() + 5 * 60 * 60 * 1000), // 5 hours from start
        endTime: new Date(startDate.getTime() + 6 * 60 * 60 * 1000), // 6 hours from start
        location: 'Zoom',
        attendees: ['client@client.com'],
        isAllDay: false,
        source: provider,
        sourceId: 'ext-2',
      },
    ];

    return mockEvents;
  }

  // Create calendar event
  async createCalendarEvent(
    event: Omit<CalendarEvent, 'id'>,
    provider?: 'google' | 'outlook'
  ): Promise<CalendarEvent> {
    // Simulate API call delay
    await new Promise((resolve) => setTimeout(resolve, 500));

    const newEvent: CalendarEvent = {
      ...event,
      id: `event-${Date.now()}`,
      source: provider || 'local',
    };

    console.log(`Created calendar event: ${newEvent.title}`);
    return newEvent;
  }

  // Update calendar event
  async updateCalendarEvent(
    eventId: string,
    updates: Partial<CalendarEvent>
  ): Promise<CalendarEvent> {
    // Simulate API call delay
    await new Promise((resolve) => setTimeout(resolve, 500));

    console.log(`Updated calendar event: ${eventId}`);
    return { id: eventId, ...updates } as CalendarEvent;
  }

  // Delete calendar event
  async deleteCalendarEvent(eventId: string): Promise<boolean> {
    // Simulate API call delay
    await new Promise((resolve) => setTimeout(resolve, 300));

    console.log(`Deleted calendar event: ${eventId}`);
    return true;
  }

  // Check for calendar conflicts
  async checkConflicts(events: CalendarEvent[]): Promise<CalendarConflict[]> {
    const conflicts: CalendarConflict[] = [];

    for (let i = 0; i < events.length; i++) {
      for (let j = i + 1; j < events.length; j++) {
        const event1 = events[i];
        const event2 = events[j];

        // Check for time overlap
        if (this.hasTimeOverlap(event1, event2)) {
          const conflict: CalendarConflict = {
            eventId: event1.id,
            conflictingEvent: event2,
            conflictType: 'overlap',
            severity: this.calculateConflictSeverity(event1, event2),
          };
          conflicts.push(conflict);
        }

        // Check for location conflicts (double booking)
        if (
          event1.location &&
          event2.location &&
          event1.location === event2.location &&
          this.hasTimeOverlap(event1, event2)
        ) {
          const conflict: CalendarConflict = {
            eventId: event1.id,
            conflictingEvent: event2,
            conflictType: 'double-booking',
            severity: 'high',
          };
          conflicts.push(conflict);
        }
      }
    }

    return conflicts;
  }

  // Sync all data with external calendars
  async syncWithExternalCalendars(
    tasks: Task[],
    bookings: Booking[],
    projects: Project[]
  ): Promise<{
    syncedEvents: CalendarEvent[];
    conflicts: CalendarConflict[];
  }> {
    const startDate = new Date();
    const endDate = new Date(startDate.getTime() + 30 * 24 * 60 * 60 * 1000); // 30 days

    // Get local events
    const localTaskEvents = await this.syncTasksToCalendar(tasks, projects);
    const localBookingEvents = await this.syncBookingsToCalendar(bookings);
    const localEvents = [...localTaskEvents, ...localBookingEvents];

    // Get external events
    const googleEvents = await this.getExternalCalendarEvents(
      'google',
      startDate,
      endDate
    );
    const outlookEvents = await this.getExternalCalendarEvents(
      'outlook',
      startDate,
      endDate
    );
    const externalEvents = [...googleEvents, ...outlookEvents];

    // Combine all events
    const allEvents = [...localEvents, ...externalEvents];

    // Check for conflicts
    const conflicts = await this.checkConflicts(allEvents);

    return {
      syncedEvents: allEvents,
      conflicts,
    };
  }

  // Get calendar settings
  getSyncSettings(): CalendarSyncSettings {
    return { ...this.syncSettings };
  }

  // Update calendar settings
  async updateSyncSettings(
    settings: Partial<CalendarSyncSettings>
  ): Promise<void> {
    this.syncSettings = { ...this.syncSettings, ...settings };

    // Save settings to localStorage or backend
    localStorage.setItem(
      'calendarSyncSettings',
      JSON.stringify(this.syncSettings)
    );
  }

  // Export calendar events to different formats
  async exportCalendarEvents(
    events: CalendarEvent[],
    format: 'ics' | 'csv' | 'json'
  ): Promise<string> {
    switch (format) {
      case 'ics':
        return this.generateICSFile(events);
      case 'csv':
        return this.generateCSVFile(events);
      case 'json':
        return JSON.stringify(events, null, 2);
      default:
        throw new Error(`Unsupported format: ${format}`);
    }
  }

  // Generate ICS file
  private generateICSFile(events: CalendarEvent[]): string {
    let ics = 'BEGIN:VCALENDAR\nVERSION:2.0\nPRODID:-//Axion//Calendar//EN\n';

    for (const event of events) {
      ics += `BEGIN:VEVENT\n`;
      ics += `UID:${event.id}\n`;
      ics += `DTSTART:${this.formatDateForICS(event.startTime)}\n`;
      ics += `DTEND:${this.formatDateForICS(event.endTime)}\n`;
      ics += `SUMMARY:${event.title}\n`;
      if (event.description) {
        ics += `DESCRIPTION:${event.description.replace(/\n/g, '\\n')}\n`;
      }
      if (event.location) {
        ics += `LOCATION:${event.location}\n`;
      }
      ics += `END:VEVENT\n`;
    }

    ics += 'END:VCALENDAR';
    return ics;
  }

  // Generate CSV file
  private generateCSVFile(events: CalendarEvent[]): string {
    const headers = [
      'Title',
      'Description',
      'Start Time',
      'End Time',
      'Location',
      'Attendees',
      'Source',
    ];
    const rows = events.map((event) => [
      event.title,
      event.description || '',
      event.startTime.toISOString(),
      event.endTime.toISOString(),
      event.location || '',
      event.attendees?.join('; ') || '',
      event.source,
    ]);

    return [headers, ...rows]
      .map((row) => row.map((cell) => `"${cell}"`).join(','))
      .join('\n');
  }

  // Helper methods
  private hasTimeOverlap(
    event1: CalendarEvent,
    event2: CalendarEvent
  ): boolean {
    return (
      event1.startTime < event2.endTime && event2.startTime < event1.endTime
    );
  }

  private calculateConflictSeverity(
    event1: CalendarEvent,
    event2: CalendarEvent
  ): 'low' | 'medium' | 'high' {
    const overlapDuration =
      Math.min(event1.endTime.getTime(), event2.endTime.getTime()) -
      Math.max(event1.startTime.getTime(), event2.startTime.getTime());

    const event1Duration =
      event1.endTime.getTime() - event1.startTime.getTime();
    const overlapPercentage = (overlapDuration / event1Duration) * 100;

    if (overlapPercentage > 50) return 'high';
    if (overlapPercentage > 25) return 'medium';
    return 'low';
  }

  private formatDateForICS(date: Date): string {
    return date
      .toISOString()
      .replace(/[-:]/g, '')
      .replace(/\.\d{3}/, '');
  }
}

export const calendarService = new CalendarService();
