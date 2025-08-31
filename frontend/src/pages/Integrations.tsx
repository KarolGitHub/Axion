import React, { useState, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  PlusIcon,
  CogIcon,
  PlayIcon,
  StopIcon,
  TrashIcon,
  EyeIcon,
  ChartBarIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  ClockIcon,
} from '@heroicons/react/24/outline';
import {
  thirdPartyIntegrationService,
  ThirdPartyIntegration,
  CreateThirdPartyIntegrationRequest,
  UpdateThirdPartyIntegrationRequest,
  IntegrationStats,
  IntegrationSync,
  IntegrationEvent,
  GitHubRepository,
  SlackWorkspace,
  GoogleWorkspaceDomain,
} from '../services/thirdPartyIntegrationService';

interface IntegrationsPageProps {}

export const IntegrationsPage: React.FC<IntegrationsPageProps> = () => {
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [selectedIntegration, setSelectedIntegration] =
    useState<ThirdPartyIntegration | null>(null);
  const [activeTab, setActiveTab] = useState<
    'overview' | 'github' | 'slack' | 'google' | 'syncs' | 'events'
  >('overview');
  const [selectedProvider, setSelectedProvider] = useState<string>('all');

  const queryClient = useQueryClient();

  // Queries
  const { data: integrations = [], isLoading } = useQuery({
    queryKey: ['integrations'],
    queryFn: () => thirdPartyIntegrationService.getIntegrations(),
  });

  const { data: stats } = useQuery({
    queryKey: ['integration-stats', selectedIntegration?.id],
    queryFn: () =>
      selectedIntegration
        ? thirdPartyIntegrationService.getIntegrationStats(
            selectedIntegration.id
          )
        : null,
    enabled: !!selectedIntegration,
  });

  const { data: syncs = [] } = useQuery({
    queryKey: ['integration-syncs', selectedIntegration?.id],
    queryFn: () =>
      selectedIntegration
        ? thirdPartyIntegrationService.getIntegrationSyncs(
            selectedIntegration.id
          )
        : [],
    enabled: !!selectedIntegration,
  });

  const { data: events = [] } = useQuery({
    queryKey: ['integration-events', selectedIntegration?.id],
    queryFn: () =>
      selectedIntegration
        ? thirdPartyIntegrationService.getIntegrationEvents(
            selectedIntegration.id
          )
        : [],
    enabled: !!selectedIntegration,
  });

  // GitHub data
  const { data: githubRepositories = [] } = useQuery({
    queryKey: ['github-repositories', selectedIntegration?.id],
    queryFn: () =>
      selectedIntegration
        ? thirdPartyIntegrationService.getGitHubRepositories(
            selectedIntegration.id
          )
        : [],
    enabled:
      !!selectedIntegration &&
      selectedIntegration.provider.toLowerCase() === 'github',
  });

  // Slack data
  const { data: slackWorkspaces = [] } = useQuery({
    queryKey: ['slack-workspaces', selectedIntegration?.id],
    queryFn: () =>
      selectedIntegration
        ? thirdPartyIntegrationService.getSlackWorkspaces(
            selectedIntegration.id
          )
        : [],
    enabled:
      !!selectedIntegration &&
      selectedIntegration.provider.toLowerCase() === 'slack',
  });

  // Google Workspace data
  const { data: googleDomains = [] } = useQuery({
    queryKey: ['google-domains', selectedIntegration?.id],
    queryFn: () =>
      selectedIntegration
        ? thirdPartyIntegrationService.getGoogleWorkspaceDomains(
            selectedIntegration.id
          )
        : [],
    enabled:
      !!selectedIntegration &&
      selectedIntegration.provider.toLowerCase() === 'googleworkspace',
  });

  // Mutations
  const createMutation = useMutation({
    mutationFn: (request: CreateThirdPartyIntegrationRequest) =>
      thirdPartyIntegrationService.createIntegration(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['integrations'] });
      setShowCreateModal(false);
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({
      id,
      request,
    }: {
      id: string;
      request: UpdateThirdPartyIntegrationRequest;
    }) => thirdPartyIntegrationService.updateIntegration(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['integrations'] });
      setShowEditModal(false);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) =>
      thirdPartyIntegrationService.deleteIntegration(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['integrations'] });
    },
  });

  const syncMutation = useMutation({
    mutationFn: (integrationId: string) =>
      thirdPartyIntegrationService.syncIntegration({
        integrationId,
        syncType: 'full',
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['integration-syncs'] });
      queryClient.invalidateQueries({ queryKey: ['integration-stats'] });
    },
  });

  const testMutation = useMutation({
    mutationFn: ({
      integrationId,
      testType,
    }: {
      integrationId: string;
      testType: string;
    }) => thirdPartyIntegrationService.testIntegration(integrationId, testType),
  });

  // Filter integrations by provider
  const filteredIntegrations = integrations.filter(
    (integration) =>
      selectedProvider === 'all' ||
      integration.provider.toLowerCase() === selectedProvider.toLowerCase()
  );

  const handleCreateIntegration = (
    request: CreateThirdPartyIntegrationRequest
  ) => {
    createMutation.mutate(request);
  };

  const handleUpdateIntegration = (
    id: string,
    request: UpdateThirdPartyIntegrationRequest
  ) => {
    updateMutation.mutate({ id, request });
  };

  const handleDeleteIntegration = (id: string) => {
    if (window.confirm('Are you sure you want to delete this integration?')) {
      deleteMutation.mutate(id);
    }
  };

  const handleSyncIntegration = (integrationId: string) => {
    syncMutation.mutate(integrationId);
  };

  const handleTestIntegration = (integrationId: string, testType: string) => {
    testMutation.mutate({ integrationId, testType });
  };

  const handleViewDetails = (integration: ThirdPartyIntegration) => {
    setSelectedIntegration(integration);
    setShowDetailsModal(true);
    setActiveTab('overview');
  };

  const getProviderIcon = (provider: string) => {
    return thirdPartyIntegrationService.getProviderIcon(provider);
  };

  const getProviderColor = (provider: string) => {
    return thirdPartyIntegrationService.getProviderColor(provider);
  };

  const formatSyncStatus = (status?: string) => {
    return thirdPartyIntegrationService.formatSyncStatus(status);
  };

  const formatEventStatus = (status?: string) => {
    return thirdPartyIntegrationService.formatEventStatus(status);
  };

  if (isLoading) {
    return (
      <div className='flex items-center justify-center h-64'>
        <div className='animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600'></div>
      </div>
    );
  }

  return (
    <div className='space-y-6'>
      {/* Header */}
      <div className='flex items-center justify-between'>
        <div>
          <h1 className='text-2xl font-bold text-gray-900 dark:text-white'>
            Third-Party Integrations
          </h1>
          <p className='text-gray-600 dark:text-gray-400'>
            Manage your GitHub, Slack, and Google Workspace integrations
          </p>
        </div>
        <button
          onClick={() => setShowCreateModal(true)}
          className='btn-primary flex items-center space-x-2'
        >
          <PlusIcon className='h-5 w-5' />
          <span>Add Integration</span>
        </button>
      </div>

      {/* Filter */}
      <div className='flex items-center space-x-4'>
        <label className='text-sm font-medium text-gray-700 dark:text-gray-300'>
          Filter by:
        </label>
        <select
          value={selectedProvider}
          onChange={(e) => setSelectedProvider(e.target.value)}
          className='input-field w-48'
        >
          <option value='all'>All Providers</option>
          <option value='github'>GitHub</option>
          <option value='slack'>Slack</option>
          <option value='googleworkspace'>Google Workspace</option>
        </select>
      </div>

      {/* Integrations Grid */}
      <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
        {filteredIntegrations.map((integration) => (
          <div key={integration.id} className='card'>
            <div className='flex items-start justify-between'>
              <div className='flex items-center space-x-3'>
                <img
                  src={getProviderIcon(integration.provider)}
                  alt={integration.provider}
                  className='h-8 w-8 rounded'
                />
                <div>
                  <h3 className='font-semibold text-gray-900 dark:text-white'>
                    {integration.name}
                  </h3>
                  <p className='text-sm text-gray-600 dark:text-gray-400'>
                    {integration.provider}
                  </p>
                </div>
              </div>
              <div className='flex items-center space-x-2'>
                <button
                  onClick={() => handleViewDetails(integration)}
                  className='text-gray-400 hover:text-gray-600 dark:text-gray-400 dark:hover:text-gray-200'
                  title='View Details'
                >
                  <EyeIcon className='h-5 w-5' />
                </button>
                <button
                  onClick={() => {
                    setSelectedIntegration(integration);
                    setShowEditModal(true);
                  }}
                  className='text-gray-400 hover:text-gray-600 dark:text-gray-400 dark:hover:text-gray-200'
                  title='Edit'
                >
                  <CogIcon className='h-5 w-5' />
                </button>
                <button
                  onClick={() => handleDeleteIntegration(integration.id)}
                  className='text-gray-400 hover:text-red-600 dark:text-gray-400 dark:hover:text-red-400'
                  title='Delete'
                >
                  <TrashIcon className='h-5 w-5' />
                </button>
              </div>
            </div>

            <div className='mt-4 space-y-2'>
              <div className='flex items-center justify-between text-sm'>
                <span className='text-gray-600 dark:text-gray-400'>
                  Status:
                </span>
                <span
                  className={`px-2 py-1 rounded-full text-xs ${
                    integration.isEnabled
                      ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                      : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
                  }`}
                >
                  {integration.isEnabled ? 'Active' : 'Inactive'}
                </span>
              </div>

              <div className='flex items-center justify-between text-sm'>
                <span className='text-gray-600 dark:text-gray-400'>
                  Auto Sync:
                </span>
                <span
                  className={`px-2 py-1 rounded-full text-xs ${
                    integration.autoSync
                      ? 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200'
                      : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200'
                  }`}
                >
                  {integration.autoSync ? 'Enabled' : 'Disabled'}
                </span>
              </div>

              {integration.lastSyncAt && (
                <div className='flex items-center justify-between text-sm'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Last Sync:
                  </span>
                  <span className='text-gray-900 dark:text-white'>
                    {new Date(integration.lastSyncAt).toLocaleDateString()}
                  </span>
                </div>
              )}

              {integration.lastSyncError && (
                <div className='flex items-center space-x-2 text-sm text-red-600 dark:text-red-400'>
                  <ExclamationTriangleIcon className='h-4 w-4' />
                  <span>Sync Error</span>
                </div>
              )}
            </div>

            <div className='mt-4 flex items-center space-x-2'>
              <button
                onClick={() => handleSyncIntegration(integration.id)}
                disabled={syncMutation.isPending}
                className='btn-secondary flex items-center space-x-1 text-sm'
              >
                <PlayIcon className='h-4 w-4' />
                <span>Sync</span>
              </button>
              <button
                onClick={() =>
                  handleTestIntegration(integration.id, 'connection')
                }
                disabled={testMutation.isPending}
                className='btn-secondary flex items-center space-x-1 text-sm'
              >
                <CheckCircleIcon className='h-4 w-4' />
                <span>Test</span>
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Empty State */}
      {filteredIntegrations.length === 0 && (
        <div className='text-center py-12'>
          <div className='mx-auto h-12 w-12 text-gray-400'>
            <CogIcon className='h-12 w-12' />
          </div>
          <h3 className='mt-2 text-sm font-medium text-gray-900 dark:text-white'>
            No integrations
          </h3>
          <p className='mt-1 text-sm text-gray-500 dark:text-gray-400'>
            Get started by adding your first integration.
          </p>
          <div className='mt-6'>
            <button
              onClick={() => setShowCreateModal(true)}
              className='btn-primary'
            >
              <PlusIcon className='h-5 w-5 mr-2' />
              Add Integration
            </button>
          </div>
        </div>
      )}

      {/* Create Integration Modal */}
      {showCreateModal && (
        <CreateIntegrationModal
          onClose={() => setShowCreateModal(false)}
          onSubmit={handleCreateIntegration}
          isLoading={createMutation.isPending}
        />
      )}

      {/* Edit Integration Modal */}
      {showEditModal && selectedIntegration && (
        <EditIntegrationModal
          integration={selectedIntegration}
          onClose={() => setShowEditModal(false)}
          onSubmit={handleUpdateIntegration}
          isLoading={updateMutation.isPending}
        />
      )}

      {/* Integration Details Modal */}
      {showDetailsModal && selectedIntegration && (
        <IntegrationDetailsModal
          integration={selectedIntegration}
          stats={stats}
          syncs={syncs}
          events={events}
          githubRepositories={githubRepositories}
          slackWorkspaces={slackWorkspaces}
          googleDomains={googleDomains}
          activeTab={activeTab}
          onTabChange={setActiveTab}
          onClose={() => setShowDetailsModal(false)}
          onSync={() => handleSyncIntegration(selectedIntegration.id)}
          onTest={(testType) =>
            handleTestIntegration(selectedIntegration.id, testType)
          }
          isLoading={syncMutation.isPending || testMutation.isPending}
        />
      )}
    </div>
  );
};

// Create Integration Modal Component
interface CreateIntegrationModalProps {
  onClose: () => void;
  onSubmit: (request: CreateThirdPartyIntegrationRequest) => void;
  isLoading: boolean;
}

const CreateIntegrationModal: React.FC<CreateIntegrationModalProps> = ({
  onClose,
  onSubmit,
  isLoading,
}) => {
  const [formData, setFormData] = useState<CreateThirdPartyIntegrationRequest>({
    name: '',
    provider: 'GitHub',
    clientId: '',
    clientSecret: '',
    webhookUrl: '',
    webhookSecret: '',
    autoSync: false,
    syncSettings: '',
    organizationId: '',
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <div className='fixed inset-0 z-50 flex items-center justify-center'>
      <div
        className='absolute inset-0 bg-black bg-opacity-50'
        onClick={onClose}
      />
      <div className='relative bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md'>
        <h2 className='text-lg font-semibold mb-4'>Create Integration</h2>
        <form onSubmit={handleSubmit} className='space-y-4'>
          <div>
            <label className='block text-sm font-medium mb-1'>Name</label>
            <input
              type='text'
              value={formData.name}
              onChange={(e) =>
                setFormData({ ...formData, name: e.target.value })
              }
              className='input-field'
              required
            />
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>Provider</label>
            <select
              value={formData.provider}
              onChange={(e) =>
                setFormData({ ...formData, provider: e.target.value })
              }
              className='input-field'
            >
              <option value='GitHub'>GitHub</option>
              <option value='Slack'>Slack</option>
              <option value='GoogleWorkspace'>Google Workspace</option>
            </select>
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>Client ID</label>
            <input
              type='text'
              value={formData.clientId}
              onChange={(e) =>
                setFormData({ ...formData, clientId: e.target.value })
              }
              className='input-field'
              required
            />
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>
              Client Secret
            </label>
            <input
              type='password'
              value={formData.clientSecret}
              onChange={(e) =>
                setFormData({ ...formData, clientSecret: e.target.value })
              }
              className='input-field'
              required
            />
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>
              Webhook URL (Optional)
            </label>
            <input
              type='url'
              value={formData.webhookUrl}
              onChange={(e) =>
                setFormData({ ...formData, webhookUrl: e.target.value })
              }
              className='input-field'
            />
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>
              Webhook Secret (Optional)
            </label>
            <input
              type='password'
              value={formData.webhookSecret}
              onChange={(e) =>
                setFormData({ ...formData, webhookSecret: e.target.value })
              }
              className='input-field'
            />
          </div>

          <div className='flex items-center'>
            <input
              type='checkbox'
              checked={formData.autoSync}
              onChange={(e) =>
                setFormData({ ...formData, autoSync: e.target.checked })
              }
              className='mr-2'
            />
            <label className='text-sm'>Enable Auto Sync</label>
          </div>

          <div className='flex space-x-3'>
            <button
              type='button'
              onClick={onClose}
              className='btn-secondary flex-1'
              disabled={isLoading}
            >
              Cancel
            </button>
            <button
              type='submit'
              className='btn-primary flex-1'
              disabled={isLoading}
            >
              {isLoading ? 'Creating...' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// Edit Integration Modal Component
interface EditIntegrationModalProps {
  integration: ThirdPartyIntegration;
  onClose: () => void;
  onSubmit: (id: string, request: UpdateThirdPartyIntegrationRequest) => void;
  isLoading: boolean;
}

const EditIntegrationModal: React.FC<EditIntegrationModalProps> = ({
  integration,
  onClose,
  onSubmit,
  isLoading,
}) => {
  const [formData, setFormData] = useState<UpdateThirdPartyIntegrationRequest>({
    name: integration.name,
    clientId: '',
    clientSecret: '',
    webhookUrl: '',
    webhookSecret: '',
    isEnabled: integration.isEnabled,
    autoSync: integration.autoSync,
    syncSettings: integration.syncSettings,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(integration.id, formData);
  };

  return (
    <div className='fixed inset-0 z-50 flex items-center justify-center'>
      <div
        className='absolute inset-0 bg-black bg-opacity-50'
        onClick={onClose}
      />
      <div className='relative bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md'>
        <h2 className='text-lg font-semibold mb-4'>Edit Integration</h2>
        <form onSubmit={handleSubmit} className='space-y-4'>
          <div>
            <label className='block text-sm font-medium mb-1'>Name</label>
            <input
              type='text'
              value={formData.name}
              onChange={(e) =>
                setFormData({ ...formData, name: e.target.value })
              }
              className='input-field'
              required
            />
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>Client ID</label>
            <input
              type='text'
              value={formData.clientId}
              onChange={(e) =>
                setFormData({ ...formData, clientId: e.target.value })
              }
              className='input-field'
              placeholder='Enter new client ID to update'
            />
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>
              Client Secret
            </label>
            <input
              type='password'
              value={formData.clientSecret}
              onChange={(e) =>
                setFormData({ ...formData, clientSecret: e.target.value })
              }
              className='input-field'
              placeholder='Enter new client secret to update'
            />
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>
              Webhook URL
            </label>
            <input
              type='url'
              value={formData.webhookUrl}
              onChange={(e) =>
                setFormData({ ...formData, webhookUrl: e.target.value })
              }
              className='input-field'
            />
          </div>

          <div>
            <label className='block text-sm font-medium mb-1'>
              Webhook Secret
            </label>
            <input
              type='password'
              value={formData.webhookSecret}
              onChange={(e) =>
                setFormData({ ...formData, webhookSecret: e.target.value })
              }
              className='input-field'
            />
          </div>

          <div className='flex items-center'>
            <input
              type='checkbox'
              checked={formData.isEnabled}
              onChange={(e) =>
                setFormData({ ...formData, isEnabled: e.target.checked })
              }
              className='mr-2'
            />
            <label className='text-sm'>Enabled</label>
          </div>

          <div className='flex items-center'>
            <input
              type='checkbox'
              checked={formData.autoSync}
              onChange={(e) =>
                setFormData({ ...formData, autoSync: e.target.checked })
              }
              className='mr-2'
            />
            <label className='text-sm'>Auto Sync</label>
          </div>

          <div className='flex space-x-3'>
            <button
              type='button'
              onClick={onClose}
              className='btn-secondary flex-1'
              disabled={isLoading}
            >
              Cancel
            </button>
            <button
              type='submit'
              className='btn-primary flex-1'
              disabled={isLoading}
            >
              {isLoading ? 'Updating...' : 'Update'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// Integration Details Modal Component
interface IntegrationDetailsModalProps {
  integration: ThirdPartyIntegration;
  stats?: IntegrationStats;
  syncs: IntegrationSync[];
  events: IntegrationEvent[];
  githubRepositories: GitHubRepository[];
  slackWorkspaces: SlackWorkspace[];
  googleDomains: GoogleWorkspaceDomain[];
  activeTab: string;
  onTabChange: (tab: string) => void;
  onClose: () => void;
  onSync: () => void;
  onTest: (testType: string) => void;
  isLoading: boolean;
}

const IntegrationDetailsModal: React.FC<IntegrationDetailsModalProps> = ({
  integration,
  stats,
  syncs,
  events,
  githubRepositories,
  slackWorkspaces,
  googleDomains,
  activeTab,
  onTabChange,
  onClose,
  onSync,
  onTest,
  isLoading,
}) => {
  const tabs = [
    { id: 'overview', name: 'Overview', icon: ChartBarIcon },
    {
      id: 'github',
      name: 'GitHub',
      icon: ChartBarIcon,
      show: integration.provider.toLowerCase() === 'github',
    },
    {
      id: 'slack',
      name: 'Slack',
      icon: ChartBarIcon,
      show: integration.provider.toLowerCase() === 'slack',
    },
    {
      id: 'google',
      name: 'Google Workspace',
      icon: ChartBarIcon,
      show: integration.provider.toLowerCase() === 'googleworkspace',
    },
    { id: 'syncs', name: 'Sync History', icon: ClockIcon },
    { id: 'events', name: 'Events', icon: ChartBarIcon },
  ].filter((tab) => tab.show !== false);

  return (
    <div className='fixed inset-0 z-50 flex items-center justify-center'>
      <div
        className='absolute inset-0 bg-black bg-opacity-50'
        onClick={onClose}
      />
      <div className='relative bg-white dark:bg-gray-800 rounded-lg w-full max-w-4xl h-[90vh] flex flex-col'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b border-gray-200 dark:border-gray-700'>
          <div className='flex items-center space-x-3'>
            <img
              src={thirdPartyIntegrationService.getProviderIcon(
                integration.provider
              )}
              alt={integration.provider}
              className='h-8 w-8 rounded'
            />
            <div>
              <h2 className='text-lg font-semibold'>{integration.name}</h2>
              <p className='text-sm text-gray-600 dark:text-gray-400'>
                {integration.provider}
              </p>
            </div>
          </div>
          <div className='flex items-center space-x-2'>
            <button
              onClick={onSync}
              disabled={isLoading}
              className='btn-secondary flex items-center space-x-1'
            >
              <PlayIcon className='h-4 w-4' />
              <span>Sync</span>
            </button>
            <button
              onClick={() => onTest('connection')}
              disabled={isLoading}
              className='btn-secondary flex items-center space-x-1'
            >
              <CheckCircleIcon className='h-4 w-4' />
              <span>Test</span>
            </button>
            <button
              onClick={onClose}
              className='text-gray-400 hover:text-gray-600 dark:text-gray-400 dark:hover:text-gray-200'
            >
              <span className='sr-only'>Close</span>
              <span className='text-2xl'>&times;</span>
            </button>
          </div>
        </div>

        {/* Tabs */}
        <div className='border-b border-gray-200 dark:border-gray-700'>
          <nav className='flex space-x-8 px-6'>
            {tabs.map((tab) => (
              <button
                key={tab.id}
                onClick={() => onTabChange(tab.id)}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  activeTab === tab.id
                    ? 'border-blue-500 text-blue-600 dark:text-blue-400'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-400 dark:hover:text-gray-300'
                }`}
              >
                <tab.icon className='h-5 w-5 inline mr-2' />
                {tab.name}
              </button>
            ))}
          </nav>
        </div>

        {/* Content */}
        <div className='flex-1 overflow-auto p-6'>
          {activeTab === 'overview' && (
            <div className='space-y-6'>
              <div className='grid grid-cols-1 md:grid-cols-3 gap-4'>
                <div className='card'>
                  <h3 className='font-semibold mb-2'>Status</h3>
                  <div className='space-y-2'>
                    <div className='flex justify-between'>
                      <span>Enabled:</span>
                      <span
                        className={
                          integration.isEnabled
                            ? 'text-green-600'
                            : 'text-red-600'
                        }
                      >
                        {integration.isEnabled ? 'Yes' : 'No'}
                      </span>
                    </div>
                    <div className='flex justify-between'>
                      <span>Auto Sync:</span>
                      <span
                        className={
                          integration.autoSync
                            ? 'text-green-600'
                            : 'text-gray-600'
                        }
                      >
                        {integration.autoSync ? 'Yes' : 'No'}
                      </span>
                    </div>
                  </div>
                </div>

                {stats && (
                  <>
                    <div className='card'>
                      <h3 className='font-semibold mb-2'>Events</h3>
                      <div className='space-y-2'>
                        <div className='flex justify-between'>
                          <span>Total:</span>
                          <span>{stats.totalEvents}</span>
                        </div>
                        <div className='flex justify-between'>
                          <span>Pending:</span>
                          <span className='text-yellow-600'>
                            {stats.pendingEvents}
                          </span>
                        </div>
                        <div className='flex justify-between'>
                          <span>Failed:</span>
                          <span className='text-red-600'>
                            {stats.failedEvents}
                          </span>
                        </div>
                      </div>
                    </div>

                    <div className='card'>
                      <h3 className='font-semibold mb-2'>Syncs</h3>
                      <div className='space-y-2'>
                        <div className='flex justify-between'>
                          <span>Total:</span>
                          <span>{stats.totalSyncs}</span>
                        </div>
                        <div className='flex justify-between'>
                          <span>Successful:</span>
                          <span className='text-green-600'>
                            {stats.successfulSyncs}
                          </span>
                        </div>
                        <div className='flex justify-between'>
                          <span>Failed:</span>
                          <span className='text-red-600'>
                            {stats.failedSyncs}
                          </span>
                        </div>
                      </div>
                    </div>
                  </>
                )}
              </div>

              {integration.lastSyncError && (
                <div className='card border-red-200 dark:border-red-800'>
                  <h3 className='font-semibold text-red-800 dark:text-red-200 mb-2'>
                    Last Error
                  </h3>
                  <p className='text-red-600 dark:text-red-400'>
                    {integration.lastSyncError}
                  </p>
                </div>
              )}
            </div>
          )}

          {activeTab === 'github' && (
            <div className='space-y-4'>
              <h3 className='text-lg font-semibold'>GitHub Repositories</h3>
              <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                {githubRepositories.map((repo) => (
                  <div key={repo.id} className='card'>
                    <h4 className='font-semibold'>{repo.name}</h4>
                    <p className='text-sm text-gray-600 dark:text-gray-400 mb-2'>
                      {repo.description}
                    </p>
                    <div className='flex items-center space-x-4 text-sm'>
                      <span>Issues: {repo.issueCount}</span>
                      <span>PRs: {repo.pullRequestCount}</span>
                      <span>Stars: {repo.stars}</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {activeTab === 'slack' && (
            <div className='space-y-4'>
              <h3 className='text-lg font-semibold'>Slack Workspaces</h3>
              <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                {slackWorkspaces.map((workspace) => (
                  <div key={workspace.id} className='card'>
                    <h4 className='font-semibold'>{workspace.name}</h4>
                    <p className='text-sm text-gray-600 dark:text-gray-400 mb-2'>
                      {workspace.domain}
                    </p>
                    <div className='flex items-center space-x-4 text-sm'>
                      <span>Channels: {workspace.channelCount}</span>
                      <span>Users: {workspace.userCount}</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {activeTab === 'google' && (
            <div className='space-y-4'>
              <h3 className='text-lg font-semibold'>
                Google Workspace Domains
              </h3>
              <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                {googleDomains.map((domain) => (
                  <div key={domain.id} className='card'>
                    <h4 className='font-semibold'>{domain.domain}</h4>
                    <p className='text-sm text-gray-600 dark:text-gray-400 mb-2'>
                      {domain.name}
                    </p>
                    <div className='flex items-center space-x-4 text-sm'>
                      <span>Users: {domain.userCount}</span>
                      <span>Groups: {domain.groupCount}</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {activeTab === 'syncs' && (
            <div className='space-y-4'>
              <h3 className='text-lg font-semibold'>Sync History</h3>
              <div className='space-y-2'>
                {syncs.map((sync) => (
                  <div key={sync.id} className='card'>
                    <div className='flex items-center justify-between'>
                      <div>
                        <h4 className='font-semibold'>{sync.syncType} Sync</h4>
                        <p className='text-sm text-gray-600 dark:text-gray-400'>
                          {sync.entityType}{' '}
                          {sync.entityId ? `- ${sync.entityId}` : ''}
                        </p>
                      </div>
                      <div className='text-right'>
                        <div className={formatSyncStatus(sync.status).color}>
                          {formatSyncStatus(sync.status).text}
                        </div>
                        <div className='text-sm text-gray-600 dark:text-gray-400'>
                          {new Date(sync.startedAt).toLocaleString()}
                        </div>
                      </div>
                    </div>
                    <div className='mt-2 flex items-center space-x-4 text-sm'>
                      <span>Processed: {sync.itemsProcessed}</span>
                      <span>Created: {sync.itemsCreated}</span>
                      <span>Updated: {sync.itemsUpdated}</span>
                      <span>Failed: {sync.itemsFailed}</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {activeTab === 'events' && (
            <div className='space-y-4'>
              <h3 className='text-lg font-semibold'>Integration Events</h3>
              <div className='space-y-2'>
                {events.map((event) => (
                  <div key={event.id} className='card'>
                    <div className='flex items-center justify-between'>
                      <div>
                        <h4 className='font-semibold'>{event.eventType}</h4>
                        <p className='text-sm text-gray-600 dark:text-gray-400'>
                          {event.externalId && `ID: ${event.externalId}`}
                        </p>
                      </div>
                      <div className='text-right'>
                        <div className={formatEventStatus(event.status).color}>
                          {formatEventStatus(event.status).text}
                        </div>
                        <div className='text-sm text-gray-600 dark:text-gray-400'>
                          {new Date(event.createdAt).toLocaleString()}
                        </div>
                      </div>
                    </div>
                    {event.errorMessage && (
                      <div className='mt-2 text-sm text-red-600 dark:text-red-400'>
                        Error: {event.errorMessage}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
