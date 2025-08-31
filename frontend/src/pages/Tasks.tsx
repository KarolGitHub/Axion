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
import { projectService } from '../services/projectService';
import { useAuthStore } from '../state/authStore';
import { AIInsights } from '../components/AIInsights';

const statusColumns = [
  { id: 'Todo', title: 'To Do', color: 'bg-gray-100' },
  { id: 'In Progress', title: 'In Progress', color: 'bg-blue-100' },
  { id: 'Review', title: 'Review', color: 'bg-yellow-100' },
  { id: 'Done', title: 'Done', color: 'bg-green-100' },
];

const priorityColors = {
  Low: 'bg-gray-100 text-gray-800',
  Medium: 'bg-yellow-100 text-yellow-800',
  High: 'bg-red-100 text-red-800',
};

export const Tasks: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [projects, setProjects] = useState<any[]>([]);
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

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    if (user) {
      setFormData((prev) => ({ ...prev, createdById: user.id }));
    }
  }, [user]);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [tasksData, projectsData] = await Promise.all([
        taskService.getAll(),
        projectService.getAll(),
      ]);
      setTasks(tasksData);
      setProjects(projectsData);
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
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
  };

  const handleDrop = async (e: React.DragEvent, newStatus: string) => {
    e.preventDefault();
    if (!draggedTask || draggedTask.status === newStatus) return;

    try {
      await taskService.updateStatus(draggedTask.id, {
        status: newStatus as any,
      });
      setTasks(
        tasks.map((t) =>
          t.id === draggedTask.id ? { ...t, status: newStatus as any } : t
        )
      );
    } catch (err) {
      setError('Failed to update task status');
      console.error('Error updating task status:', err);
    }
    setDraggedTask(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.title.trim()) return;

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

  const handlePriorityUpdate = async (
    taskId: string,
    priority: 'Low' | 'Medium' | 'High'
  ) => {
    try {
      const task = tasks.find((t) => t.id === taskId);
      if (!task) return;

      await taskService.update(taskId, { priority });
      setTasks(tasks.map((t) => (t.id === taskId ? { ...t, priority } : t)));
    } catch (err) {
      setError('Failed to update task priority');
      console.error('Error updating task priority:', err);
    }
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
        <div className='grid grid-cols-1 gap-6 md:grid-cols-4'>
          {[1, 2, 3, 4].map((i) => (
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
          <h1 className='text-3xl font-bold text-gray-900'>Tasks</h1>
          <p className='mt-2 text-gray-600'>
            Manage your tasks with AI-powered insights and drag-and-drop
            organization.
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
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-blue-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>T</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Total Tasks</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {tasks.length}
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-green-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>D</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Done</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {tasks.filter((t) => t.status === 'Done').length}
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-yellow-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>P</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>In Progress</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {tasks.filter((t) => t.status === 'In Progress').length}
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-red-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>H</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>High Priority</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {tasks.filter((t) => t.priority === 'High').length}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* AI Insights */}
      <AIInsights
        tasks={tasks}
        projects={projects}
        onPriorityUpdate={handlePriorityUpdate}
      />

      {/* Kanban Board */}
      <div className='grid grid-cols-1 gap-6 md:grid-cols-4'>
        {statusColumns.map((column) => (
          <div key={column.id} className='space-y-4'>
            <div className={`${column.color} p-4 rounded-lg`}>
              <h3 className='font-medium text-gray-900'>{column.title}</h3>
              <p className='text-sm text-gray-600'>
                {getTasksByStatus(column.id).length} tasks
              </p>
            </div>
            <div
              className='min-h-64 p-4 bg-gray-50 rounded-lg'
              onDragOver={handleDragOver}
              onDrop={(e) => handleDrop(e, column.id)}
            >
              <div className='space-y-3'>
                {getTasksByStatus(column.id).map((task) => (
                  <div
                    key={task.id}
                    draggable
                    onDragStart={(e) => handleDragStart(e, task)}
                    className='bg-white p-4 rounded-lg shadow-sm border border-gray-200 cursor-move hover:shadow-md transition-shadow'
                  >
                    <div className='flex items-start justify-between mb-2'>
                      <h4 className='font-medium text-gray-900 text-sm'>
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
                    <p className='text-xs text-gray-600 mb-2 line-clamp-2'>
                      {task.description}
                    </p>
                    <div className='flex items-center justify-between'>
                      <span
                        className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                          priorityColors[
                            task.priority as keyof typeof priorityColors
                          ]
                        }`}
                      >
                        {task.priority}
                      </span>
                      {task.dueDate && (
                        <div className='flex items-center text-xs text-gray-500'>
                          <CalendarIcon className='h-3 w-3 mr-1' />
                          {new Date(task.dueDate).toLocaleDateString()}
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
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
                    rows={3}
                    className='input-field'
                  />
                </div>

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
                    <option value='Todo'>Todo</option>
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

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Project
                  </label>
                  <select
                    name='projectId'
                    value={formData.projectId}
                    onChange={handleInputChange}
                    className='input-field'
                  >
                    <option value=''>Select a project</option>
                    {projects.map((project) => (
                      <option key={project.id} value={project.id}>
                        {project.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Assigned To
                  </label>
                  <input
                    type='text'
                    name='assignedToId'
                    value={formData.assignedToId}
                    onChange={handleInputChange}
                    className='input-field'
                    placeholder='User ID'
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
