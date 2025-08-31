import React, { useState, useEffect } from 'react';
import {
  PlusIcon,
  PencilIcon,
  TrashIcon,
  CalendarIcon,
  ClockIcon,
  UserIcon,
  MapPinIcon,
} from '@heroicons/react/24/outline';
import { Booking, Resource, User } from '../types';
import {
  bookingService,
  CreateBookingRequest,
  UpdateBookingRequest,
} from '../services/bookingService';
import { resourceService } from '../services/resourceService';
import { useAuthStore } from '../state/authStore';

const bookingStatusColors = {
  upcoming: 'bg-blue-100 text-blue-800',
  ongoing: 'bg-green-100 text-green-800',
  completed: 'bg-gray-100 text-gray-800',
  cancelled: 'bg-red-100 text-red-800',
};

export const Bookings: React.FC = () => {
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [resources, setResources] = useState<Resource[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [selectedBooking, setSelectedBooking] = useState<Booking | null>(null);
  const [formData, setFormData] = useState<CreateBookingRequest>({
    resourceId: '',
    userId: '',
    startTime: '',
    endTime: '',
    purpose: '',
  });
  const [submitting, setSubmitting] = useState(false);

  const { user } = useAuthStore();

  // Load bookings and resources on component mount
  useEffect(() => {
    loadData();
  }, []);

  // Set user ID when user is available
  useEffect(() => {
    if (user) {
      setFormData((prev) => ({ ...prev, userId: user.id }));
    }
  }, [user]);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [bookingsData, resourcesData] = await Promise.all([
        bookingService.getAll(),
        resourceService.getAll(),
      ]);
      setBookings(bookingsData);
      setResources(resourcesData);
    } catch (err) {
      setError('Failed to load bookings');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateBooking = () => {
    setFormData({
      resourceId: '',
      userId: user?.id || '',
      startTime: '',
      endTime: '',
      purpose: '',
    });
    setSelectedBooking(null);
    setShowCreateModal(true);
  };

  const handleEditBooking = (booking: Booking) => {
    setFormData({
      resourceId: booking.resourceId,
      userId: booking.userId,
      startTime: new Date(booking.startTime).toISOString().slice(0, 16),
      endTime: new Date(booking.endTime).toISOString().slice(0, 16),
      purpose: booking.purpose,
    });
    setSelectedBooking(booking);
    setShowCreateModal(true);
  };

  const handleDeleteBooking = async (bookingId: string) => {
    if (window.confirm('Are you sure you want to delete this booking?')) {
      try {
        await bookingService.delete(bookingId);
        setBookings(bookings.filter((b) => b.id !== bookingId));
      } catch (err) {
        setError('Failed to delete booking');
        console.error('Error deleting booking:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (
      !formData.purpose.trim() ||
      !formData.startTime ||
      !formData.endTime ||
      !formData.resourceId
    )
      return;

    try {
      setSubmitting(true);
      setError(null);

      if (selectedBooking) {
        // Update existing booking
        const updateData: UpdateBookingRequest = {
          resourceId: formData.resourceId,
          userId: formData.userId,
          startTime: formData.startTime,
          endTime: formData.endTime,
          purpose: formData.purpose,
        };
        await bookingService.update(selectedBooking.id, updateData);

        // Update local state
        setBookings(
          bookings.map((b) =>
            b.id === selectedBooking.id ? { ...b, ...updateData } : b
          )
        );
      } else {
        // Create new booking
        const newBooking = await bookingService.create(formData);
        setBookings([...bookings, newBooking]);
      }

      setShowCreateModal(false);
    } catch (err) {
      setError(
        selectedBooking
          ? 'Failed to update booking'
          : 'Failed to create booking'
      );
      console.error('Error saving booking:', err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleInputChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const getBookingStatus = (booking: Booking) => {
    const now = new Date();
    const start = new Date(booking.startTime);
    const end = new Date(booking.endTime);

    if (now < start) return 'upcoming';
    if (now >= start && now <= end) return 'ongoing';
    if (now > end) return 'completed';
    return 'upcoming';
  };

  const getResourceName = (resourceId: string) => {
    const resource = resources.find((r) => r.id === resourceId);
    return resource?.name || 'Unknown Resource';
  };

  const getResourceType = (resourceId: string) => {
    const resource = resources.find((r) => r.id === resourceId);
    return resource?.type || 'Unknown';
  };

  const getResourceLocation = (resourceId: string) => {
    const resource = resources.find((r) => r.id === resourceId);
    return resource?.location || 'Unknown Location';
  };

  const formatDateTime = (dateTime: string) => {
    return new Date(dateTime).toLocaleString();
  };

  const formatDate = (dateTime: string) => {
    return new Date(dateTime).toLocaleDateString();
  };

  const formatTime = (dateTime: string) => {
    return new Date(dateTime).toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const isBookingEditable = (booking: Booking) => {
    const now = new Date();
    const start = new Date(booking.startTime);
    return now < start; // Can only edit future bookings
  };

  if (loading) {
    return (
      <div className='space-y-6'>
        <div className='flex items-center justify-between'>
          <div>
            <h1 className='text-3xl font-bold text-gray-900'>Bookings</h1>
            <p className='mt-2 text-gray-600'>Loading bookings...</p>
          </div>
        </div>
        <div className='space-y-4'>
          {[1, 2, 3, 4, 5].map((i) => (
            <div key={i} className='card'>
              <div className='animate-pulse'>
                <div className='h-4 bg-gray-200 rounded w-3/4 mb-4'></div>
                <div className='space-y-3'>
                  <div className='h-3 bg-gray-200 rounded w-1/2'></div>
                  <div className='h-3 bg-gray-200 rounded w-2/3'></div>
                  <div className='h-3 bg-gray-200 rounded w-1/3'></div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className='space-y-6'>
      {/* Header */}
      <div className='flex items-center justify-between'>
        <div>
          <h1 className='text-3xl font-bold text-gray-900'>Bookings</h1>
          <p className='mt-2 text-gray-600'>
            Manage and track all resource bookings.
          </p>
        </div>
        <button
          onClick={handleCreateBooking}
          className='btn-primary flex items-center'
        >
          <PlusIcon className='h-5 w-5 mr-2' />
          New Booking
        </button>
      </div>

      {/* Error Message */}
      {error && (
        <div className='bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded'>
          {error}
        </div>
      )}

      {/* Stats */}
      <div className='grid grid-cols-1 gap-6 sm:grid-cols-4'>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-blue-500 rounded-full flex items-center justify-center'>
                <CalendarIcon className='h-5 w-5 text-white' />
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>
                Total Bookings
              </p>
              <p className='text-2xl font-semibold text-gray-900'>
                {bookings.length}
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-green-500 rounded-full flex items-center justify-center'>
                <ClockIcon className='h-5 w-5 text-white' />
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Upcoming</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {
                  bookings.filter((b) => getBookingStatus(b) === 'upcoming')
                    .length
                }
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-yellow-500 rounded-full flex items-center justify-center'>
                <UserIcon className='h-5 w-5 text-white' />
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Ongoing</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {
                  bookings.filter((b) => getBookingStatus(b) === 'ongoing')
                    .length
                }
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-gray-500 rounded-full flex items-center justify-center'>
                <CalendarIcon className='h-5 w-5 text-white' />
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Completed</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {
                  bookings.filter((b) => getBookingStatus(b) === 'completed')
                    .length
                }
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Bookings List */}
      <div className='space-y-4'>
        {bookings.map((booking) => (
          <div key={booking.id} className='card'>
            <div className='flex items-start justify-between'>
              <div className='flex-1'>
                <div className='flex items-center justify-between mb-2'>
                  <h3 className='text-lg font-medium text-gray-900'>
                    {booking.purpose}
                  </h3>
                  <span
                    className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                      bookingStatusColors[
                        getBookingStatus(
                          booking
                        ) as keyof typeof bookingStatusColors
                      ]
                    }`}
                  >
                    {getBookingStatus(booking).charAt(0).toUpperCase() +
                      getBookingStatus(booking).slice(1)}
                  </span>
                </div>

                <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                  <div className='space-y-2'>
                    <div className='flex items-center text-sm text-gray-500'>
                      <MapPinIcon className='h-4 w-4 mr-2' />
                      {getResourceName(booking.resourceId)} (
                      {getResourceType(booking.resourceId)})
                    </div>
                    <div className='flex items-center text-sm text-gray-500'>
                      <CalendarIcon className='h-4 w-4 mr-2' />
                      {formatDate(booking.startTime)}
                    </div>
                    <div className='flex items-center text-sm text-gray-500'>
                      <ClockIcon className='h-4 w-4 mr-2' />
                      {formatTime(booking.startTime)} -{' '}
                      {formatTime(booking.endTime)}
                    </div>
                  </div>
                  <div className='space-y-2'>
                    <div className='flex items-center text-sm text-gray-500'>
                      <UserIcon className='h-4 w-4 mr-2' />
                      {booking.userId === user?.id
                        ? 'You'
                        : `User ${booking.userId}`}
                    </div>
                    <div className='text-sm text-gray-500'>
                      Created: {formatDateTime(booking.createdAt)}
                    </div>
                  </div>
                </div>
              </div>

              <div className='flex space-x-2 ml-4'>
                {isBookingEditable(booking) && (
                  <>
                    <button
                      onClick={() => handleEditBooking(booking)}
                      className='text-indigo-600 hover:text-indigo-900'
                      title='Edit Booking'
                    >
                      <PencilIcon className='h-4 w-4' />
                    </button>
                    <button
                      onClick={() => handleDeleteBooking(booking.id)}
                      className='text-red-600 hover:text-red-900'
                      title='Delete Booking'
                    >
                      <TrashIcon className='h-4 w-4' />
                    </button>
                  </>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {bookings.length === 0 && (
        <div className='text-center py-8 text-gray-500'>
          No bookings found. Create your first booking to get started.
        </div>
      )}

      {/* Create/Edit Booking Modal */}
      {showCreateModal && (
        <div className='fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50'>
          <div className='relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white'>
            <div className='mt-3'>
              <h3 className='text-lg font-medium text-gray-900 mb-4'>
                {selectedBooking ? 'Edit Booking' : 'Create New Booking'}
              </h3>

              <form onSubmit={handleSubmit} className='space-y-4'>
                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Resource *
                  </label>
                  <select
                    name='resourceId'
                    value={formData.resourceId}
                    onChange={handleInputChange}
                    className='input-field'
                    required
                  >
                    <option value=''>Select a resource</option>
                    {resources.map((resource) => (
                      <option key={resource.id} value={resource.id}>
                        {resource.name} ({resource.type}) - {resource.location}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Purpose *
                  </label>
                  <input
                    type='text'
                    name='purpose'
                    value={formData.purpose}
                    onChange={handleInputChange}
                    className='input-field'
                    placeholder='Meeting purpose'
                    required
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Start Time *
                  </label>
                  <input
                    type='datetime-local'
                    name='startTime'
                    value={formData.startTime}
                    onChange={handleInputChange}
                    className='input-field'
                    required
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    End Time *
                  </label>
                  <input
                    type='datetime-local'
                    name='endTime'
                    value={formData.endTime}
                    onChange={handleInputChange}
                    className='input-field'
                    required
                  />
                </div>

                <div className='flex justify-end space-x-3 pt-4'>
                  <button
                    type='button'
                    onClick={() => setShowCreateModal(false)}
                    className='btn-secondary'
                    disabled={submitting}
                  >
                    Cancel
                  </button>
                  <button
                    type='submit'
                    className='btn-primary'
                    disabled={submitting}
                  >
                    {submitting
                      ? 'Saving...'
                      : selectedBooking
                      ? 'Update'
                      : 'Create'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
