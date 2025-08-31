import { apiClient } from '../utils/api';

// Types for Analytics
export interface Dashboard {
  id: number;
  name: string;
  description?: string;
  isDefault: boolean;
  layout: string;
  organizationId: number;
  createdBy: string;
  createdAt: string;
  updatedAt?: string;
  widgets: DashboardWidget[];
}

export interface DashboardWidget {
  id: number;
  name: string;
  type: string;
  configuration: string;
  position: number;
  createdAt: string;
}

export interface CreateDashboardRequest {
  name: string;
  description?: string;
  isDefault: boolean;
  layout: string;
}

export interface UpdateDashboardRequest {
  name: string;
  description?: string;
  isDefault: boolean;
  layout: string;
}

export interface CreateWidgetRequest {
  name: string;
  type: string;
  configuration: string;
  position: number;
}

// Analytics Metrics
export interface AnalyticsMetric {
  id: number;
  name: string;
  category: string;
  query: string;
  parameters: string;
  lastCalculated: string;
  organizationId: number;
  createdAt: string;
  values: MetricValue[];
}

export interface MetricValue {
  id: number;
  value: number;
  calculatedAt: string;
  context?: string;
}

export interface CreateMetricRequest {
  name: string;
  category: string;
  query: string;
  parameters: string;
}

// Burndown Charts
export interface BurndownChart {
  id: number;
  name: string;
  projectId: number;
  projectName: string;
  startDate: string;
  endDate: string;
  sprintData: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateBurndownChartRequest {
  name: string;
  projectId: number;
  startDate: string;
  endDate: string;
  sprintData: string;
}

// Agile Metrics
export interface AgileMetrics {
  id: number;
  projectId: number;
  projectName: string;
  sprintId: string;
  totalStoryPoints: number;
  completedStoryPoints: number;
  remainingStoryPoints: number;
  velocity: number;
  burndownRate: number;
  sprintStartDate: string;
  sprintEndDate: string;
  calculatedAt: string;
}

export interface CreateAgileMetricsRequest {
  projectId: number;
  sprintId: string;
  totalStoryPoints: number;
  completedStoryPoints: number;
  sprintStartDate: string;
  sprintEndDate: string;
}

// Resource Utilization
export interface ResourceUtilization {
  id: number;
  userId: number;
  userName: string;
  organizationId: number;
  date: string;
  hoursWorked: number;
  hoursAllocated: number;
  utilizationRate: number;
  notes?: string;
  createdAt: string;
}

export interface CreateResourceUtilizationRequest {
  userId: number;
  date: string;
  hoursWorked: number;
  hoursAllocated: number;
  notes?: string;
}

// ROI Tracking
export interface ROITracking {
  id: number;
  projectName: string;
  projectId: number;
  investment: number;
  return: number;
  roi: number;
  laborCost: number;
  infrastructureCost: number;
  otherCosts: number;
  startDate: string;
  endDate?: string;
  notes?: string;
  organizationId: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateROITrackingRequest {
  projectName: string;
  projectId: number;
  investment: number;
  return: number;
  laborCost: number;
  infrastructureCost: number;
  otherCosts: number;
  startDate: string;
  endDate?: string;
  notes?: string;
}

// Predictive Analytics
export interface PredictiveAnalytics {
  id: number;
  modelName: string;
  type: string;
  trainingData: string;
  modelParameters: string;
  accuracy: number;
  lastTrained: string;
  organizationId: number;
  createdAt: string;
  predictions: Prediction[];
}

export interface Prediction {
  id: number;
  inputData: string;
  predictionResult: string;
  confidence: number;
  predictedAt: string;
}

export interface CreatePredictiveAnalyticsRequest {
  modelName: string;
  type: string;
  trainingData: string;
  modelParameters: string;
}

export interface MakePredictionRequest {
  modelId: number;
  inputData: string;
}

// Chart Data
export interface ChartData {
  chartType: string;
  labels: string[];
  datasets: ChartDataset[];
  options: ChartOptions;
}

export interface ChartDataset {
  label: string;
  data: number[];
  backgroundColor: string;
  borderColor: string;
  borderWidth: number;
}

export interface ChartOptions {
  responsive: boolean;
  maintainAspectRatio: boolean;
  title?: string;
  xAxisLabel?: string;
  yAxisLabel?: string;
}

// Analytics Summary
export interface AnalyticsSummary {
  dashboardSummary: DashboardSummary;
  metricsSummary: MetricsSummary;
  agileSummary: AgileSummary;
  resourceSummary: ResourceSummary;
  roiSummary: ROISummary;
  predictiveSummary: PredictiveSummary;
}

export interface DashboardSummary {
  totalDashboards: number;
  defaultDashboards: number;
  totalWidgets: number;
  widgetTypes: string[];
}

export interface MetricsSummary {
  totalMetrics: number;
  metricsCalculatedToday: number;
  categories: string[];
  averageAccuracy: number;
}

export interface AgileSummary {
  totalSprints: number;
  activeSprints: number;
  averageVelocity: number;
  averageBurndownRate: number;
  totalStoryPoints: number;
  completedStoryPoints: number;
}

export interface ResourceSummary {
  totalUsers: number;
  averageUtilizationRate: number;
  totalHoursWorked: number;
  totalHoursAllocated: number;
  topUtilizedUsers: string[];
}

export interface ROISummary {
  totalProjects: number;
  totalInvestment: number;
  totalReturn: number;
  averageROI: number;
  totalLaborCost: number;
  totalInfrastructureCost: number;
}

export interface PredictiveSummary {
  totalModels: number;
  activeModels: number;
  averageAccuracy: number;
  totalPredictions: number;
  modelTypes: string[];
}

// Report
export interface ReportRequest {
  reportType: string;
  startDate?: string;
  endDate?: string;
  projectId?: number;
  userId?: number;
  category?: string;
  format?: string;
}

export interface ReportResponse {
  reportType: string;
  generatedAt: string;
  format: string;
  data: any;
  downloadUrl?: string;
}

class AnalyticsService {
  // Dashboard operations
  async getDashboards(): Promise<Dashboard[]> {
    return apiClient.get('/analytics/dashboards');
  }

  async getDashboard(id: number): Promise<Dashboard> {
    return apiClient.get(`/analytics/dashboards/${id}`);
  }

  async createDashboard(request: CreateDashboardRequest): Promise<Dashboard> {
    return apiClient.post('/analytics/dashboards', request);
  }

  async updateDashboard(
    id: number,
    request: UpdateDashboardRequest
  ): Promise<Dashboard> {
    return apiClient.put(`/analytics/dashboards/${id}`, request);
  }

  async deleteDashboard(id: number): Promise<void> {
    return apiClient.delete(`/analytics/dashboards/${id}`);
  }

  async addWidget(
    dashboardId: number,
    request: CreateWidgetRequest
  ): Promise<DashboardWidget> {
    return apiClient.post(
      `/analytics/dashboards/${dashboardId}/widgets`,
      request
    );
  }

  async deleteWidget(widgetId: number): Promise<void> {
    return apiClient.delete(`/analytics/widgets/${widgetId}`);
  }

  // Metrics operations
  async getMetrics(): Promise<AnalyticsMetric[]> {
    return apiClient.get('/analytics/metrics');
  }

  async getMetric(id: number): Promise<AnalyticsMetric> {
    return apiClient.get(`/analytics/metrics/${id}`);
  }

  async createMetric(request: CreateMetricRequest): Promise<AnalyticsMetric> {
    return apiClient.post('/analytics/metrics', request);
  }

  async deleteMetric(id: number): Promise<void> {
    return apiClient.delete(`/analytics/metrics/${id}`);
  }

  async calculateMetric(id: number): Promise<number> {
    return apiClient.post(`/analytics/metrics/${id}/calculate`);
  }

  // Burndown charts
  async getBurndownCharts(projectId: number): Promise<BurndownChart[]> {
    return apiClient.get(`/analytics/projects/${projectId}/burndown-charts`);
  }

  async getBurndownChart(id: number): Promise<BurndownChart> {
    return apiClient.get(`/analytics/burndown-charts/${id}`);
  }

  async createBurndownChart(
    request: CreateBurndownChartRequest
  ): Promise<BurndownChart> {
    return apiClient.post('/analytics/burndown-charts', request);
  }

  async deleteBurndownChart(id: number): Promise<void> {
    return apiClient.delete(`/analytics/burndown-charts/${id}`);
  }

  async getBurndownChartData(id: number): Promise<ChartData> {
    return apiClient.get(`/analytics/burndown-charts/${id}/data`);
  }

  // Agile metrics
  async getAgileMetrics(projectId: number): Promise<AgileMetrics[]> {
    return apiClient.get(`/analytics/projects/${projectId}/agile-metrics`);
  }

  async getAgileMetricsById(id: number): Promise<AgileMetrics> {
    return apiClient.get(`/analytics/agile-metrics/${id}`);
  }

  async createAgileMetrics(
    request: CreateAgileMetricsRequest
  ): Promise<AgileMetrics> {
    return apiClient.post('/analytics/agile-metrics', request);
  }

  async deleteAgileMetrics(id: number): Promise<void> {
    return apiClient.delete(`/analytics/agile-metrics/${id}`);
  }

  async getVelocityChartData(projectId: number): Promise<ChartData> {
    return apiClient.get(`/analytics/projects/${projectId}/velocity-chart`);
  }

  // Resource utilization
  async getResourceUtilization(
    startDate?: string,
    endDate?: string
  ): Promise<ResourceUtilization[]> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    return apiClient.get(
      `/analytics/resource-utilization?${params.toString()}`
    );
  }

  async getResourceUtilizationById(id: number): Promise<ResourceUtilization> {
    return apiClient.get(`/analytics/resource-utilization/${id}`);
  }

  async createResourceUtilization(
    request: CreateResourceUtilizationRequest
  ): Promise<ResourceUtilization> {
    return apiClient.post('/analytics/resource-utilization', request);
  }

  async deleteResourceUtilization(id: number): Promise<void> {
    return apiClient.delete(`/analytics/resource-utilization/${id}`);
  }

  async getUtilizationChartData(
    startDate: string,
    endDate: string
  ): Promise<ChartData> {
    return apiClient.get(
      `/analytics/resource-utilization/chart?startDate=${startDate}&endDate=${endDate}`
    );
  }

  // ROI tracking
  async getROITracking(): Promise<ROITracking[]> {
    return apiClient.get('/analytics/roi-tracking');
  }

  async getROITrackingById(id: number): Promise<ROITracking> {
    return apiClient.get(`/analytics/roi-tracking/${id}`);
  }

  async createROITracking(
    request: CreateROITrackingRequest
  ): Promise<ROITracking> {
    return apiClient.post('/analytics/roi-tracking', request);
  }

  async deleteROITracking(id: number): Promise<void> {
    return apiClient.delete(`/analytics/roi-tracking/${id}`);
  }

  async getROIChartData(): Promise<ChartData> {
    return apiClient.get('/analytics/roi-tracking/chart');
  }

  // Predictive analytics
  async getPredictiveAnalytics(): Promise<PredictiveAnalytics[]> {
    return apiClient.get('/analytics/predictive-analytics');
  }

  async getPredictiveAnalyticsById(id: number): Promise<PredictiveAnalytics> {
    return apiClient.get(`/analytics/predictive-analytics/${id}`);
  }

  async createPredictiveAnalytics(
    request: CreatePredictiveAnalyticsRequest
  ): Promise<PredictiveAnalytics> {
    return apiClient.post('/analytics/predictive-analytics', request);
  }

  async deletePredictiveAnalytics(id: number): Promise<void> {
    return apiClient.delete(`/analytics/predictive-analytics/${id}`);
  }

  async makePrediction(request: MakePredictionRequest): Promise<Prediction> {
    return apiClient.post('/analytics/predictive-analytics/predict', request);
  }

  // Summary and reports
  async getAnalyticsSummary(): Promise<AnalyticsSummary> {
    return apiClient.get('/analytics/summary');
  }

  async generateReport(request: ReportRequest): Promise<ReportResponse> {
    return apiClient.post('/analytics/reports', request);
  }

  // Utility methods
  formatUtilizationRate(rate: number): string {
    return `${(rate * 100).toFixed(1)}%`;
  }

  formatROI(roi: number): string {
    return `${(roi * 100).toFixed(2)}%`;
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString();
  }

  formatDateTime(date: string): string {
    return new Date(date).toLocaleString();
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

export const analyticsService = new AnalyticsService();
