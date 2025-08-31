import { apiClient } from '../utils/api';

// Types for Performance & Caching
export interface CacheEntry {
  id: number;
  key: string;
  value: string;
  expiresAt: string;
  createdAt: string;
  lastAccessed: string;
  accessCount: number;
  organizationId: number;
  cacheType?: string;
}

export interface CreateCacheEntryRequest {
  key: string;
  value: string;
  expiresAt: string;
  cacheType?: string;
}

export interface UpdateCacheEntryRequest {
  value: string;
  expiresAt: string;
}

export interface CacheManagementRequest {
  action: string; // clear, clear_expired, clear_by_type
  cacheType?: string;
  pattern?: string;
}

export interface CacheManagementResponse {
  action: string;
  entriesAffected: number;
  executedAt: string;
  isSuccessful: boolean;
  message?: string;
}

// Performance Metrics
export interface PerformanceMetric {
  id: number;
  metricName: string;
  category: string;
  value: number;
  unit: string;
  recordedAt: string;
  organizationId: number;
  context?: string;
  tags?: string;
}

export interface CreatePerformanceMetricRequest {
  metricName: string;
  category: string;
  value: number;
  unit: string;
  context?: string;
  tags?: string;
}

// Database Optimization
export interface DatabaseOptimization {
  id: number;
  tableName: string;
  optimizationType: string;
  query: string;
  executionTime: number;
  improvementPercentage: number;
  executedAt: string;
  isSuccessful: boolean;
  notes?: string;
  organizationId: number;
}

export interface CreateDatabaseOptimizationRequest {
  tableName: string;
  optimizationType: string;
  query: string;
  notes?: string;
}

// API Performance Logs
export interface ApiPerformanceLog {
  id: number;
  httpMethod: string;
  endpoint: string;
  statusCode: number;
  responseTime: number;
  requestedAt: string;
  userId?: number;
  userName?: string;
  organizationId: number;
  userAgent?: string;
  ipAddress?: string;
  requestBody?: string;
  responseBody?: string;
  requestSize: number;
  responseSize: number;
}

// Memory Usage
export interface MemoryUsage {
  id: number;
  totalMemory: number;
  usedMemory: number;
  availableMemory: number;
  memoryUsagePercentage: number;
  recordedAt: string;
  organizationId: number;
  serverInstance?: string;
}

export interface CreateMemoryUsageRequest {
  totalMemory: number;
  usedMemory: number;
  availableMemory: number;
  serverInstance?: string;
}

// CPU Usage
export interface CpuUsage {
  id: number;
  cpuUsagePercentage: number;
  loadAverage: number;
  activeThreads: number;
  recordedAt: string;
  organizationId: number;
  serverInstance?: string;
}

export interface CreateCpuUsageRequest {
  cpuUsagePercentage: number;
  loadAverage: number;
  activeThreads: number;
  serverInstance?: string;
}

// Performance Summary
export interface PerformanceSummary {
  cacheSummary: CacheSummary;
  metricsSummary: MetricsSummary;
  databaseSummary: DatabaseSummary;
  apiSummary: ApiSummary;
  systemSummary: SystemSummary;
}

export interface CacheSummary {
  totalEntries: number;
  expiredEntries: number;
  hitRate: number;
  averageAccessCount: number;
  cacheTypes: string[];
}

export interface MetricsSummary {
  totalMetrics: number;
  metricsToday: number;
  categories: string[];
  averageValue: number;
}

export interface DatabaseSummary {
  totalOptimizations: number;
  successfulOptimizations: number;
  averageImprovement: number;
  averageExecutionTime: number;
  tableNames: string[];
}

export interface ApiSummary {
  totalRequests: number;
  requestsToday: number;
  averageResponseTime: number;
  successRate: number;
  topEndpoints: string[];
}

export interface SystemSummary {
  averageMemoryUsage: number;
  averageCpuUsage: number;
  averageLoadAverage: number;
  totalServerInstances: number;
  serverInstances: string[];
}

// Performance Charts
export interface PerformanceChartData {
  chartType: string;
  labels: string[];
  datasets: PerformanceChartDataset[];
  options: PerformanceChartOptions;
}

export interface PerformanceChartDataset {
  label: string;
  data: number[];
  backgroundColor: string;
  borderColor: string;
  borderWidth: number;
  fill: boolean;
}

export interface PerformanceChartOptions {
  responsive: boolean;
  maintainAspectRatio: boolean;
  title?: string;
  xAxisLabel?: string;
  yAxisLabel?: string;
  showLegend: boolean;
}

// Performance Alerts
export interface PerformanceAlert {
  id: number;
  alertType: string;
  metricName: string;
  threshold: number;
  currentValue: number;
  severity: string;
  triggeredAt: string;
  isResolved: boolean;
  resolvedAt?: string;
  message?: string;
  organizationId: number;
}

export interface CreatePerformanceAlertRequest {
  alertType: string;
  metricName: string;
  threshold: number;
  severity: string;
  message?: string;
}

class PerformanceService {
  // Cache operations
  async getCacheEntries(): Promise<CacheEntry[]> {
    return apiClient.get('/performance/cache');
  }

  async getCacheEntry(key: string): Promise<CacheEntry> {
    return apiClient.get(`/performance/cache/${key}`);
  }

  async createCacheEntry(
    request: CreateCacheEntryRequest
  ): Promise<CacheEntry> {
    return apiClient.post('/performance/cache', request);
  }

  async updateCacheEntry(
    key: string,
    request: UpdateCacheEntryRequest
  ): Promise<CacheEntry> {
    return apiClient.put(`/performance/cache/${key}`, request);
  }

  async deleteCacheEntry(key: string): Promise<void> {
    return apiClient.delete(`/performance/cache/${key}`);
  }

  async manageCache(
    request: CacheManagementRequest
  ): Promise<CacheManagementResponse> {
    return apiClient.post('/performance/cache/manage', request);
  }

  // Performance metrics
  async getPerformanceMetrics(category?: string): Promise<PerformanceMetric[]> {
    const params = new URLSearchParams();
    if (category) params.append('category', category);

    return apiClient.get(`/performance/metrics?${params.toString()}`);
  }

  async getPerformanceMetric(id: number): Promise<PerformanceMetric> {
    return apiClient.get(`/performance/metrics/${id}`);
  }

  async createPerformanceMetric(
    request: CreatePerformanceMetricRequest
  ): Promise<PerformanceMetric> {
    return apiClient.post('/performance/metrics', request);
  }

  async deletePerformanceMetric(id: number): Promise<void> {
    return apiClient.delete(`/performance/metrics/${id}`);
  }

  // Database optimization
  async getDatabaseOptimizations(): Promise<DatabaseOptimization[]> {
    return apiClient.get('/performance/database-optimizations');
  }

  async getDatabaseOptimization(id: number): Promise<DatabaseOptimization> {
    return apiClient.get(`/performance/database-optimizations/${id}`);
  }

  async createDatabaseOptimization(
    request: CreateDatabaseOptimizationRequest
  ): Promise<DatabaseOptimization> {
    return apiClient.post('/performance/database-optimizations', request);
  }

  async deleteDatabaseOptimization(id: number): Promise<void> {
    return apiClient.delete(`/performance/database-optimizations/${id}`);
  }

  async executeOptimization(id: number): Promise<DatabaseOptimization> {
    return apiClient.post(`/performance/database-optimizations/${id}/execute`);
  }

  // API performance logs
  async getApiPerformanceLogs(
    startDate?: string,
    endDate?: string
  ): Promise<ApiPerformanceLog[]> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    return apiClient.get(`/performance/api-logs?${params.toString()}`);
  }

  async getApiPerformanceLog(id: number): Promise<ApiPerformanceLog> {
    return apiClient.get(`/performance/api-logs/${id}`);
  }

  // Memory usage
  async getMemoryUsage(
    startDate?: string,
    endDate?: string
  ): Promise<MemoryUsage[]> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    return apiClient.get(`/performance/memory-usage?${params.toString()}`);
  }

  async getMemoryUsageById(id: number): Promise<MemoryUsage> {
    return apiClient.get(`/performance/memory-usage/${id}`);
  }

  async createMemoryUsage(
    request: CreateMemoryUsageRequest
  ): Promise<MemoryUsage> {
    return apiClient.post('/performance/memory-usage', request);
  }

  async deleteMemoryUsage(id: number): Promise<void> {
    return apiClient.delete(`/performance/memory-usage/${id}`);
  }

  // CPU usage
  async getCpuUsage(startDate?: string, endDate?: string): Promise<CpuUsage[]> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    return apiClient.get(`/performance/cpu-usage?${params.toString()}`);
  }

  async getCpuUsageById(id: number): Promise<CpuUsage> {
    return apiClient.get(`/performance/cpu-usage/${id}`);
  }

  async createCpuUsage(request: CreateCpuUsageRequest): Promise<CpuUsage> {
    return apiClient.post('/performance/cpu-usage', request);
  }

  async deleteCpuUsage(id: number): Promise<void> {
    return apiClient.delete(`/performance/cpu-usage/${id}`);
  }

  // Summary and charts
  async getPerformanceSummary(): Promise<PerformanceSummary> {
    return apiClient.get('/performance/summary');
  }

  async getPerformanceChartData(
    chartType: string,
    startDate: string,
    endDate: string
  ): Promise<PerformanceChartData> {
    return apiClient.get(
      `/performance/charts/${chartType}?startDate=${startDate}&endDate=${endDate}`
    );
  }

  // Performance alerts
  async getPerformanceAlerts(): Promise<PerformanceAlert[]> {
    return apiClient.get('/performance/alerts');
  }

  async getPerformanceAlert(id: number): Promise<PerformanceAlert> {
    return apiClient.get(`/performance/alerts/${id}`);
  }

  async createPerformanceAlert(
    request: CreatePerformanceAlertRequest
  ): Promise<PerformanceAlert> {
    return apiClient.post('/performance/alerts', request);
  }

  async resolveAlert(id: number): Promise<PerformanceAlert> {
    return apiClient.post(`/performance/alerts/${id}/resolve`);
  }

  // Utility methods
  formatPercentage(value: number): string {
    return `${value.toFixed(2)}%`;
  }

  formatBytes(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  formatMilliseconds(ms: number): string {
    if (ms < 1000) return `${ms.toFixed(2)}ms`;
    return `${(ms / 1000).toFixed(2)}s`;
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString();
  }

  formatDateTime(date: string): string {
    return new Date(date).toLocaleString();
  }

  getSeverityColor(severity: string): string {
    switch (severity.toLowerCase()) {
      case 'critical':
        return 'text-red-600';
      case 'high':
        return 'text-orange-600';
      case 'medium':
        return 'text-yellow-600';
      case 'low':
        return 'text-blue-600';
      default:
        return 'text-gray-600';
    }
  }

  getSeverityBgColor(severity: string): string {
    switch (severity.toLowerCase()) {
      case 'critical':
        return 'bg-red-100 text-red-800';
      case 'high':
        return 'bg-orange-100 text-orange-800';
      case 'medium':
        return 'bg-yellow-100 text-yellow-800';
      case 'low':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  getChartColors(index: number): {
    backgroundColor: string;
    borderColor: string;
  } {
    const colors = [
      {
        backgroundColor: 'rgba(54, 162, 235, 0.2)',
        borderColor: 'rgba(54, 162, 235, 1)',
      },
      {
        backgroundColor: 'rgba(255, 99, 132, 0.2)',
        borderColor: 'rgba(255, 99, 132, 1)',
      },
      {
        backgroundColor: 'rgba(75, 192, 192, 0.2)',
        borderColor: 'rgba(75, 192, 192, 1)',
      },
      {
        backgroundColor: 'rgba(255, 159, 64, 0.2)',
        borderColor: 'rgba(255, 159, 64, 1)',
      },
      {
        backgroundColor: 'rgba(153, 102, 255, 0.2)',
        borderColor: 'rgba(153, 102, 255, 1)',
      },
      {
        backgroundColor: 'rgba(255, 205, 86, 0.2)',
        borderColor: 'rgba(255, 205, 86, 1)',
      },
      {
        backgroundColor: 'rgba(201, 203, 207, 0.2)',
        borderColor: 'rgba(201, 203, 207, 1)',
      },
    ];
    return colors[index % colors.length];
  }
}

export const performanceService = new PerformanceService();
