import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  ChartBarIcon,
  CogIcon,
  ExclamationTriangleIcon,
  ServerIcon,
  ClockIcon,
  DatabaseIcon,
  CpuChipIcon,
  MemoryIcon,
} from '@heroicons/react/24/outline';
import { performanceService } from '../services/performanceService';

const PerformancePage: React.FC = () => {
  const [activeTab, setActiveTab] = useState('summary');
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [createModalType, setCreateModalType] = useState('');
  const queryClient = useQueryClient();

  // Queries
  const { data: summary } = useQuery({
    queryKey: ['performance-summary'],
    queryFn: performanceService.getPerformanceSummary,
  });

  const { data: cacheEntries } = useQuery({
    queryKey: ['cache-entries'],
    queryFn: performanceService.getCacheEntries,
  });

  const { data: metrics } = useQuery({
    queryKey: ['performance-metrics'],
    queryFn: () => performanceService.getPerformanceMetrics(),
  });

  const { data: optimizations } = useQuery({
    queryKey: ['database-optimizations'],
    queryFn: performanceService.getDatabaseOptimizations,
  });

  const { data: apiLogs } = useQuery({
    queryKey: ['api-performance-logs'],
    queryFn: () => performanceService.getApiPerformanceLogs(),
  });

  const { data: memoryUsage } = useQuery({
    queryKey: ['memory-usage'],
    queryFn: () => performanceService.getMemoryUsage(),
  });

  const { data: cpuUsage } = useQuery({
    queryKey: ['cpu-usage'],
    queryFn: () => performanceService.getCpuUsage(),
  });

  const { data: alerts } = useQuery({
    queryKey: ['performance-alerts'],
    queryFn: performanceService.getPerformanceAlerts,
  });

  // Mutations
  const deleteCacheEntryMutation = useMutation({
    mutationFn: performanceService.deleteCacheEntry,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['cache-entries'] });
    },
  });

  const deleteMetricMutation = useMutation({
    mutationFn: performanceService.deletePerformanceMetric,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['performance-metrics'] });
    },
  });

  const deleteOptimizationMutation = useMutation({
    mutationFn: performanceService.deleteDatabaseOptimization,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['database-optimizations'] });
    },
  });

  const executeOptimizationMutation = useMutation({
    mutationFn: performanceService.executeOptimization,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['database-optimizations'] });
    },
  });

  const resolveAlertMutation = useMutation({
    mutationFn: performanceService.resolveAlert,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['performance-alerts'] });
    },
  });

  const tabs = [
    { id: 'summary', name: 'Summary', icon: ChartBarIcon },
    { id: 'cache', name: 'Cache', icon: CogIcon },
    { id: 'metrics', name: 'Metrics', icon: ServerIcon },
    { id: 'database', name: 'Database', icon: DatabaseIcon },
    { id: 'api', name: 'API Logs', icon: ClockIcon },
    { id: 'system', name: 'System', icon: CpuChipIcon },
    { id: 'alerts', name: 'Alerts', icon: ExclamationTriangleIcon },
  ];

  const handleDeleteCacheEntry = (key: string) => {
    if (window.confirm('Are you sure you want to delete this cache entry?')) {
      deleteCacheEntryMutation.mutate(key);
    }
  };

  const handleDeleteMetric = (id: number) => {
    if (window.confirm('Are you sure you want to delete this metric?')) {
      deleteMetricMutation.mutate(id);
    }
  };

  const handleDeleteOptimization = (id: number) => {
    if (window.confirm('Are you sure you want to delete this optimization?')) {
      deleteOptimizationMutation.mutate(id);
    }
  };

  const handleExecuteOptimization = (id: number) => {
    executeOptimizationMutation.mutate(id);
  };

  const handleResolveAlert = (id: number) => {
    resolveAlertMutation.mutate(id);
  };

  const renderSummary = () => (
    <div className='space-y-6'>
      <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6'>
        {/* Cache Summary */}
        <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
          <div className='flex items-center'>
            <CogIcon className='h-8 w-8 text-blue-500' />
            <div className='ml-4'>
              <h3 className='text-lg font-medium text-gray-900 dark:text-gray-100'>
                Cache
              </h3>
              <p className='text-2xl font-bold text-blue-600'>
                {summary?.cacheSummary.totalEntries || 0}
              </p>
              <p className='text-sm text-gray-500'>Total Entries</p>
            </div>
          </div>
          <div className='mt-4 space-y-2'>
            <div className='flex justify-between text-sm'>
              <span>Hit Rate:</span>
              <span>
                {performanceService.formatPercentage(
                  summary?.cacheSummary.hitRate || 0
                )}
              </span>
            </div>
            <div className='flex justify-between text-sm'>
              <span>Expired:</span>
              <span>{summary?.cacheSummary.expiredEntries || 0}</span>
            </div>
          </div>
        </div>

        {/* API Summary */}
        <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
          <div className='flex items-center'>
            <ClockIcon className='h-8 w-8 text-green-500' />
            <div className='ml-4'>
              <h3 className='text-lg font-medium text-gray-900 dark:text-gray-100'>
                API
              </h3>
              <p className='text-2xl font-bold text-green-600'>
                {summary?.apiSummary.totalRequests || 0}
              </p>
              <p className='text-sm text-gray-500'>Total Requests</p>
            </div>
          </div>
          <div className='mt-4 space-y-2'>
            <div className='flex justify-between text-sm'>
              <span>Avg Response:</span>
              <span>
                {performanceService.formatMilliseconds(
                  summary?.apiSummary.averageResponseTime || 0
                )}
              </span>
            </div>
            <div className='flex justify-between text-sm'>
              <span>Success Rate:</span>
              <span>
                {performanceService.formatPercentage(
                  summary?.apiSummary.successRate || 0
                )}
              </span>
            </div>
          </div>
        </div>

        {/* System Summary */}
        <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
          <div className='flex items-center'>
            <CpuChipIcon className='h-8 w-8 text-purple-500' />
            <div className='ml-4'>
              <h3 className='text-lg font-medium text-gray-900 dark:text-gray-100'>
                System
              </h3>
              <p className='text-2xl font-bold text-purple-600'>
                {performanceService.formatPercentage(
                  summary?.systemSummary.averageCpuUsage || 0
                )}
              </p>
              <p className='text-sm text-gray-500'>CPU Usage</p>
            </div>
          </div>
          <div className='mt-4 space-y-2'>
            <div className='flex justify-between text-sm'>
              <span>Memory:</span>
              <span>
                {performanceService.formatPercentage(
                  summary?.systemSummary.averageMemoryUsage || 0
                )}
              </span>
            </div>
            <div className='flex justify-between text-sm'>
              <span>Instances:</span>
              <span>{summary?.systemSummary.totalServerInstances || 0}</span>
            </div>
          </div>
        </div>

        {/* Database Summary */}
        <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
          <div className='flex items-center'>
            <DatabaseIcon className='h-8 w-8 text-orange-500' />
            <div className='ml-4'>
              <h3 className='text-lg font-medium text-gray-900 dark:text-gray-100'>
                Database
              </h3>
              <p className='text-2xl font-bold text-orange-600'>
                {summary?.databaseSummary.totalOptimizations || 0}
              </p>
              <p className='text-sm text-gray-500'>Optimizations</p>
            </div>
          </div>
          <div className='mt-4 space-y-2'>
            <div className='flex justify-between text-sm'>
              <span>Success Rate:</span>
              <span>
                {summary?.databaseSummary.totalOptimizations > 0
                  ? performanceService.formatPercentage(
                      (summary.databaseSummary.successfulOptimizations /
                        summary.databaseSummary.totalOptimizations) *
                        100
                    )
                  : '0%'}
              </span>
            </div>
            <div className='flex justify-between text-sm'>
              <span>Avg Improvement:</span>
              <span>
                {performanceService.formatPercentage(
                  summary?.databaseSummary.averageImprovement || 0
                )}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );

  const renderCache = () => (
    <div className='space-y-6'>
      <div className='flex justify-between items-center'>
        <h2 className='text-xl font-semibold text-gray-900 dark:text-gray-100'>
          Cache Management
        </h2>
        <button
          onClick={() => {
            setCreateModalType('cache');
            setShowCreateModal(true);
          }}
          className='bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700'
        >
          Add Cache Entry
        </button>
      </div>

      <div className='bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden'>
        <div className='overflow-x-auto'>
          <table className='min-w-full divide-y divide-gray-200 dark:divide-gray-700'>
            <thead className='bg-gray-50 dark:bg-gray-700'>
              <tr>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Key
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Type
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Access Count
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Expires At
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className='bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700'>
              {cacheEntries?.map((entry) => (
                <tr key={entry.id}>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100'>
                    {entry.key}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {entry.cacheType || 'N/A'}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {entry.accessCount}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {performanceService.formatDateTime(entry.expiresAt)}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium'>
                    <button
                      onClick={() => handleDeleteCacheEntry(entry.key)}
                      className='text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300'
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );

  const renderMetrics = () => (
    <div className='space-y-6'>
      <div className='flex justify-between items-center'>
        <h2 className='text-xl font-semibold text-gray-900 dark:text-gray-100'>
          Performance Metrics
        </h2>
        <button
          onClick={() => {
            setCreateModalType('metric');
            setShowCreateModal(true);
          }}
          className='bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700'
        >
          Add Metric
        </button>
      </div>

      <div className='bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden'>
        <div className='overflow-x-auto'>
          <table className='min-w-full divide-y divide-gray-200 dark:divide-gray-700'>
            <thead className='bg-gray-50 dark:bg-gray-700'>
              <tr>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Name
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Category
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Value
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Unit
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Recorded At
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className='bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700'>
              {metrics?.map((metric) => (
                <tr key={metric.id}>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100'>
                    {metric.metricName}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {metric.category}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {metric.value}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {metric.unit}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {performanceService.formatDateTime(metric.recordedAt)}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium'>
                    <button
                      onClick={() => handleDeleteMetric(metric.id)}
                      className='text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300'
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );

  const renderDatabase = () => (
    <div className='space-y-6'>
      <div className='flex justify-between items-center'>
        <h2 className='text-xl font-semibold text-gray-900 dark:text-gray-100'>
          Database Optimizations
        </h2>
        <button
          onClick={() => {
            setCreateModalType('optimization');
            setShowCreateModal(true);
          }}
          className='bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700'
        >
          Add Optimization
        </button>
      </div>

      <div className='bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden'>
        <div className='overflow-x-auto'>
          <table className='min-w-full divide-y divide-gray-200 dark:divide-gray-700'>
            <thead className='bg-gray-50 dark:bg-gray-700'>
              <tr>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Table
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Type
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Status
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Improvement
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Executed At
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className='bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700'>
              {optimizations?.map((optimization) => (
                <tr key={optimization.id}>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100'>
                    {optimization.tableName}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {optimization.optimizationType}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <span
                      className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        optimization.isSuccessful
                          ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                          : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
                      }`}
                    >
                      {optimization.isSuccessful ? 'Success' : 'Failed'}
                    </span>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {performanceService.formatPercentage(
                      optimization.improvementPercentage
                    )}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {performanceService.formatDateTime(optimization.executedAt)}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2'>
                    {!optimization.isSuccessful && (
                      <button
                        onClick={() =>
                          handleExecuteOptimization(optimization.id)
                        }
                        className='text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300'
                      >
                        Execute
                      </button>
                    )}
                    <button
                      onClick={() => handleDeleteOptimization(optimization.id)}
                      className='text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300'
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );

  const renderApiLogs = () => (
    <div className='space-y-6'>
      <h2 className='text-xl font-semibold text-gray-900 dark:text-gray-100'>
        API Performance Logs
      </h2>

      <div className='bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden'>
        <div className='overflow-x-auto'>
          <table className='min-w-full divide-y divide-gray-200 dark:divide-gray-700'>
            <thead className='bg-gray-50 dark:bg-gray-700'>
              <tr>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Method
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Endpoint
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Status
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Response Time
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  User
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Requested At
                </th>
              </tr>
            </thead>
            <tbody className='bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700'>
              {apiLogs?.slice(0, 20).map((log) => (
                <tr key={log.id}>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100'>
                    {log.httpMethod}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {log.endpoint}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <span
                      className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        log.statusCode < 400
                          ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                          : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
                      }`}
                    >
                      {log.statusCode}
                    </span>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {performanceService.formatMilliseconds(log.responseTime)}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {log.userName || 'N/A'}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {performanceService.formatDateTime(log.requestedAt)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );

  const renderSystem = () => (
    <div className='space-y-6'>
      <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
        {/* Memory Usage */}
        <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
          <div className='flex items-center mb-4'>
            <MemoryIcon className='h-6 w-6 text-blue-500 mr-2' />
            <h3 className='text-lg font-medium text-gray-900 dark:text-gray-100'>
              Memory Usage
            </h3>
          </div>
          <div className='space-y-4'>
            {memoryUsage?.slice(0, 10).map((usage) => (
              <div
                key={usage.id}
                className='border-b border-gray-200 dark:border-gray-700 pb-2'
              >
                <div className='flex justify-between text-sm'>
                  <span>Usage:</span>
                  <span>
                    {performanceService.formatPercentage(
                      usage.memoryUsagePercentage
                    )}
                  </span>
                </div>
                <div className='flex justify-between text-sm text-gray-500'>
                  <span>
                    Total:{' '}
                    {performanceService.formatBytes(
                      usage.totalMemory * 1024 * 1024
                    )}
                  </span>
                  <span>
                    Used:{' '}
                    {performanceService.formatBytes(
                      usage.usedMemory * 1024 * 1024
                    )}
                  </span>
                </div>
                <div className='text-xs text-gray-400'>
                  {performanceService.formatDateTime(usage.recordedAt)}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* CPU Usage */}
        <div className='bg-white dark:bg-gray-800 rounded-lg shadow p-6'>
          <div className='flex items-center mb-4'>
            <CpuChipIcon className='h-6 w-6 text-green-500 mr-2' />
            <h3 className='text-lg font-medium text-gray-900 dark:text-gray-100'>
              CPU Usage
            </h3>
          </div>
          <div className='space-y-4'>
            {cpuUsage?.slice(0, 10).map((usage) => (
              <div
                key={usage.id}
                className='border-b border-gray-200 dark:border-gray-700 pb-2'
              >
                <div className='flex justify-between text-sm'>
                  <span>Usage:</span>
                  <span>
                    {performanceService.formatPercentage(
                      usage.cpuUsagePercentage
                    )}
                  </span>
                </div>
                <div className='flex justify-between text-sm text-gray-500'>
                  <span>Load: {usage.loadAverage.toFixed(2)}</span>
                  <span>Threads: {usage.activeThreads}</span>
                </div>
                <div className='text-xs text-gray-400'>
                  {performanceService.formatDateTime(usage.recordedAt)}
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );

  const renderAlerts = () => (
    <div className='space-y-6'>
      <div className='flex justify-between items-center'>
        <h2 className='text-xl font-semibold text-gray-900 dark:text-gray-100'>
          Performance Alerts
        </h2>
        <button
          onClick={() => {
            setCreateModalType('alert');
            setShowCreateModal(true);
          }}
          className='bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700'
        >
          Create Alert
        </button>
      </div>

      <div className='bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden'>
        <div className='overflow-x-auto'>
          <table className='min-w-full divide-y divide-gray-200 dark:divide-gray-700'>
            <thead className='bg-gray-50 dark:bg-gray-700'>
              <tr>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Type
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Metric
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Severity
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Status
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Triggered At
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider'>
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className='bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700'>
              {alerts?.map((alert) => (
                <tr key={alert.id}>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100'>
                    {alert.alertType}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {alert.metricName}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <span
                      className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${performanceService.getSeverityBgColor(
                        alert.severity
                      )}`}
                    >
                      {alert.severity}
                    </span>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <span
                      className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        alert.isResolved
                          ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                          : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
                      }`}
                    >
                      {alert.isResolved ? 'Resolved' : 'Active'}
                    </span>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400'>
                    {performanceService.formatDateTime(alert.triggeredAt)}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium'>
                    {!alert.isResolved && (
                      <button
                        onClick={() => handleResolveAlert(alert.id)}
                        className='text-green-600 hover:text-green-900 dark:text-green-400 dark:hover:text-green-300'
                      >
                        Resolve
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );

  const renderContent = () => {
    switch (activeTab) {
      case 'summary':
        return renderSummary();
      case 'cache':
        return renderCache();
      case 'metrics':
        return renderMetrics();
      case 'database':
        return renderDatabase();
      case 'api':
        return renderApiLogs();
      case 'system':
        return renderSystem();
      case 'alerts':
        return renderAlerts();
      default:
        return renderSummary();
    }
  };

  return (
    <div className='space-y-6'>
      <div className='sm:flex sm:items-center sm:justify-between'>
        <div>
          <h1 className='text-2xl font-bold text-gray-900 dark:text-gray-100'>
            Performance & Caching
          </h1>
          <p className='mt-2 text-sm text-gray-700 dark:text-gray-300'>
            Monitor and optimize application performance, manage caching, and
            track system metrics.
          </p>
        </div>
      </div>

      {/* Tabs */}
      <div className='border-b border-gray-200 dark:border-gray-700'>
        <nav className='-mb-px flex space-x-8'>
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`${
                activeTab === tab.id
                  ? 'border-blue-500 text-blue-600 dark:text-blue-400'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-400 dark:hover:text-gray-300'
              } whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm flex items-center`}
            >
              <tab.icon className='h-4 w-4 mr-2' />
              {tab.name}
            </button>
          ))}
        </nav>
      </div>

      {/* Content */}
      {renderContent()}

      {/* Create Modal (placeholder) */}
      {showCreateModal && (
        <div className='fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50'>
          <div className='relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800'>
            <div className='mt-3 text-center'>
              <h3 className='text-lg font-medium text-gray-900 dark:text-gray-100'>
                Create{' '}
                {createModalType.charAt(0).toUpperCase() +
                  createModalType.slice(1)}
              </h3>
              <div className='mt-2 px-7 py-3'>
                <p className='text-sm text-gray-500 dark:text-gray-400'>
                  Modal implementation would go here for creating{' '}
                  {createModalType}.
                </p>
              </div>
              <div className='items-center px-4 py-3'>
                <button
                  onClick={() => setShowCreateModal(false)}
                  className='px-4 py-2 bg-gray-500 text-white text-base font-medium rounded-md w-full shadow-sm hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-300'
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default PerformancePage;
