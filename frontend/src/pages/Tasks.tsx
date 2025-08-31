import React, { useState, useEffect } from 'react';
import {
  PlusIcon,
  PencilIcon,
  TrashIcon,
  UserIcon,
  CalendarIcon,
} from '@heroicons/react/24/outline';
import { Task } from '../types';
import {
  taskService,
  CreateTaskRequest,
  UpdateTaskRequest,
} from '../services/taskService';
import { useAuthStore } from '../state/authStore';

const statusColumns = [
  { id: 'Todo', title: 'To Do', color: 'bg-gray-100' },
  { id: 'In Progress', title: 'In Progress', color: 'bg-blue-100' },
  { id: 'Review', title: 'Review', color: 'bg-yellow-100' },
  { id: 'Done', title: 'Done', color: 'bg-green-100' },
];

const priorityColors = {
  High: 'bg-red-100 text-red-800',
  Medium: 'bg-yellow-100 text-yellow-800',
  Low: 'bg-gray-100 text-gray-800',
};

export const Tasks: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [draggedTask, setDraggedTask] = useState<Task | null>(null);
  const [formData, setFormData] = useState<CreateTaskRequest>({
    title: '',
    description: '',
    status: 'Todo',
    priority: 'Medium',
    projectId: '',
    assignedToId: '',
    createdById: '',
  });
  const [submitting, setSubmitting] = useState(false);

  const { user } = useAuthStore();

  // Load tasks on component mount
  useEffect(() => {
    loadTasks();
  }, []);

  // Set user ID when user is available
  useEffect(() => {
    if (user) {
      setFormData((prev) => ({ ...prev, createdById: user.id }));
    }
  }, [user]);

  const loadTasks = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await taskService.getAll();
      setTasks(data);
    } catch (err) {
      setError('Failed to load tasks');
      console.error('Error loading tasks:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateTask = () => {
    setFormData({
      title: '',
      description: '',
      status: 'Todo',
      priority: 'Medium',
      projectId: '',
      assignedToId: '',
      createdById: user?.id || '',
    });
    setSelectedTask(null);
    setShowCreateModal(true);
  };

  const handleEditTask = (task: Task) => {
    setFormData({
      title: task.title,
      description: task.description,
      status: task.status,
      priority: task.priority,
      projectId: task.projectId,
      assignedToId: task.assignedTo,
      createdById: task.createdBy,
      dueDate: task.dueDate,
    });
    setSelectedTask(task);
    setShowCreateModal(true);
  };

  const handleDeleteTask = async (taskId: string) => {
    if (window.confirm('Are you sure you want to delete this task?')) {
      try {
        await taskService.delete(taskId);
        setTasks(tasks.filter((t) => t.id !== taskId));
      } catch (err) {
        setError('Failed to delete task');
        console.error('Error deleting task:', err);
      }
    }
  };

  const handleDragStart = (e: React.DragEvent, task: Task) => {
    setDraggedTask(task);
    e.dataTransfer.effectAllowed = 'move';
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
  };

  const handleDrop = async (e: React.DragEvent, newStatus: string) => {
    e.preventDefault();
    if (draggedTask && draggedTask.status !== newStatus) {
      try {
        // Update task status in backend
        await taskService.updateStatus(draggedTask.id, {
          status: newStatus as Task['status'],
        });

        // Update local state
        setTasks(
          tasks.map((task) =>
            task.id === draggedTask.id
              ? { ...task, status: newStatus as Task['status'] }
              : task
          )
        );
      } catch (err) {
        setError('Failed to update task status');
        console.error('Error updating task status:', err);
      }
    }
    setDraggedTask(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.title.trim() || !formData.projectId || !formData.assignedToId)
      return;

    try {
      setSubmitting(true);
      setError(null);

      if (selectedTask) {
        // Update existing task
        const updateData: UpdateTaskRequest = {
          title: formData.title,
          description: formData.description,
          status: formData.status,
          priority: formData.priority,
          projectId: formData.projectId,
          assignedToId: formData.assignedToId,
          dueDate: formData.dueDate,
        };
        await taskService.update(selectedTask.id, updateData);

        // Update local state
        setTasks(
          tasks.map((t) =>
            t.id === selectedTask.id ? { ...t, ...updateData } : t
          )
        );
      } else {
        // Create new task
        const newTask = await taskService.create(formData);
        setTasks([...tasks, newTask]);
      }

      setShowCreateModal(false);
    } catch (err) {
      setError(
        selectedTask ? 'Failed to update task' : 'Failed to create task'
      );
      console.error('Error saving task:', err);
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

  const getTasksByStatus = (status: string) => {
    return tasks.filter((task) => task.status === status);
  };

  if (loading) {
    return (
      <div className='space-y-6'>
        <div className='flex items-center justify-between'>
          <div>
            <h1 className='text-3xl font-bold text-gray-900'>Tasks</h1>
            <p className='mt-2 text-gray-600'>Loading tasks...</p>
          </div>
        </div>
        <div className='grid grid-cols-1 gap-6 lg:grid-cols-4'>
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className='card'>
              <div className='animate-pulse'>
                <div className='h-4 bg-gray-200 rounded w-1/3 mb-4'></div>
                <div className='space-y-3'>
                  {[1, 2, 3].map((j) => (
                    <div key={j} className='h-20 bg-gray-200 rounded'></div>
                  ))}
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
          <h1 className='text-3xl font-bold text-gray-900'>Tasks</h1>
          <p className='mt-2 text-gray-600'>
            Manage your tasks with our Kanban board.
          </p>
        </div>
        <button
          onClick={handleCreateTask}
          className='btn-primary flex items-center'
        >
          <PlusIcon className='h-5 w-5 mr-2' />
          New Task
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
        {statusColumns.map((status) => (
          <div key={status.id} className='card'>
            <div className='flex items-center'>
              <div className='flex-shrink-0'>
                <div
                  className={`h-8 w-8 ${status.color} rounded-full flex items-center justify-center`}
                >
                  <span className='text-gray-700 font-medium'>
                    {getTasksByStatus(status.id).length}
                  </span>
                </div>
              </div>
              <div className='ml-4'>
                <p className='text-sm font-medium text-gray-500'>
                  {status.title}
                </p>
                <p className='text-2xl font-semibold text-gray-900'>
                  {getTasksByStatus(status.id).length}
                </p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Kanban Board */}
      <div className='grid grid-cols-1 gap-6 lg:grid-cols-4'>
        {statusColumns.map((status) => (
          <div key={status.id} className='space-y-4'>
            <div className='flex items-center justify-between'>
              <h3 className='text-lg font-medium text-gray-900'>
                {status.title}
              </h3>
              <span className='text-sm text-gray-500'>
                {getTasksByStatus(status.id).length}
              </span>
            </div>
            <div
              className={`min-h-[500px] p-4 rounded-lg border-2 border-dashed ${
                status.color
              } ${draggedTask ? 'border-blue-400' : 'border-gray-300'}`}
              onDragOver={handleDragOver}
              onDrop={(e) => handleDrop(e, status.id)}
            >
              {getTasksByStatus(status.id).map((task) => (
                <div
                  key={task.id}
                  className='bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-3 cursor-move hover:shadow-md transition-shadow'
                  draggable
                  onDragStart={(e) => handleDragStart(e, task)}
                >
                  <div className='flex items-start justify-between mb-2'>
                    <h4 className='text-sm font-medium text-gray-900 line-clamp-2'>
                      {task.title}
                    </h4>
                    <div className='flex space-x-1'>
                      <button
                        onClick={() => handleEditTask(task)}
                        className='text-indigo-600 hover:text-indigo-900'
                        title='Edit Task'
                      >
                        <PencilIcon className='h-3 w-3' />
                      </button>
                      <button
                        onClick={() => handleDeleteTask(task.id)}
                        className='text-red-600 hover:text-red-900'
                        title='Delete Task'
                      >
                        <TrashIcon className='h-3 w-3' />
                      </button>
                    </div>
                  </div>
                  <p className='text-xs text-gray-500 mb-3 line-clamp-2'>
                    {task.description}
                  </p>
                  <div className='flex items-center justify-between'>
                    <div className='flex items-center text-xs text-gray-500'>
                      <UserIcon className='h-3 w-3 mr-1' />
                      {task.assignedTo}
                    </div>
                    <span
                      className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        priorityColors[
                          task.priority as keyof typeof priorityColors
                        ]
                      }`}
                    >
                      {task.priority}
                    </span>
                  </div>
                  {task.dueDate && (
                    <div className='flex items-center text-xs text-gray-500 mt-2'>
                      <CalendarIcon className='h-3 w-3 mr-1' />
                      Due: {new Date(task.dueDate).toLocaleDateString()}
                    </div>
                  )}
                </div>
              ))}
              {getTasksByStatus(status.id).length === 0 && (
                <div className='text-center text-gray-500 text-sm py-8'>
                  No tasks in this column
                </div>
              )}
            </div>
          </div>
        ))}
      </div>

      {/* Create/Edit Modal */}
      {showCreateModal && (
        <div className='fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50'>
          <div className='relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white'>
            <div className='mt-3'>
              <h3 className='text-lg font-medium text-gray-900 mb-4'>
                {selectedTask ? 'Edit Task' : 'Create New Task'}
              </h3>

              <form onSubmit={handleSubmit} className='space-y-4'>
                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Task Title *
                  </label>
                  <input
                    type='text'
                    name='title'
                    value={formData.title}
                    onChange={handleInputChange}
                    className='input-field'
                    required
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Description
                  </label>
                  <textarea
                    name='description'
                    value={formData.description}
                    onChange={handleInputChange}
                    className='input-field'
                    rows={3}
                  />
                </div>

                <div className='grid grid-cols-2 gap-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Status
                    </label>
                    <select
                      name='status'
                      value={formData.status}
                      onChange={handleInputChange}
                      className='input-field'
                    >
                      <option value='Todo'>To Do</option>
                      <option value='In Progress'>In Progress</option>
                      <option value='Review'>Review</option>
                      <option value='Done'>Done</option>
                    </select>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Priority
                    </label>
                    <select
                      name='priority'
                      value={formData.priority}
                      onChange={handleInputChange}
                      className='input-field'
                    >
                      <option value='Low'>Low</option>
                      <option value='Medium'>Medium</option>
                      <option value='High'>High</option>
                    </select>
                  </div>
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Project ID *
                  </label>
                  <input
                    type='text'
                    name='projectId'
                    value={formData.projectId}
                    onChange={handleInputChange}
                    className='input-field'
                    placeholder='Enter project ID'
                    required
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Assigned To ID *
                  </label>
                  <input
                    type='text'
                    name='assignedToId'
                    value={formData.assignedToId}
                    onChange={handleInputChange}
                    className='input-field'
                    placeholder='Enter user ID'
                    required
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Due Date
                  </label>
                  <input
                    type='date'
                    name='dueDate'
                    value={formData.dueDate || ''}
                    onChange={handleInputChange}
                    className='input-field'
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
                      : selectedTask
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
