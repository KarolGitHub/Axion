import React, { useState, useEffect } from 'react';
import {
  PlusIcon,
  PencilIcon,
  TrashIcon,
  CalendarIcon,
  ClockIcon,
  MapPinIcon,
} from '@heroicons/react/24/outline';
import { Resource, Booking } from '../types';
import {
  resourceService,
  bookingService,
  CreateResourceRequest,
  UpdateResourceRequest,
  CreateBookingRequest,
} from '../services/resourceService';
import { useAuthStore } from '../state/authStore';

const resourceTypeColors = {
  'Meeting Room': 'bg-blue-100 text-blue-800',
  Desk: 'bg-green-100 text-green-800',
  Equipment: 'bg-purple-100 text-purple-800',
};

export const Resources: React.FC = () => {
  const [resources, setResources] = useState<Resource[]>([]);
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showBookingModal, setShowBookingModal] = useState(false);
  const [selectedResource, setSelectedResource] = useState<Resource | null>(
    null
  );
  const [resourceFormData, setResourceFormData] =
    useState<CreateResourceRequest>({
      name: '',
      type: 'Meeting Room',
      capacity: undefined,
      location: '',
      isAvailable: true,
    });
  const [bookingFormData, setBookingFormData] = useState<CreateBookingRequest>({
    resourceId: '',
    userId: '',
    startTime: '',
    endTime: '',
    purpose: '',
  });
  const [submitting, setSubmitting] = useState(false);

  const { user } = useAuthStore();

  // Load resources and bookings on component mount
  useEffect(() => {
    loadData();
  }, []);

  // Set user ID when user is available
  useEffect(() => {
    if (user) {
      setBookingFormData((prev) => ({ ...prev, userId: user.id }));
    }
  }, [user]);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [resourcesData, bookingsData] = await Promise.all([
        resourceService.getAll(),
        bookingService.getAll(),
      ]);
      setResources(resourcesData);
      setBookings(bookingsData);
    } catch (err) {
      setError('Failed to load resources');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateResource = () => {
    setResourceFormData({
      name: '',
      type: 'Meeting Room',
      capacity: undefined,
      location: '',
      isAvailable: true,
    });
    setSelectedResource(null);
    setShowCreateModal(true);
  };

  const handleEditResource = (resource: Resource) => {
    setResourceFormData({
      name: resource.name,
      type: resource.type,
      capacity: resource.capacity || undefined,
      location: resource.location,
      isAvailable: resource.isAvailable,
    });
    setSelectedResource(resource);
    setShowCreateModal(true);
  };

  const handleDeleteResource = async (resourceId: string) => {
    if (window.confirm('Are you sure you want to delete this resource?')) {
      try {
        await resourceService.delete(resourceId);
        setResources(resources.filter((r) => r.id !== resourceId));
      } catch (err) {
        setError('Failed to delete resource');
        console.error('Error deleting resource:', err);
      }
    }
  };

  const handleBookResource = (resource: Resource) => {
    setBookingFormData({
      resourceId: resource.id,
      userId: user?.id || '',
      startTime: '',
      endTime: '',
      purpose: '',
    });
    setSelectedResource(resource);
    setShowBookingModal(true);
  };

  const handleResourceSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!resourceFormData.name.trim()) return;

    try {
      setSubmitting(true);
      setError(null);

      if (selectedResource) {
        // Update existing resource
        const updateData: UpdateResourceRequest = {
          name: resourceFormData.name,
          type: resourceFormData.type,
          capacity: resourceFormData.capacity,
          location: resourceFormData.location,
          isAvailable: resourceFormData.isAvailable,
        };
        await resourceService.update(selectedResource.id, updateData);

        // Update local state
        setResources(
          resources.map((r) =>
            r.id === selectedResource.id ? { ...r, ...updateData } : r
          )
        );
      } else {
        // Create new resource
        const newResource = await resourceService.create(resourceFormData);
        setResources([...resources, newResource]);
      }

      setShowCreateModal(false);
    } catch (err) {
      setError(
        selectedResource
          ? 'Failed to update resource'
          : 'Failed to create resource'
      );
      console.error('Error saving resource:', err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleBookingSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (
      !bookingFormData.purpose.trim() ||
      !bookingFormData.startTime ||
      !bookingFormData.endTime
    )
      return;

    try {
      setSubmitting(true);
      setError(null);

      const newBooking = await bookingService.create(bookingFormData);
      setBookings([...bookings, newBooking]);
      setShowBookingModal(false);
    } catch (err) {
      setError('Failed to create booking');
      console.error('Error creating booking:', err);
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
    setResourceFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleBookingInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setBookingFormData((prev) => ({ ...prev, [name]: value }));
  };

  const getResourceBookings = (resourceId: string) => {
    return bookings.filter((booking) => booking.resourceId === resourceId);
  };

  const isResourceAvailable = (resource: Resource) => {
    const resourceBookings = getResourceBookings(resource.id);
    const now = new Date();
    const hasActiveBooking = resourceBookings.some((booking) => {
      const start = new Date(booking.startTime);
      const end = new Date(booking.endTime);
      return now >= start && now <= end;
    });
    return resource.isAvailable && !hasActiveBooking;
  };

  if (loading) {
    return (
      <div className='space-y-6'>
        <div className='flex items-center justify-between'>
          <div>
            <h1 className='text-3xl font-bold text-gray-900'>Resources</h1>
            <p className='mt-2 text-gray-600'>Loading resources...</p>
          </div>
        </div>
        <div className='grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3'>
          {[1, 2, 3, 4, 5, 6].map((i) => (
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
          <h1 className='text-3xl font-bold text-gray-900'>Resources</h1>
          <p className='mt-2 text-gray-600'>
            Book and manage shared resources like meeting rooms and equipment.
          </p>
        </div>
        <button
          onClick={handleCreateResource}
          className='btn-primary flex items-center'
        >
          <PlusIcon className='h-5 w-5 mr-2' />
          Add Resource
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
                <span className='text-white font-medium'>T</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>
                Total Resources
              </p>
              <p className='text-2xl font-semibold text-gray-900'>
                {resources.length}
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-green-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>A</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Available</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {resources.filter((r) => isResourceAvailable(r)).length}
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-yellow-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>B</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Booked Today</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {
                  bookings.filter((b) => {
                    const today = new Date().toDateString();
                    const bookingDate = new Date(b.startTime).toDateString();
                    return bookingDate === today;
                  }).length
                }
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-purple-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>M</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Meeting Rooms</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {resources.filter((r) => r.type === 'Meeting Room').length}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Resources Grid */}
      <div className='grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3'>
        {resources.map((resource) => (
          <div key={resource.id} className='card'>
            <div className='flex items-start justify-between mb-4'>
              <div>
                <h3 className='text-lg font-medium text-gray-900'>
                  {resource.name}
                </h3>
                <span
                  className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full mt-1 ${
                    resourceTypeColors[
                      resource.type as keyof typeof resourceTypeColors
                    ]
                  }`}
                >
                  {resource.type}
                </span>
              </div>
              <div className='flex space-x-1'>
                <button
                  onClick={() => handleEditResource(resource)}
                  className='text-indigo-600 hover:text-indigo-900'
                  title='Edit Resource'
                >
                  <PencilIcon className='h-4 w-4' />
                </button>
                <button
                  onClick={() => handleDeleteResource(resource.id)}
                  className='text-red-600 hover:text-red-900'
                  title='Delete Resource'
                >
                  <TrashIcon className='h-4 w-4' />
                </button>
              </div>
            </div>

            <div className='space-y-3'>
              <div className='flex items-center text-sm text-gray-500'>
                <MapPinIcon className='h-4 w-4 mr-2' />
                {resource.location}
              </div>

              {resource.capacity && (
                <div className='flex items-center text-sm text-gray-500'>
                  <span className='mr-2'>Capacity:</span>
                  <span className='font-medium'>
                    {resource.capacity} people
                  </span>
                </div>
              )}

              <div className='flex items-center justify-between'>
                <span
                  className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                    isResourceAvailable(resource)
                      ? 'bg-green-100 text-green-800'
                      : 'bg-red-100 text-red-800'
                  }`}
                >
                  {isResourceAvailable(resource) ? 'Available' : 'Occupied'}
                </span>

                <button
                  onClick={() => handleBookResource(resource)}
                  disabled={!isResourceAvailable(resource)}
                  className={`px-3 py-1 text-xs font-medium rounded-md ${
                    isResourceAvailable(resource)
                      ? 'bg-blue-600 text-white hover:bg-blue-700'
                      : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                  }`}
                >
                  Book Now
                </button>
              </div>
            </div>

            {/* Recent Bookings */}
            <div className='mt-4 pt-4 border-t border-gray-200'>
              <h4 className='text-sm font-medium text-gray-900 mb-2'>
                Recent Bookings
              </h4>
              <div className='space-y-2'>
                {getResourceBookings(resource.id)
                  .slice(0, 2)
                  .map((booking) => (
                    <div key={booking.id} className='text-xs text-gray-500'>
                      <div className='flex items-center'>
                        <CalendarIcon className='h-3 w-3 mr-1' />
                        {new Date(booking.startTime).toLocaleDateString()}
                      </div>
                      <div className='flex items-center'>
                        <ClockIcon className='h-3 w-3 mr-1' />
                        {new Date(booking.startTime).toLocaleTimeString([], {
                          hour: '2-digit',
                          minute: '2-digit',
                        })}{' '}
                        -{' '}
                        {new Date(booking.endTime).toLocaleTimeString([], {
                          hour: '2-digit',
                          minute: '2-digit',
                        })}
                      </div>
                      <div className='truncate'>{booking.purpose}</div>
                    </div>
                  ))}
                {getResourceBookings(resource.id).length === 0 && (
                  <div className='text-xs text-gray-400'>
                    No recent bookings
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {resources.length === 0 && (
        <div className='text-center py-8 text-gray-500'>
          No resources found. Add your first resource to get started.
        </div>
      )}

      {/* Create/Edit Resource Modal */}
      {showCreateModal && (
        <div className='fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50'>
          <div className='relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white'>
            <div className='mt-3'>
              <h3 className='text-lg font-medium text-gray-900 mb-4'>
                {selectedResource ? 'Edit Resource' : 'Add New Resource'}
              </h3>

              <form onSubmit={handleResourceSubmit} className='space-y-4'>
                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Resource Name *
                  </label>
                  <input
                    type='text'
                    name='name'
                    value={resourceFormData.name}
                    onChange={handleInputChange}
                    className='input-field'
                    required
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Type
                  </label>
                  <select
                    name='type'
                    value={resourceFormData.type}
                    onChange={handleInputChange}
                    className='input-field'
                  >
                    <option value='Meeting Room'>Meeting Room</option>
                    <option value='Desk'>Desk</option>
                    <option value='Equipment'>Equipment</option>
                  </select>
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Location *
                  </label>
                  <input
                    type='text'
                    name='location'
                    value={resourceFormData.location}
                    onChange={handleInputChange}
                    className='input-field'
                    required
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Capacity
                  </label>
                  <input
                    type='number'
                    name='capacity'
                    value={resourceFormData.capacity || ''}
                    onChange={handleInputChange}
                    className='input-field'
                    placeholder='Number of people'
                  />
                </div>

                <div className='flex items-center'>
                  <input
                    type='checkbox'
                    name='isAvailable'
                    checked={resourceFormData.isAvailable}
                    onChange={(e) =>
                      setResourceFormData((prev) => ({
                        ...prev,
                        isAvailable: e.target.checked,
                      }))
                    }
                    className='h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded'
                  />
                  <label className='ml-2 block text-sm text-gray-700'>
                    Available for booking
                  </label>
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
                      : selectedResource
                      ? 'Update'
                      : 'Create'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Booking Modal */}
      {showBookingModal && (
        <div className='fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50'>
          <div className='relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white'>
            <div className='mt-3'>
              <h3 className='text-lg font-medium text-gray-900 mb-4'>
                Book Resource: {selectedResource?.name}
              </h3>

              <form onSubmit={handleBookingSubmit} className='space-y-4'>
                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Purpose *
                  </label>
                  <input
                    type='text'
                    name='purpose'
                    value={bookingFormData.purpose}
                    onChange={handleBookingInputChange}
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
                    value={bookingFormData.startTime}
                    onChange={handleBookingInputChange}
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
                    value={bookingFormData.endTime}
                    onChange={handleBookingInputChange}
                    className='input-field'
                    required
                  />
                </div>

                <div className='flex justify-end space-x-3 pt-4'>
                  <button
                    type='button'
                    onClick={() => setShowBookingModal(false)}
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
                    {submitting ? 'Booking...' : 'Book Resource'}
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
