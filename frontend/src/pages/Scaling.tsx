import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  ChartBarIcon,
  CogIcon,
  ServerIcon,
  ScaleIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  XCircleIcon,
  PlusIcon,
  PencilIcon,
  TrashIcon,
  ArrowUpIcon,
  ArrowDownIcon,
  ClockIcon,
  CpuChipIcon,
  ServerStackIcon,
} from '@heroicons/react/24/outline';
import {
  scalingService,
  type LoadBalancer,
  type AutoScalingGroup,
  type ScalingPolicy,
  type ScalingEvent,
  type ScalingSummary,
} from '../services/scalingService';

const ScalingPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState('summary');
  const [selectedLoadBalancer, setSelectedLoadBalancer] =
    useState<LoadBalancer | null>(null);
  const [selectedAutoScalingGroup, setSelectedAutoScalingGroup] =
    useState<AutoScalingGroup | null>(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [createModalType, setCreateModalType] = useState<
    'loadBalancer' | 'autoScalingGroup' | 'scalingPolicy'
  >('loadBalancer');

  const queryClient = useQueryClient();

  // Queries
  const { data: summary, isLoading: summaryLoading } = useQuery({
    queryKey: ['scaling-summary'],
    queryFn: () => scalingService.getScalingSummary(),
  });

  const { data: loadBalancers, isLoading: loadBalancersLoading } = useQuery({
    queryKey: ['load-balancers'],
    queryFn: () => scalingService.getLoadBalancers(),
  });

  const { data: autoScalingGroups, isLoading: autoScalingGroupsLoading } =
    useQuery({
      queryKey: ['auto-scaling-groups'],
      queryFn: () => scalingService.getAutoScalingGroups(),
    });

  const { data: scalingPolicies, isLoading: scalingPoliciesLoading } = useQuery(
    {
      queryKey: ['scaling-policies'],
      queryFn: () => scalingService.getScalingPolicies(),
    }
  );

  const { data: scalingEvents, isLoading: scalingEventsLoading } = useQuery({
    queryKey: ['scaling-events'],
    queryFn: () => scalingService.getScalingEvents(),
  });

  // Mutations
  const deleteLoadBalancerMutation = useMutation({
    mutationFn: (id: number) => scalingService.deleteLoadBalancer(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['load-balancers'] });
      queryClient.invalidateQueries({ queryKey: ['scaling-summary'] });
    },
  });

  const deleteAutoScalingGroupMutation = useMutation({
    mutationFn: (id: number) => scalingService.deleteAutoScalingGroup(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auto-scaling-groups'] });
      queryClient.invalidateQueries({ queryKey: ['scaling-summary'] });
    },
  });

  const deleteScalingPolicyMutation = useMutation({
    mutationFn: (id: number) => scalingService.deleteScalingPolicy(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['scaling-policies'] });
      queryClient.invalidateQueries({ queryKey: ['scaling-summary'] });
    },
  });

  const executeScalingActionMutation = useMutation({
    mutationFn: (request: any) => scalingService.executeScalingAction(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['auto-scaling-groups'] });
      queryClient.invalidateQueries({ queryKey: ['scaling-events'] });
      queryClient.invalidateQueries({ queryKey: ['scaling-summary'] });
    },
  });

  const handleDeleteLoadBalancer = (id: number) => {
    if (window.confirm('Are you sure you want to delete this load balancer?')) {
      deleteLoadBalancerMutation.mutate(id);
    }
  };

  const handleDeleteAutoScalingGroup = (id: number) => {
    if (
      window.confirm('Are you sure you want to delete this auto scaling group?')
    ) {
      deleteAutoScalingGroupMutation.mutate(id);
    }
  };

  const handleDeleteScalingPolicy = (id: number) => {
    if (
      window.confirm('Are you sure you want to delete this scaling policy?')
    ) {
      deleteScalingPolicyMutation.mutate(id);
    }
  };

  const handleScalingAction = (
    groupId: number,
    action: string,
    adjustment?: number
  ) => {
    executeScalingActionMutation.mutate({
      autoScalingGroupId: groupId,
      action,
      adjustment,
      reason: `Manual ${action} action`,
    });
  };

  const tabs = [
    { id: 'summary', name: 'Summary', icon: ChartBarIcon },
    { id: 'load-balancers', name: 'Load Balancers', icon: ServerIcon },
    { id: 'auto-scaling', name: 'Auto Scaling', icon: ScaleIcon },
    { id: 'policies', name: 'Scaling Policies', icon: CogIcon },
    { id: 'events', name: 'Scaling Events', icon: ClockIcon },
  ];

  const renderSummary = () => {
    if (summaryLoading || !summary)
      return <div className='animate-pulse'>Loading summary...</div>;

    return (
      <div className='space-y-6'>
        {/* Overview Cards */}
        <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6'>
          <div className='bg-white rounded-lg shadow p-6'>
            <div className='flex items-center'>
              <ServerIcon className='h-8 w-8 text-blue-600' />
              <div className='ml-4'>
                <p className='text-sm font-medium text-gray-500'>
                  Load Balancers
                </p>
                <p className='text-2xl font-semibold text-gray-900'>
                  {summary.loadBalancerSummary.totalLoadBalancers}
                </p>
                <p className='text-sm text-green-600'>
                  {summary.loadBalancerSummary.activeLoadBalancers} active
                </p>
              </div>
            </div>
          </div>

          <div className='bg-white rounded-lg shadow p-6'>
            <div className='flex items-center'>
              <ScaleIcon className='h-8 w-8 text-green-600' />
              <div className='ml-4'>
                <p className='text-sm font-medium text-gray-500'>
                  Auto Scaling Groups
                </p>
                <p className='text-2xl font-semibold text-gray-900'>
                  {summary.autoScalingSummary.totalGroups}
                </p>
                <p className='text-sm text-green-600'>
                  {summary.autoScalingSummary.activeGroups} active
                </p>
              </div>
            </div>
          </div>

          <div className='bg-white rounded-lg shadow p-6'>
            <div className='flex items-center'>
              <CogIcon className='h-8 w-8 text-purple-600' />
              <div className='ml-4'>
                <p className='text-sm font-medium text-gray-500'>
                  Scaling Policies
                </p>
                <p className='text-2xl font-semibold text-gray-900'>
                  {summary.policySummary.totalPolicies}
                </p>
                <p className='text-sm text-green-600'>
                  {summary.policySummary.activePolicies} active
                </p>
              </div>
            </div>
          </div>

          <div className='bg-white rounded-lg shadow p-6'>
            <div className='flex items-center'>
              <ClockIcon className='h-8 w-8 text-orange-600' />
              <div className='ml-4'>
                <p className='text-sm font-medium text-gray-500'>
                  Events Today
                </p>
                <p className='text-2xl font-semibold text-gray-900'>
                  {summary.eventSummary.eventsToday}
                </p>
                <p className='text-sm text-green-600'>
                  {summary.eventSummary.successfulEvents} successful
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Detailed Metrics */}
        <div className='grid grid-cols-1 lg:grid-cols-2 gap-6'>
          <div className='bg-white rounded-lg shadow p-6'>
            <h3 className='text-lg font-medium text-gray-900 mb-4'>
              Load Balancer Metrics
            </h3>
            <div className='space-y-3'>
              <div className='flex justify-between'>
                <span className='text-sm text-gray-500'>Total Instances</span>
                <span className='font-medium'>
                  {summary.loadBalancerSummary.totalInstances}
                </span>
              </div>
              <div className='flex justify-between'>
                <span className='text-sm text-gray-500'>Healthy Instances</span>
                <span className='font-medium text-green-600'>
                  {summary.loadBalancerSummary.healthyInstances}
                </span>
              </div>
              <div className='flex justify-between'>
                <span className='text-sm text-gray-500'>
                  Average Response Time
                </span>
                <span className='font-medium'>
                  {summary.loadBalancerSummary.averageResponseTime.toFixed(2)}ms
                </span>
              </div>
              <div className='flex justify-between'>
                <span className='text-sm text-gray-500'>Total Requests</span>
                <span className='font-medium'>
                  {scalingService.formatMetricValue(
                    summary.loadBalancerSummary.totalRequests
                  )}
                </span>
              </div>
            </div>
          </div>

          <div className='bg-white rounded-lg shadow p-6'>
            <h3 className='text-lg font-medium text-gray-900 mb-4'>
              Auto Scaling Metrics
            </h3>
            <div className='space-y-3'>
              <div className='flex justify-between'>
                <span className='text-sm text-gray-500'>Total Instances</span>
                <span className='font-medium'>
                  {summary.autoScalingSummary.totalInstances}
                </span>
              </div>
              <div className='flex justify-between'>
                <span className='text-sm text-gray-500'>In Service</span>
                <span className='font-medium text-green-600'>
                  {summary.autoScalingSummary.inServiceInstances}
                </span>
              </div>
              <div className='flex justify-between'>
                <span className='text-sm text-gray-500'>Pending</span>
                <span className='font-medium text-yellow-600'>
                  {summary.autoScalingSummary.pendingInstances}
                </span>
              </div>
              <div className='flex justify-between'>
                <span className='text-sm text-gray-500'>
                  Avg CPU Utilization
                </span>
                <span className='font-medium'>
                  {summary.autoScalingSummary.averageCpuUtilization.toFixed(1)}%
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  };

  const renderLoadBalancers = () => {
    if (loadBalancersLoading)
      return <div className='animate-pulse'>Loading load balancers...</div>;

    return (
      <div className='space-y-6'>
        <div className='flex justify-between items-center'>
          <h2 className='text-2xl font-bold text-gray-900'>Load Balancers</h2>
          <button
            onClick={() => {
              setCreateModalType('loadBalancer');
              setShowCreateModal(true);
            }}
            className='bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 flex items-center'
          >
            <PlusIcon className='h-5 w-5 mr-2' />
            Create Load Balancer
          </button>
        </div>

        <div className='grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6'>
          {loadBalancers?.map((lb) => (
            <div key={lb.id} className='bg-white rounded-lg shadow p-6'>
              <div className='flex justify-between items-start mb-4'>
                <div>
                  <h3 className='text-lg font-semibold text-gray-900'>
                    {lb.name}
                  </h3>
                  <p className='text-sm text-gray-500'>
                    {lb.type} • {lb.algorithm}
                  </p>
                </div>
                <div className='flex space-x-2'>
                  <button
                    onClick={() => setSelectedLoadBalancer(lb)}
                    className='text-gray-400 hover:text-gray-600'
                  >
                    <PencilIcon className='h-5 w-5' />
                  </button>
                  <button
                    onClick={() => handleDeleteLoadBalancer(lb.id)}
                    className='text-red-400 hover:text-red-600'
                  >
                    <TrashIcon className='h-5 w-5' />
                  </button>
                </div>
              </div>

              <div className='space-y-2'>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Status</span>
                  <span
                    className={`px-2 py-1 rounded-full text-xs font-medium ${
                      lb.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-red-100 text-red-800'
                    }`}
                  >
                    {lb.isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Instances</span>
                  <span className='font-medium'>
                    {lb.instanceCount} ({lb.healthyInstanceCount} healthy)
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Health Check</span>
                  <span className='font-medium'>{lb.healthCheckInterval}s</span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Created</span>
                  <span className='font-medium'>
                    {new Date(lb.createdAt).toLocaleDateString()}
                  </span>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  };

  const renderAutoScaling = () => {
    if (autoScalingGroupsLoading)
      return (
        <div className='animate-pulse'>Loading auto scaling groups...</div>
      );

    return (
      <div className='space-y-6'>
        <div className='flex justify-between items-center'>
          <h2 className='text-2xl font-bold text-gray-900'>
            Auto Scaling Groups
          </h2>
          <button
            onClick={() => {
              setCreateModalType('autoScalingGroup');
              setShowCreateModal(true);
            }}
            className='bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 flex items-center'
          >
            <PlusIcon className='h-5 w-5 mr-2' />
            Create Auto Scaling Group
          </button>
        </div>

        <div className='grid grid-cols-1 lg:grid-cols-2 gap-6'>
          {autoScalingGroups?.map((group) => (
            <div key={group.id} className='bg-white rounded-lg shadow p-6'>
              <div className='flex justify-between items-start mb-4'>
                <div>
                  <h3 className='text-lg font-semibold text-gray-900'>
                    {group.name}
                  </h3>
                  <p className='text-sm text-gray-500'>{group.status}</p>
                </div>
                <div className='flex space-x-2'>
                  <button
                    onClick={() => setSelectedAutoScalingGroup(group)}
                    className='text-gray-400 hover:text-gray-600'
                  >
                    <PencilIcon className='h-5 w-5' />
                  </button>
                  <button
                    onClick={() => handleDeleteAutoScalingGroup(group.id)}
                    className='text-red-400 hover:text-red-600'
                  >
                    <TrashIcon className='h-5 w-5' />
                  </button>
                </div>
              </div>

              <div className='space-y-3'>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>
                    Current Capacity
                  </span>
                  <span className='font-medium'>{group.currentCapacity}</span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>
                    Desired Capacity
                  </span>
                  <span className='font-medium'>{group.desiredCapacity}</span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Min/Max Size</span>
                  <span className='font-medium'>
                    {group.minSize} / {group.maxSize}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>In Service</span>
                  <span className='font-medium text-green-600'>
                    {group.inServiceCapacity}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Pending</span>
                  <span className='font-medium text-yellow-600'>
                    {group.pendingCapacity}
                  </span>
                </div>
              </div>

              <div className='mt-4 flex space-x-2'>
                <button
                  onClick={() => handleScalingAction(group.id, 'ScaleUp', 1)}
                  className='flex-1 bg-green-600 text-white px-3 py-2 rounded text-sm hover:bg-green-700 flex items-center justify-center'
                >
                  <ArrowUpIcon className='h-4 w-4 mr-1' />
                  Scale Up
                </button>
                <button
                  onClick={() => handleScalingAction(group.id, 'ScaleDown', -1)}
                  className='flex-1 bg-red-600 text-white px-3 py-2 rounded text-sm hover:bg-red-700 flex items-center justify-center'
                >
                  <ArrowDownIcon className='h-4 w-4 mr-1' />
                  Scale Down
                </button>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  };

  const renderScalingPolicies = () => {
    if (scalingPoliciesLoading)
      return <div className='animate-pulse'>Loading scaling policies...</div>;

    return (
      <div className='space-y-6'>
        <div className='flex justify-between items-center'>
          <h2 className='text-2xl font-bold text-gray-900'>Scaling Policies</h2>
          <button
            onClick={() => {
              setCreateModalType('scalingPolicy');
              setShowCreateModal(true);
            }}
            className='bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 flex items-center'
          >
            <PlusIcon className='h-5 w-5 mr-2' />
            Create Scaling Policy
          </button>
        </div>

        <div className='grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6'>
          {scalingPolicies?.map((policy) => (
            <div key={policy.id} className='bg-white rounded-lg shadow p-6'>
              <div className='flex justify-between items-start mb-4'>
                <div>
                  <h3 className='text-lg font-semibold text-gray-900'>
                    {policy.name}
                  </h3>
                  <p className='text-sm text-gray-500'>
                    {policy.type} • {policy.metricName}
                  </p>
                </div>
                <div className='flex space-x-2'>
                  <button className='text-gray-400 hover:text-gray-600'>
                    <PencilIcon className='h-5 w-5' />
                  </button>
                  <button
                    onClick={() => handleDeleteScalingPolicy(policy.id)}
                    className='text-red-400 hover:text-red-600'
                  >
                    <TrashIcon className='h-5 w-5' />
                  </button>
                </div>
              </div>

              <div className='space-y-2'>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Status</span>
                  <span
                    className={`px-2 py-1 rounded-full text-xs font-medium ${
                      policy.isEnabled
                        ? 'bg-green-100 text-green-800'
                        : 'bg-red-100 text-red-800'
                    }`}
                  >
                    {policy.isEnabled ? 'Enabled' : 'Disabled'}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Threshold</span>
                  <span className='font-medium'>{policy.threshold}</span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Scale Up</span>
                  <span className='font-medium text-green-600'>
                    +{policy.scaleUpAdjustment}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Scale Down</span>
                  <span className='font-medium text-red-600'>
                    {policy.scaleDownAdjustment}
                  </span>
                </div>
                <div className='flex justify-between'>
                  <span className='text-sm text-gray-500'>Events</span>
                  <span className='font-medium'>{policy.eventCount}</span>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  };

  const renderScalingEvents = () => {
    if (scalingEventsLoading)
      return <div className='animate-pulse'>Loading scaling events...</div>;

    return (
      <div className='space-y-6'>
        <h2 className='text-2xl font-bold text-gray-900'>Scaling Events</h2>

        <div className='bg-white shadow rounded-lg overflow-hidden'>
          <table className='min-w-full divide-y divide-gray-200'>
            <thead className='bg-gray-50'>
              <tr>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Policy
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Action
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Reason
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Adjustment
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Status
                </th>
                <th className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                  Triggered
                </th>
              </tr>
            </thead>
            <tbody className='bg-white divide-y divide-gray-200'>
              {scalingEvents?.map((event) => (
                <tr key={event.id}>
                  <td className='px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900'>
                    {event.policyName}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500'>
                    {event.action}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500'>
                    {event.reason}
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500'>
                    <span
                      className={`font-medium ${
                        event.adjustment > 0 ? 'text-green-600' : 'text-red-600'
                      }`}
                    >
                      {event.adjustment > 0 ? '+' : ''}
                      {event.adjustment}
                    </span>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap'>
                    <span
                      className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        event.isSuccessful
                          ? 'bg-green-100 text-green-800'
                          : 'bg-red-100 text-red-800'
                      }`}
                    >
                      {event.isSuccessful ? 'Success' : 'Failed'}
                    </span>
                  </td>
                  <td className='px-6 py-4 whitespace-nowrap text-sm text-gray-500'>
                    {new Date(event.triggeredAt).toLocaleString()}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    );
  };

  return (
    <div className='min-h-screen bg-gray-50'>
      <div className='max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8'>
        <div className='mb-8'>
          <h1 className='text-3xl font-bold text-gray-900'>
            Scaling & Load Balancing
          </h1>
          <p className='mt-2 text-gray-600'>
            Manage load balancers, auto-scaling groups, and scaling policies
          </p>
        </div>

        {/* Tabs */}
        <div className='border-b border-gray-200 mb-8'>
          <nav className='-mb-px flex space-x-8'>
            {tabs.map((tab) => {
              const Icon = tab.icon;
              return (
                <button
                  key={tab.id}
                  onClick={() => setActiveTab(tab.id)}
                  className={`${
                    activeTab === tab.id
                      ? 'border-blue-500 text-blue-600'
                      : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  } whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm flex items-center`}
                >
                  <Icon className='h-5 w-5 mr-2' />
                  {tab.name}
                </button>
              );
            })}
          </nav>
        </div>

        {/* Tab Content */}
        {activeTab === 'summary' && renderSummary()}
        {activeTab === 'load-balancers' && renderLoadBalancers()}
        {activeTab === 'auto-scaling' && renderAutoScaling()}
        {activeTab === 'policies' && renderScalingPolicies()}
        {activeTab === 'events' && renderScalingEvents()}

        {/* Create Modal Placeholder */}
        {showCreateModal && (
          <div className='fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50'>
            <div className='relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white'>
              <div className='mt-3'>
                <h3 className='text-lg font-medium text-gray-900 mb-4'>
                  Create{' '}
                  {createModalType === 'loadBalancer'
                    ? 'Load Balancer'
                    : createModalType === 'autoScalingGroup'
                    ? 'Auto Scaling Group'
                    : 'Scaling Policy'}
                </h3>
                <p className='text-sm text-gray-500 mb-4'>
                  This feature will be implemented in the next phase.
                </p>
                <div className='flex justify-end'>
                  <button
                    onClick={() => setShowCreateModal(false)}
                    className='bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700'
                  >
                    Close
                  </button>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ScalingPage;
