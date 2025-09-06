import { api } from './api';

// Types for Load Balancing & Auto-scaling
export interface LoadBalancer {
  id: number;
  name: string;
  type: string;
  algorithm: string;
  isActive: boolean;
  healthCheckInterval: number;
  healthCheckTimeout: number;
  unhealthyThreshold: number;
  healthyThreshold: number;
  healthCheckPath?: string;
  configuration?: string;
  createdAt: string;
  updatedAt: string;
  organizationId: number;
  instanceCount: number;
  healthyInstanceCount: number;
}

export interface LoadBalancerInstance {
  id: number;
  instanceId: string;
  status: string;
  ipAddress: string;
  port: number;
  weight: number;
  isEnabled: boolean;
  lastHealthCheck: string;
  lastResponseTime: string;
  responseTime: number;
  activeConnections: number;
  totalRequests: number;
  failedRequests: number;
  createdAt: string;
  updatedAt: string;
  loadBalancerId: number;
}

export interface LoadBalancerRule {
  id: number;
  name: string;
  type: string;
  condition: string;
  action: string;
  priority: number;
  isEnabled: boolean;
  createdAt: string;
  updatedAt: string;
  loadBalancerId: number;
}

export interface ScalingPolicy {
  id: number;
  name: string;
  type: string;
  metricName: string;
  threshold: number;
  cooldownPeriod: number;
  scaleUpAdjustment: number;
  scaleDownAdjustment: number;
  minInstances: number;
  maxInstances: number;
  isEnabled: boolean;
  schedule?: string;
  conditions?: string;
  createdAt: string;
  updatedAt: string;
  organizationId: number;
  eventCount: number;
  lastEventId: number;
}

export interface ScalingEvent {
  id: number;
  action: string;
  reason: string;
  metricValue: number;
  threshold: number;
  instancesBefore: number;
  instancesAfter: number;
  adjustment: number;
  isSuccessful: boolean;
  errorMessage?: string;
  triggeredAt: string;
  completedAt?: string;
  scalingPolicyId: number;
  policyName: string;
}

export interface AutoScalingGroup {
  id: number;
  name: string;
  status: string;
  desiredCapacity: number;
  minSize: number;
  maxSize: number;
  currentCapacity: number;
  pendingCapacity: number;
  inServiceCapacity: number;
  launchTemplate?: string;
  vpcConfig?: string;
  tags?: string;
  createdAt: string;
  updatedAt: string;
  organizationId: number;
  instanceCount: number;
  policyCount: number;
}

export interface AutoScalingInstance {
  id: number;
  instanceId: string;
  status: string;
  ipAddress: string;
  privateIpAddress?: string;
  instanceType?: string;
  availabilityZone?: string;
  launchTime: string;
  terminationTime?: string;
  terminationReason?: string;
  cpuUtilization: number;
  memoryUtilization: number;
  activeConnections: number;
  lastHealthCheck: string;
  createdAt: string;
  updatedAt: string;
  autoScalingGroupId: number;
  groupName: string;
}

export interface LoadBalancerMetrics {
  id: number;
  loadBalancerId: number;
  requestCount: number;
  targetResponseTime: number;
  healthyHostCount: number;
  unhealthyHostCount: number;
  targetConnectionErrorCount: number;
  targetTLSNegotiationErrorCount: number;
  requestCountPerTarget: number;
  unHealthyHostCount: number;
  recordedAt: string;
  loadBalancerName: string;
}

export interface ScalingMetrics {
  id: number;
  autoScalingGroupId: number;
  cpuUtilization: number;
  memoryUtilization: number;
  networkIn: number;
  networkOut: number;
  diskReadOps: number;
  diskWriteOps: number;
  instanceCount: number;
  recordedAt: string;
  groupName: string;
}

export interface ScalingSummary {
  loadBalancerSummary: {
    totalLoadBalancers: number;
    activeLoadBalancers: number;
    totalInstances: number;
    healthyInstances: number;
    averageResponseTime: number;
    totalRequests: number;
    loadBalancerTypes: string[];
  };
  autoScalingSummary: {
    totalGroups: number;
    activeGroups: number;
    totalInstances: number;
    inServiceInstances: number;
    pendingInstances: number;
    averageCpuUtilization: number;
    averageMemoryUtilization: number;
    groupStatuses: string[];
  };
  policySummary: {
    totalPolicies: number;
    activePolicies: number;
    cpuBasedPolicies: number;
    memoryBasedPolicies: number;
    scheduledPolicies: number;
    policyTypes: string[];
  };
  eventSummary: {
    totalEvents: number;
    eventsToday: number;
    successfulEvents: number;
    failedEvents: number;
    scaleUpEvents: number;
    scaleDownEvents: number;
    averageAdjustment: number;
  };
}

export interface HealthCheckResponse {
  loadBalancerId: number;
  loadBalancerName: string;
  isHealthy: boolean;
  totalInstances: number;
  healthyInstances: number;
  unhealthyInstances: number;
  instanceStatuses: Array<{
    instanceId: number;
    instanceIdStr: string;
    status: string;
    ipAddress: string;
    responseTime: number;
    lastHealthCheck: string;
    errorMessage?: string;
  }>;
  checkedAt: string;
}

export interface ScalingActionResponse {
  autoScalingGroupId: number;
  groupName: string;
  action: string;
  isSuccessful: boolean;
  instancesBefore: number;
  instancesAfter: number;
  adjustment: number;
  executedAt: string;
}

export interface ScalingChartData {
  chartType: string;
  labels: string[];
  datasets: Array<{
    label: string;
    data: number[];
    backgroundColor: string;
    borderColor: string;
    borderWidth: number;
    fill: boolean;
  }>;
  options: {
    title?: string;
    xAxisLabel?: string;
    yAxisLabel?: string;
    responsive: boolean;
    maintainAspectRatio: boolean;
    showLegend: boolean;
  };
}

// Request types
export interface CreateLoadBalancerRequest {
  name: string;
  type: string;
  algorithm: string;
  healthCheckInterval?: number;
  healthCheckTimeout?: number;
  unhealthyThreshold?: number;
  healthyThreshold?: number;
  healthCheckPath?: string;
  configuration?: string;
}

export interface UpdateLoadBalancerRequest {
  name: string;
  type: string;
  algorithm: string;
  isActive: boolean;
  healthCheckInterval: number;
  healthCheckTimeout: number;
  unhealthyThreshold: number;
  healthyThreshold: number;
  healthCheckPath?: string;
  configuration?: string;
}

export interface CreateLoadBalancerInstanceRequest {
  instanceId: string;
  ipAddress: string;
  port?: number;
  weight?: number;
  isEnabled?: boolean;
}

export interface UpdateLoadBalancerInstanceRequest {
  status: string;
  weight: number;
  isEnabled: boolean;
}

export interface CreateLoadBalancerRuleRequest {
  name: string;
  type: string;
  condition: string;
  action: string;
  priority?: number;
  isEnabled?: boolean;
}

export interface CreateScalingPolicyRequest {
  name: string;
  type: string;
  metricName: string;
  threshold: number;
  cooldownPeriod?: number;
  scaleUpAdjustment?: number;
  scaleDownAdjustment?: number;
  minInstances?: number;
  maxInstances?: number;
  isEnabled?: boolean;
  schedule?: string;
  conditions?: string;
}

export interface UpdateScalingPolicyRequest {
  name: string;
  type: string;
  metricName: string;
  threshold: number;
  cooldownPeriod: number;
  scaleUpAdjustment: number;
  scaleDownAdjustment: number;
  minInstances: number;
  maxInstances: number;
  isEnabled: boolean;
  schedule?: string;
  conditions?: string;
}

export interface CreateAutoScalingGroupRequest {
  name: string;
  status: string;
  desiredCapacity?: number;
  minSize?: number;
  maxSize?: number;
  launchTemplate?: string;
  vpcConfig?: string;
  tags?: string;
}

export interface UpdateAutoScalingGroupRequest {
  name: string;
  status: string;
  desiredCapacity: number;
  minSize: number;
  maxSize: number;
  launchTemplate?: string;
  vpcConfig?: string;
  tags?: string;
}

export interface CreateLoadBalancerMetricsRequest {
  requestCount: number;
  targetResponseTime: number;
  healthyHostCount: number;
  unhealthyHostCount: number;
  targetConnectionErrorCount?: number;
  targetTLSNegotiationErrorCount?: number;
  requestCountPerTarget?: number;
  unHealthyHostCount?: number;
}

export interface CreateScalingMetricsRequest {
  cpuUtilization: number;
  memoryUtilization: number;
  networkIn?: number;
  networkOut?: number;
  diskReadOps?: number;
  diskWriteOps?: number;
  instanceCount: number;
}

export interface HealthCheckRequest {
  loadBalancerId: number;
  instanceId?: number;
}

export interface ScalingActionRequest {
  autoScalingGroupId: number;
  action: string;
  adjustment?: number;
  desiredCapacity?: number;
  reason?: string;
}

class ScalingService {
  // Load Balancer operations
  async getLoadBalancers(): Promise<LoadBalancer[]> {
    const response = await api.get('/scaling/load-balancers');
    return response.data;
  }

  async getLoadBalancer(id: number): Promise<LoadBalancer> {
    const response = await api.get(`/scaling/load-balancers/${id}`);
    return response.data;
  }

  async createLoadBalancer(
    request: CreateLoadBalancerRequest
  ): Promise<LoadBalancer> {
    const response = await api.post('/scaling/load-balancers', request);
    return response.data;
  }

  async updateLoadBalancer(
    id: number,
    request: UpdateLoadBalancerRequest
  ): Promise<LoadBalancer> {
    const response = await api.put(`/scaling/load-balancers/${id}`, request);
    return response.data;
  }

  async deleteLoadBalancer(id: number): Promise<void> {
    await api.delete(`/scaling/load-balancers/${id}`);
  }

  // Load Balancer Instance operations
  async getLoadBalancerInstances(
    loadBalancerId: number
  ): Promise<LoadBalancerInstance[]> {
    const response = await api.get(
      `/scaling/load-balancers/${loadBalancerId}/instances`
    );
    return response.data;
  }

  async getLoadBalancerInstance(id: number): Promise<LoadBalancerInstance> {
    const response = await api.get(`/scaling/load-balancer-instances/${id}`);
    return response.data;
  }

  async createLoadBalancerInstance(
    loadBalancerId: number,
    request: CreateLoadBalancerInstanceRequest
  ): Promise<LoadBalancerInstance> {
    const response = await api.post(
      `/scaling/load-balancers/${loadBalancerId}/instances`,
      request
    );
    return response.data;
  }

  async updateLoadBalancerInstance(
    id: number,
    request: UpdateLoadBalancerInstanceRequest
  ): Promise<LoadBalancerInstance> {
    const response = await api.put(
      `/scaling/load-balancer-instances/${id}`,
      request
    );
    return response.data;
  }

  async deleteLoadBalancerInstance(id: number): Promise<void> {
    await api.delete(`/scaling/load-balancer-instances/${id}`);
  }

  // Load Balancer Rule operations
  async getLoadBalancerRules(
    loadBalancerId: number
  ): Promise<LoadBalancerRule[]> {
    const response = await api.get(
      `/scaling/load-balancers/${loadBalancerId}/rules`
    );
    return response.data;
  }

  async getLoadBalancerRule(id: number): Promise<LoadBalancerRule> {
    const response = await api.get(`/scaling/load-balancer-rules/${id}`);
    return response.data;
  }

  async createLoadBalancerRule(
    loadBalancerId: number,
    request: CreateLoadBalancerRuleRequest
  ): Promise<LoadBalancerRule> {
    const response = await api.post(
      `/scaling/load-balancers/${loadBalancerId}/rules`,
      request
    );
    return response.data;
  }

  async deleteLoadBalancerRule(id: number): Promise<void> {
    await api.delete(`/scaling/load-balancer-rules/${id}`);
  }

  // Scaling Policy operations
  async getScalingPolicies(): Promise<ScalingPolicy[]> {
    const response = await api.get('/scaling/scaling-policies');
    return response.data;
  }

  async getScalingPolicy(id: number): Promise<ScalingPolicy> {
    const response = await api.get(`/scaling/scaling-policies/${id}`);
    return response.data;
  }

  async createScalingPolicy(
    request: CreateScalingPolicyRequest
  ): Promise<ScalingPolicy> {
    const response = await api.post('/scaling/scaling-policies', request);
    return response.data;
  }

  async updateScalingPolicy(
    id: number,
    request: UpdateScalingPolicyRequest
  ): Promise<ScalingPolicy> {
    const response = await api.put(`/scaling/scaling-policies/${id}`, request);
    return response.data;
  }

  async deleteScalingPolicy(id: number): Promise<void> {
    await api.delete(`/scaling/scaling-policies/${id}`);
  }

  // Scaling Event operations
  async getScalingEvents(
    startDate?: string,
    endDate?: string
  ): Promise<ScalingEvent[]> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    const response = await api.get(
      `/scaling/scaling-events?${params.toString()}`
    );
    return response.data;
  }

  async getScalingEvent(id: number): Promise<ScalingEvent> {
    const response = await api.get(`/scaling/scaling-events/${id}`);
    return response.data;
  }

  // Auto Scaling Group operations
  async getAutoScalingGroups(): Promise<AutoScalingGroup[]> {
    const response = await api.get('/scaling/auto-scaling-groups');
    return response.data;
  }

  async getAutoScalingGroup(id: number): Promise<AutoScalingGroup> {
    const response = await api.get(`/scaling/auto-scaling-groups/${id}`);
    return response.data;
  }

  async createAutoScalingGroup(
    request: CreateAutoScalingGroupRequest
  ): Promise<AutoScalingGroup> {
    const response = await api.post('/scaling/auto-scaling-groups', request);
    return response.data;
  }

  async updateAutoScalingGroup(
    id: number,
    request: UpdateAutoScalingGroupRequest
  ): Promise<AutoScalingGroup> {
    const response = await api.put(
      `/scaling/auto-scaling-groups/${id}`,
      request
    );
    return response.data;
  }

  async deleteAutoScalingGroup(id: number): Promise<void> {
    await api.delete(`/scaling/auto-scaling-groups/${id}`);
  }

  // Auto Scaling Instance operations
  async getAutoScalingInstances(
    autoScalingGroupId: number
  ): Promise<AutoScalingInstance[]> {
    const response = await api.get(
      `/scaling/auto-scaling-groups/${autoScalingGroupId}/instances`
    );
    return response.data;
  }

  async getAutoScalingInstance(id: number): Promise<AutoScalingInstance> {
    const response = await api.get(`/scaling/auto-scaling-instances/${id}`);
    return response.data;
  }

  // Metrics operations
  async getLoadBalancerMetrics(
    loadBalancerId: number,
    startDate?: string,
    endDate?: string
  ): Promise<LoadBalancerMetrics[]> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    const response = await api.get(
      `/scaling/load-balancers/${loadBalancerId}/metrics?${params.toString()}`
    );
    return response.data;
  }

  async createLoadBalancerMetrics(
    loadBalancerId: number,
    request: CreateLoadBalancerMetricsRequest
  ): Promise<LoadBalancerMetrics> {
    const response = await api.post(
      `/scaling/load-balancers/${loadBalancerId}/metrics`,
      request
    );
    return response.data;
  }

  async getScalingMetrics(
    autoScalingGroupId: number,
    startDate?: string,
    endDate?: string
  ): Promise<ScalingMetrics[]> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    const response = await api.get(
      `/scaling/auto-scaling-groups/${autoScalingGroupId}/metrics?${params.toString()}`
    );
    return response.data;
  }

  async createScalingMetrics(
    autoScalingGroupId: number,
    request: CreateScalingMetricsRequest
  ): Promise<ScalingMetrics> {
    const response = await api.post(
      `/scaling/auto-scaling-groups/${autoScalingGroupId}/metrics`,
      request
    );
    return response.data;
  }

  // Summary and utility operations
  async getScalingSummary(): Promise<ScalingSummary> {
    const response = await api.get('/scaling/summary');
    return response.data;
  }

  async performHealthCheck(
    request: HealthCheckRequest
  ): Promise<HealthCheckResponse> {
    const response = await api.post('/scaling/health-check', request);
    return response.data;
  }

  async executeScalingAction(
    request: ScalingActionRequest
  ): Promise<ScalingActionResponse> {
    const response = await api.post('/scaling/scaling-action', request);
    return response.data;
  }

  async getScalingChartData(
    chartType: string,
    startDate: string,
    endDate: string
  ): Promise<ScalingChartData> {
    const params = new URLSearchParams();
    params.append('startDate', startDate);
    params.append('endDate', endDate);

    const response = await api.get(
      `/scaling/charts/${chartType}?${params.toString()}`
    );
    return response.data;
  }

  // Utility methods
  formatStatus(status: string): string {
    return (
      status.charAt(0).toUpperCase() +
      status.slice(1).replace(/([A-Z])/g, ' $1')
    );
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'healthy':
      case 'active':
      case 'inservice':
        return 'text-green-600 bg-green-100';
      case 'unhealthy':
      case 'suspended':
      case 'terminating':
        return 'text-red-600 bg-red-100';
      case 'pending':
        return 'text-yellow-600 bg-yellow-100';
      default:
        return 'text-gray-600 bg-gray-100';
    }
  }

  formatMetricValue(value: number, unit: string = ''): string {
    if (value >= 1000000) {
      return `${(value / 1000000).toFixed(1)}M${unit}`;
    } else if (value >= 1000) {
      return `${(value / 1000).toFixed(1)}K${unit}`;
    }
    return `${value.toFixed(1)}${unit}`;
  }

  generateChartColors(): string[] {
    return [
      'rgba(54, 162, 235, 0.8)',
      'rgba(255, 99, 132, 0.8)',
      'rgba(75, 192, 192, 0.8)',
      'rgba(255, 205, 86, 0.8)',
      'rgba(153, 102, 255, 0.8)',
      'rgba(255, 159, 64, 0.8)',
      'rgba(199, 199, 199, 0.8)',
      'rgba(83, 102, 255, 0.8)',
    ];
  }
}

export const scalingService = new ScalingService();
