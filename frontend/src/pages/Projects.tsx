import React, { useState, useEffect } from 'react';
import {
  PlusIcon,
  PencilIcon,
  TrashIcon,
  EyeIcon,
} from '@heroicons/react/24/outline';
import { Project } from '../types';
import {
  projectService,
  CreateProjectRequest,
  UpdateProjectRequest,
} from '../services/projectService';
import { useAuthStore } from '../state/authStore';

const statusColors = {
  Active: 'bg-green-100 text-green-800',
  Completed: 'bg-blue-100 text-blue-800',
  'On Hold': 'bg-yellow-100 text-yellow-800',
};

const priorityColors = {
  High: 'bg-red-100 text-red-800',
  Medium: 'bg-yellow-100 text-yellow-800',
  Low: 'bg-gray-100 text-gray-800',
};

export const Projects: React.FC = () => {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);
  const [formData, setFormData] = useState<CreateProjectRequest>({
    name: '',
    description: '',
    status: 'Active',
    priority: 'Medium',
    startDate: new Date().toISOString().split('T')[0],
    createdById: '',
  });
  const [submitting, setSubmitting] = useState(false);

  const { user } = useAuthStore();

  // Load projects on component mount
  useEffect(() => {
    loadProjects();
  }, []);

  // Set user ID when user is available
  useEffect(() => {
    if (user) {
      setFormData((prev) => ({ ...prev, createdById: user.id }));
    }
  }, [user]);

  const loadProjects = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await projectService.getAll();
      setProjects(data);
    } catch (err) {
      setError('Failed to load projects');
      console.error('Error loading projects:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateProject = () => {
    setFormData({
      name: '',
      description: '',
      status: 'Active',
      priority: 'Medium',
      startDate: new Date().toISOString().split('T')[0],
      createdById: user?.id || '',
    });
    setSelectedProject(null);
    setShowCreateModal(true);
  };

  const handleEditProject = (project: Project) => {
    setFormData({
      name: project.name,
      description: project.description,
      status: project.status,
      priority: project.priority,
      startDate: project.startDate,
      endDate: project.endDate,
      createdById: project.createdBy,
    });
    setSelectedProject(project);
    setShowCreateModal(true);
  };

  const handleDeleteProject = async (projectId: string) => {
    if (window.confirm('Are you sure you want to delete this project?')) {
      try {
        await projectService.delete(projectId);
        setProjects(projects.filter((p) => p.id !== projectId));
      } catch (err) {
        setError('Failed to delete project');
        console.error('Error deleting project:', err);
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.name.trim()) return;

    try {
      setSubmitting(true);
      setError(null);

      if (selectedProject) {
        // Update existing project
        const updateData: UpdateProjectRequest = {
          name: formData.name,
          description: formData.description,
          status: formData.status,
          priority: formData.priority,
          startDate: formData.startDate,
          endDate: formData.endDate,
        };
        await projectService.update(selectedProject.id, updateData);

        // Update local state
        setProjects(
          projects.map((p) =>
            p.id === selectedProject.id ? { ...p, ...updateData } : p
          )
        );
      } else {
        // Create new project
        const newProject = await projectService.create(formData);
        setProjects([...projects, newProject]);
      }

      setShowCreateModal(false);
    } catch (err) {
      setError(
        selectedProject
          ? 'Failed to update project'
          : 'Failed to create project'
      );
      console.error('Error saving project:', err);
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

  if (loading) {
    return (
      <div className='space-y-6'>
        <div className='flex items-center justify-between'>
          <div>
            <h1 className='text-3xl font-bold text-gray-900'>Projects</h1>
            <p className='mt-2 text-gray-600'>Loading projects...</p>
          </div>
        </div>
        <div className='card'>
          <div className='animate-pulse'>
            <div className='h-4 bg-gray-200 rounded w-1/4 mb-4'></div>
            <div className='space-y-3'>
              {[1, 2, 3].map((i) => (
                <div key={i} className='h-12 bg-gray-200 rounded'></div>
              ))}
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className='space-y-6'>
      {/* Header */}
      <div className='flex items-center justify-between'>
        <div>
          <h1 className='text-3xl font-bold text-gray-900'>Projects</h1>
          <p className='mt-2 text-gray-600'>
            Manage your projects and track their progress.
          </p>
        </div>
        <button
          onClick={handleCreateProject}
          className='btn-primary flex items-center'
        >
          <PlusIcon className='h-5 w-5 mr-2' />
          New Project
        </button>
      </div>

      {/* Error Message */}
      {error && (
        <div className='bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded'>
          {error}
        </div>
      )}

      {/* Stats */}
      <div className='grid grid-cols-1 gap-6 sm:grid-cols-3'>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-blue-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>A</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>
                Active Projects
              </p>
              <p className='text-2xl font-semibold text-gray-900'>
                {projects.filter((p) => p.status === 'Active').length}
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-green-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>C</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>Completed</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {projects.filter((p) => p.status === 'Completed').length}
              </p>
            </div>
          </div>
        </div>
        <div className='card'>
          <div className='flex items-center'>
            <div className='flex-shrink-0'>
              <div className='h-8 w-8 bg-yellow-500 rounded-full flex items-center justify-center'>
                <span className='text-white font-medium'>H</span>
              </div>
            </div>
            <div className='ml-4'>
              <p className='text-sm font-medium text-gray-500'>On Hold</p>
              <p className='text-2xl font-semibold text-gray-900'>
                {projects.filter((p) => p.status === 'On Hold').length}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Projects List */}
      <div className='card'>
        <div className='overflow-x-auto'>
          <table className='min-w-full divide-y divide-gray-200'>
            <thead className='bg-gray-50'>
              <tr>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Project
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Status
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Priority
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Created By
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Timeline
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className='bg-white divide-y divide-gray-200'>
              {projects.map((project) => (
                <tr key={project.id} className='hover:bg-gray-50'>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <div>
                      <div className='text-sm font-medium text-gray-900'>
                        {project.name}
                      </div>
                      <div className='text-sm text-gray-500'>
                        {project.description}
                      </div>
                    </div>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <span
                      className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        statusColors[
                          project.status as keyof typeof statusColors
                        ]
                      }`}
                    >
                      {project.status}
                    </span>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <span
                      className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        priorityColors[
                          project.priority as keyof typeof priorityColors
                        ]
                      }`}
                    >
                      {project.priority}
                    </span>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <div className='text-sm text-gray-900'>
                      {project.createdBy}
                    </div>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <div className='text-sm text-gray-900'>
                      {new Date(project.startDate).toLocaleDateString()} -{' '}
                      {project.endDate
                        ? new Date(project.endDate).toLocaleDateString()
                        : 'Ongoing'}
                    </div>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium'>
                    <div className='flex space-x-2'>
                      <button
                        onClick={() => setSelectedProject(project)}
                        className='text-blue-600 hover:text-blue-900'
                        title='View Details'
                      >
                        <EyeIcon className='h-4 w-4' />
                      </button>
                      <button
                        onClick={() => handleEditProject(project)}
                        className='text-indigo-600 hover:text-indigo-900'
                        title='Edit Project'
                      >
                        <PencilIcon className='h-4 w-4' />
                      </button>
                      <button
                        onClick={() => handleDeleteProject(project.id)}
                        className='text-red-600 hover:text-red-900'
                        title='Delete Project'
                      >
                        <TrashIcon className='h-4 w-4' />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {projects.length === 0 && (
            <div className='text-center py-8 text-gray-500'>
              No projects found. Create your first project to get started.
            </div>
          )}
        </div>
      </div>

      {/* Create/Edit Modal */}
      {showCreateModal && (
        <div className='fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50'>
          <div className='relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white'>
            <div className='mt-3'>
              <h3 className='text-lg font-medium text-gray-900 mb-4'>
                {selectedProject ? 'Edit Project' : 'Create New Project'}
              </h3>

              <form onSubmit={handleSubmit} className='space-y-4'>
                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Project Name *
                  </label>
                  <input
                    type='text'
                    name='name'
                    value={formData.name}
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
                      <option value='Active'>Active</option>
                      <option value='Completed'>Completed</option>
                      <option value='On Hold'>On Hold</option>
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

                <div className='grid grid-cols-2 gap-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Start Date *
                    </label>
                    <input
                      type='date'
                      name='startDate'
                      value={formData.startDate}
                      onChange={handleInputChange}
                      className='input-field'
                      required
                    />
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      End Date
                    </label>
                    <input
                      type='date'
                      name='endDate'
                      value={formData.endDate || ''}
                      onChange={handleInputChange}
                      className='input-field'
                    />
                  </div>
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
                      : selectedProject
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
