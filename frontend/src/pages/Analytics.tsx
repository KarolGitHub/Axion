import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  ChartBarIcon,
  PresentationChartLineIcon,
  CalculatorIcon,
  ClockIcon,
  CurrencyDollarIcon,
  BrainIcon,
  PlusIcon,
  PencilIcon,
  TrashIcon,
  EyeIcon,
  DocumentChartBarIcon,
} from '@heroicons/react/24/outline';
import {
  analyticsService,
  type Dashboard,
  type AnalyticsMetric,
  type BurndownChart,
  type AgileMetrics,
  type ResourceUtilization,
  type ROITracking,
  type PredictiveAnalytics,
  type AnalyticsSummary,
} from '../services/analyticsService';

const AnalyticsPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState('summary');
  const [selectedDashboard, setSelectedDashboard] = useState<Dashboard | null>(
    null
  );
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [createModalType, setCreateModalType] = useState<string>('');
  const queryClient = useQueryClient();

  // Queries
  const { data: summary } = useQuery<AnalyticsSummary>({
    queryKey: ['analytics-summary'],
    queryFn: analyticsService.getAnalyticsSummary,
  });

  const { data: dashboards } = useQuery<Dashboard[]>({
    queryKey: ['dashboards'],
    queryFn: analyticsService.getDashboards,
  });

  const { data: metrics } = useQuery<AnalyticsMetric[]>({
    queryKey: ['metrics'],
    queryFn: analyticsService.getMetrics,
  });

  const { data: resourceUtilizations } = useQuery<ResourceUtilization[]>({
    queryKey: ['resource-utilization'],
    queryFn: () => analyticsService.getResourceUtilization(),
  });

  const { data: roiTracking } = useQuery<ROITracking[]>({
    queryKey: ['roi-tracking'],
    queryFn: analyticsService.getROITracking,
  });

  const { data: predictiveAnalytics } = useQuery<PredictiveAnalytics[]>({
    queryKey: ['predictive-analytics'],
    queryFn: analyticsService.getPredictiveAnalytics,
  });

  // Mutations
  const deleteDashboardMutation = useMutation({
    mutationFn: analyticsService.deleteDashboard,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dashboards'] });
    },
  });

  const deleteMetricMutation = useMutation({
    mutationFn: analyticsService.deleteMetric,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });

  const deleteUtilizationMutation = useMutation({
    mutationFn: analyticsService.deleteResourceUtilization,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['resource-utilization'] });
    },
  });

  const deleteROIMutation = useMutation({
    mutationFn: analyticsService.deleteROITracking,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['roi-tracking'] });
    },
  });

  const deletePredictiveMutation = useMutation({
    mutationFn: analyticsService.deletePredictiveAnalytics,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['predictive-analytics'] });
    },
  });

  const tabs = [
    { id: 'summary', name: 'Summary', icon: DocumentChartBarIcon },
    { id: 'dashboards', name: 'Dashboards', icon: ChartBarIcon },
    { id: 'metrics', name: 'Metrics', icon: CalculatorIcon },
    { id: 'utilization', name: 'Resource Utilization', icon: ClockIcon },
    { id: 'roi', name: 'ROI Tracking', icon: CurrencyDollarIcon },
    { id: 'predictive', name: 'Predictive Analytics', icon: BrainIcon },
  ];

  const handleDelete = (type: string, id: number) => {
    if (window.confirm('Are you sure you want to delete this item?')) {
      switch (type) {
        case 'dashboard':
          deleteDashboardMutation.mutate(id);
          break;
        case 'metric':
          deleteMetricMutation.mutate(id);
          break;
        case 'utilization':
          deleteUtilizationMutation.mutate(id);
          break;
        case 'roi':
          deleteROIMutation.mutate(id);
          break;
        case 'predictive':
          deletePredictiveMutation.mutate(id);
          break;
      }
    }
  };

  const openCreateModal = (type: string) => {
    setCreateModalType(type);
    setShowCreateModal(true);
  };

  return (
    <div className='p-6'>
      <div className='mb-6'>
        <h1 className='text-3xl font-bold text-gray-900 dark:text-white'>
          Analytics & Reporting
        </h1>
        <p className='text-gray-600 dark:text-gray-400 mt-2'>
          Advanced analytics, custom dashboards, and comprehensive reporting
        </p>
      </div>

      {/* Tab Navigation */}
      <div className='border-b border-gray-200 dark:border-gray-700 mb-6'>
        <nav className='-mb-px flex space-x-8'>
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`py-2 px-1 border-b-2 font-medium text-sm flex items-center space-x-2 ${
                activeTab === tab.id
                  ? 'border-blue-500 text-blue-600 dark:text-blue-400'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-400 dark:hover:text-gray-300'
              }`}
            >
              <tab.icon className='h-5 w-5' />
              <span>{tab.name}</span>
            </button>
          ))}
        </nav>
      </div>

      {/* Summary Tab */}
      {activeTab === 'summary' && summary && (
        <div className='space-y-6'>
          <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
            <SummaryCard
              title='Dashboards'
              value={summary.dashboardSummary.totalDashboards}
              subtitle={`${summary.dashboardSummary.defaultDashboards} default`}
              icon={ChartBarIcon}
              color='blue'
            />
            <SummaryCard
              title='Metrics'
              value={summary.metricsSummary.totalMetrics}
              subtitle={`${summary.metricsSummary.metricsCalculatedToday} calculated today`}
              icon={CalculatorIcon}
              color='green'
            />
            <SummaryCard
              title='Active Sprints'
              value={summary.agileSummary.activeSprints}
              subtitle={`${summary.agileSummary.totalSprints} total sprints`}
              icon={PresentationChartLineIcon}
              color='purple'
            />
            <SummaryCard
              title='Avg Utilization'
              value={analyticsService.formatUtilizationRate(
                summary.resourceSummary.averageUtilizationRate
              )}
              subtitle={`${summary.resourceSummary.totalUsers} users tracked`}
              icon={ClockIcon}
              color='orange'
            />
            <SummaryCard
              title='Average ROI'
              value={analyticsService.formatROI(summary.roiSummary.averageROI)}
              subtitle={`${summary.roiSummary.totalProjects} projects`}
              icon={CurrencyDollarIcon}
              color='green'
            />
            <SummaryCard
              title='Predictive Models'
              value={summary.predictiveSummary.activeModels}
              subtitle={`${summary.predictiveSummary.totalModels} total models`}
              icon={BrainIcon}
              color='indigo'
            />
          </div>

          <div className='grid grid-cols-1 lg:grid-cols-2 gap-6'>
            <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
              <h3 className='text-lg font-semibold text-gray-900 dark:text-white mb-4'>
                Agile Metrics
              </h3>
              <div className='space-y-3'>
                <div className='flex justify-between'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Total Story Points
                  </span>
                  <span className='font-medium'>
                    {summary.agileSummary.totalStoryPoints}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Completed Story Points
                  </span>
                  <span className='font-medium'>
                    {summary.agileSummary.completedStoryPoints}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Average Velocity
                  </span>
                  <span className='font-medium'>
                    {summary.agileSummary.averageVelocity.toFixed(2)}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Average Burndown Rate
                  </span>
                  <span className='font-medium'>
                    {summary.agileSummary.averageBurndownRate.toFixed(2)}
                  </span>
                </div>
              </div>
            </div>

            <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
              <h3 className='text-lg font-semibold text-gray-900 dark:text-white mb-4'>
                Financial Overview
              </h3>
              <div className='space-y-3'>
                <div className='flex justify-between'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Total Investment
                  </span>
                  <span className='font-medium'>
                    {analyticsService.formatCurrency(
                      summary.roiSummary.totalInvestment
                    )}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Total Return
                  </span>
                  <span className='font-medium'>
                    {analyticsService.formatCurrency(
                      summary.roiSummary.totalReturn
                    )}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Total Labor Cost
                  </span>
                  <span className='font-medium'>
                    {analyticsService.formatCurrency(
                      summary.roiSummary.totalLaborCost
                    )}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-gray-600 dark:text-gray-400'>
                    Total Infrastructure Cost
                  </span>
                  <span className='font-medium'>
                    {analyticsService.formatCurrency(
                      summary.roiSummary.totalInfrastructureCost
                    )}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Dashboards Tab */}
      {activeTab === 'dashboards' && (
        <div className='space-y-6'>
          <div className='flex justify-between items-center'>
            <h2 className='text-xl font-semibold text-gray-900 dark:text-white'>
              Custom Dashboards
            </h2>
            <button
              onClick={() => openCreateModal('dashboard')}
              className='bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2'
            >
              <PlusIcon className='h-5 w-5' />
              <span>Create Dashboard</span>
            </button>
          </div>

          <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
            {dashboards?.map((dashboard) => (
              <div
                key={dashboard.id}
                className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'
              >
                <div className='flex justify-between items-start mb-4'>
                  <div>
                    <h3 className='text-lg font-semibold text-gray-900 dark:text-white'>
                      {dashboard.name}
                    </h3>
                    {dashboard.description && (
                      <p className='text-gray-600 dark:text-gray-400 text-sm mt-1'>
                        {dashboard.description}
                      </p>
                    )}
                  </div>
                  {dashboard.isDefault && (
                    <span className='bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full'>
                      Default
                    </span>
                  )}
                </div>

                <div className='space-y-2 mb-4'>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Widgets
                    </span>
                    <span className='font-medium'>
                      {dashboard.widgets.length}
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Created by
                    </span>
                    <span className='font-medium'>{dashboard.createdBy}</span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Created
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatDate(dashboard.createdAt)}
                    </span>
                  </div>
                </div>

                <div className='flex space-x-2'>
                  <button
                    onClick={() => setSelectedDashboard(dashboard)}
                    className='flex-1 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300 px-3 py-2 rounded text-sm flex items-center justify-center space-x-1'
                  >
                    <EyeIcon className='h-4 w-4' />
                    <span>View</span>
                  </button>
                  <button
                    onClick={() => openCreateModal('widget')}
                    className='flex-1 bg-blue-100 hover:bg-blue-200 dark:bg-blue-900 dark:hover:bg-blue-800 text-blue-700 dark:text-blue-300 px-3 py-2 rounded text-sm flex items-center justify-center space-x-1'
                  >
                    <PlusIcon className='h-4 w-4' />
                    <span>Add Widget</span>
                  </button>
                  <button
                    onClick={() => handleDelete('dashboard', dashboard.id)}
                    className='bg-red-100 hover:bg-red-200 dark:bg-red-900 dark:hover:bg-red-800 text-red-700 dark:text-red-300 px-3 py-2 rounded text-sm'
                  >
                    <TrashIcon className='h-4 w-4' />
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Metrics Tab */}
      {activeTab === 'metrics' && (
        <div className='space-y-6'>
          <div className='flex justify-between items-center'>
            <h2 className='text-xl font-semibold text-gray-900 dark:text-white'>
              Analytics Metrics
            </h2>
            <button
              onClick={() => openCreateModal('metric')}
              className='bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2'
            >
              <PlusIcon className='h-5 w-5' />
              <span>Create Metric</span>
            </button>
          </div>

          <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
            {metrics?.map((metric) => (
              <div
                key={metric.id}
                className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'
              >
                <div className='flex justify-between items-start mb-4'>
                  <div>
                    <h3 className='text-lg font-semibold text-gray-900 dark:text-white'>
                      {metric.name}
                    </h3>
                    <p className='text-gray-600 dark:text-gray-400 text-sm mt-1'>
                      Category: {metric.category}
                    </p>
                  </div>
                </div>

                <div className='space-y-2 mb-4'>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Last Calculated
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatDateTime(metric.lastCalculated)}
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Values
                    </span>
                    <span className='font-medium'>{metric.values.length}</span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Created
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatDate(metric.createdAt)}
                    </span>
                  </div>
                </div>

                <div className='flex space-x-2'>
                  <button
                    onClick={() => openCreateModal('calculate')}
                    className='flex-1 bg-green-100 hover:bg-green-200 dark:bg-green-900 dark:hover:bg-green-800 text-green-700 dark:text-green-300 px-3 py-2 rounded text-sm'
                  >
                    Calculate
                  </button>
                  <button
                    onClick={() => handleDelete('metric', metric.id)}
                    className='bg-red-100 hover:bg-red-200 dark:bg-red-900 dark:hover:bg-red-800 text-red-700 dark:text-red-300 px-3 py-2 rounded text-sm'
                  >
                    <TrashIcon className='h-4 w-4' />
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Resource Utilization Tab */}
      {activeTab === 'utilization' && (
        <div className='space-y-6'>
          <div className='flex justify-between items-center'>
            <h2 className='text-xl font-semibold text-gray-900 dark:text-white'>
              Resource Utilization
            </h2>
            <button
              onClick={() => openCreateModal('utilization')}
              className='bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2'
            >
              <PlusIcon className='h-5 w-5' />
              <span>Add Utilization</span>
            </button>
          </div>

          <div className='bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden'>
            <table className='min-w-full divide-y divide-gray-200 dark:divide-gray-700'>
              <thead className='bg-gray-50 dark:bg-gray-700'>
                <tr>
                  <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                    User
                  </th>
                  <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                    Date
                  </th>
                  <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                    Hours Worked
                  </th>
                  <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                    Hours Allocated
                  </th>
                  <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                    Utilization Rate
                  </th>
                  <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className='bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700'>
                {resourceUtilizations?.map((utilization) => (
                  <tr key={utilization.id}>
                    <td className='px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white'>
                      {utilization.userName}
                    </td>
                    <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                      {analyticsService.formatDate(utilization.date)}
                    </td>
                    <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                      {utilization.hoursWorked}
                    </td>
                    <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                      {utilization.hoursAllocated}
                    </td>
                    <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                      {analyticsService.formatUtilizationRate(
                        utilization.utilizationRate
                      )}
                    </td>
                    <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                      <button
                        onClick={() =>
                          handleDelete('utilization', utilization.id)
                        }
                        className='text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300'
                      >
                        <TrashIcon className='h-4 w-4' />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ROI Tracking Tab */}
      {activeTab === 'roi' && (
        <div className='space-y-6'>
          <div className='flex justify-between items-center'>
            <h2 className='text-xl font-semibold text-gray-900 dark:text-white'>
              ROI Tracking
            </h2>
            <button
              onClick={() => openCreateModal('roi')}
              className='bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2'
            >
              <PlusIcon className='h-5 w-5' />
              <span>Add ROI Tracking</span>
            </button>
          </div>

          <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
            {roiTracking?.map((roi) => (
              <div
                key={roi.id}
                className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'
              >
                <div className='mb-4'>
                  <h3 className='text-lg font-semibold text-gray-900 dark:text-white'>
                    {roi.projectName}
                  </h3>
                  <p className='text-gray-600 dark:text-gray-400 text-sm mt-1'>
                    Project ID: {roi.projectId}
                  </p>
                </div>

                <div className='space-y-2 mb-4'>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Investment
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatCurrency(roi.investment)}
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Return
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatCurrency(roi.return)}
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      ROI
                    </span>
                    <span
                      className={`font-medium ${
                        roi.roi >= 0 ? 'text-green-600' : 'text-red-600'
                      }`}
                    >
                      {analyticsService.formatROI(roi.roi)}
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Labor Cost
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatCurrency(roi.laborCost)}
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Infrastructure Cost
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatCurrency(roi.infrastructureCost)}
                    </span>
                  </div>
                </div>

                <div className='flex space-x-2'>
                  <button
                    onClick={() => openCreateModal('roi-details')}
                    className='flex-1 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300 px-3 py-2 rounded text-sm'
                  >
                    View Details
                  </button>
                  <button
                    onClick={() => handleDelete('roi', roi.id)}
                    className='bg-red-100 hover:bg-red-200 dark:bg-red-900 dark:hover:bg-red-800 text-red-700 dark:text-red-300 px-3 py-2 rounded text-sm'
                  >
                    <TrashIcon className='h-4 w-4' />
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Predictive Analytics Tab */}
      {activeTab === 'predictive' && (
        <div className='space-y-6'>
          <div className='flex justify-between items-center'>
            <h2 className='text-xl font-semibold text-gray-900 dark:text-white'>
              Predictive Analytics
            </h2>
            <button
              onClick={() => openCreateModal('predictive')}
              className='bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2'
            >
              <PlusIcon className='h-5 w-5' />
              <span>Create Model</span>
            </button>
          </div>

          <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
            {predictiveAnalytics?.map((model) => (
              <div
                key={model.id}
                className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'
              >
                <div className='mb-4'>
                  <h3 className='text-lg font-semibold text-gray-900 dark:text-white'>
                    {model.modelName}
                  </h3>
                  <p className='text-gray-600 dark:text-gray-400 text-sm mt-1'>
                    Type: {model.type}
                  </p>
                </div>

                <div className='space-y-2 mb-4'>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Accuracy
                    </span>
                    <span className='font-medium'>
                      {(model.accuracy * 100).toFixed(1)}%
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Last Trained
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatDate(model.lastTrained)}
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Predictions
                    </span>
                    <span className='font-medium'>
                      {model.predictions.length}
                    </span>
                  </div>
                  <div className='flex justify-between text-sm'>
                    <span className='text-gray-600 dark:text-gray-400'>
                      Created
                    </span>
                    <span className='font-medium'>
                      {analyticsService.formatDate(model.createdAt)}
                    </span>
                  </div>
                </div>

                <div className='flex space-x-2'>
                  <button
                    onClick={() => openCreateModal('prediction')}
                    className='flex-1 bg-green-100 hover:bg-green-200 dark:bg-green-900 dark:hover:bg-green-800 text-green-700 dark:text-green-300 px-3 py-2 rounded text-sm'
                  >
                    Make Prediction
                  </button>
                  <button
                    onClick={() => handleDelete('predictive', model.id)}
                    className='bg-red-100 hover:bg-red-200 dark:bg-red-900 dark:hover:bg-red-800 text-red-700 dark:text-red-300 px-3 py-2 rounded text-sm'
                  >
                    <TrashIcon className='h-4 w-4' />
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Create Modal */}
      {showCreateModal && (
        <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50'>
          <div className='bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md'>
            <h3 className='text-lg font-semibold text-gray-900 dark:text-white mb-4'>
              Create{' '}
              {createModalType.charAt(0).toUpperCase() +
                createModalType.slice(1)}
            </h3>
            <p className='text-gray-600 dark:text-gray-400 mb-4'>
              This feature is coming soon. The backend API is ready, but the
              form interface needs to be implemented.
            </p>
            <div className='flex space-x-3'>
              <button
                onClick={() => setShowCreateModal(false)}
                className='flex-1 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300 px-4 py-2 rounded'
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Dashboard Details Modal */}
      {selectedDashboard && (
        <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50'>
          <div className='bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-4xl max-h-[90vh] overflow-y-auto'>
            <div className='flex justify-between items-start mb-4'>
              <h3 className='text-lg font-semibold text-gray-900 dark:text-white'>
                {selectedDashboard.name}
              </h3>
              <button
                onClick={() => setSelectedDashboard(null)}
                className='text-gray-400 hover:text-gray-600 dark:hover:text-gray-300'
              >
                ×
              </button>
            </div>

            {selectedDashboard.description && (
              <p className='text-gray-600 dark:text-gray-400 mb-4'>
                {selectedDashboard.description}
              </p>
            )}

            <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4'>
              {selectedDashboard.widgets.map((widget) => (
                <div
                  key={widget.id}
                  className='bg-gray-50 dark:bg-gray-700 rounded p-4'
                >
                  <h4 className='font-medium text-gray-900 dark:text-white mb-2'>
                    {widget.name}
                  </h4>
                  <p className='text-sm text-gray-600 dark:text-gray-400'>
                    Type: {widget.type}
                  </p>
                  <p className='text-sm text-gray-600 dark:text-gray-400'>
                    Position: {widget.position}
                  </p>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

// Summary Card Component
interface SummaryCardProps {
  title: string;
  value: string | number;
  subtitle: string;
  icon: React.ComponentType<{ className?: string }>;
  color: string;
}

const SummaryCard: React.FC<SummaryCardProps> = ({
  title,
  value,
  subtitle,
  icon: Icon,
  color,
}) => {
  const colorClasses = {
    blue: 'bg-blue-100 text-blue-600 dark:bg-blue-900 dark:text-blue-400',
    green: 'bg-green-100 text-green-600 dark:bg-green-900 dark:text-green-400',
    purple:
      'bg-purple-100 text-purple-600 dark:bg-purple-900 dark:text-purple-400',
    orange:
      'bg-orange-100 text-orange-600 dark:bg-orange-900 dark:text-orange-400',
    indigo:
      'bg-indigo-100 text-indigo-600 dark:bg-indigo-900 dark:text-indigo-400',
    red: 'bg-red-100 text-red-600 dark:bg-red-900 dark:text-red-400',
  };

  return (
    <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
      <div className='flex items-center'>
        <div
          className={`p-3 rounded-lg ${
            colorClasses[color as keyof typeof colorClasses]
          }`}
        >
          <Icon className='h-6 w-6' />
        </div>
        <div className='ml-4'>
          <p className='text-sm font-medium text-gray-600 dark:text-gray-400'>
            {title}
          </p>
          <p className='text-2xl font-semibold text-gray-900 dark:text-white'>
            {value}
          </p>
          <p className='text-sm text-gray-500 dark:text-gray-400'>{subtitle}</p>
        </div>
      </div>
    </div>
  );
};

export default AnalyticsPage;
