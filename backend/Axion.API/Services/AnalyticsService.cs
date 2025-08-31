using Axion.API.Data;
using Axion.API.DTOs;
using Axion.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Axion.API.Services
{
  public interface IAnalyticsService
  {
    // Dashboard operations
    Task<List<DashboardResponse>> GetDashboardsAsync(int organizationId);
    Task<DashboardResponse?> GetDashboardAsync(int id);
    Task<DashboardResponse> CreateDashboardAsync(CreateDashboardRequest request, int organizationId, string userId);
    Task<DashboardResponse> UpdateDashboardAsync(int id, UpdateDashboardRequest request);
    Task DeleteDashboardAsync(int id);
    Task<DashboardWidgetResponse> AddWidgetAsync(int dashboardId, CreateWidgetRequest request);
    Task DeleteWidgetAsync(int widgetId);

    // Metrics operations
    Task<List<AnalyticsMetricResponse>> GetMetricsAsync(int organizationId);
    Task<AnalyticsMetricResponse?> GetMetricAsync(int id);
    Task<AnalyticsMetricResponse> CreateMetricAsync(CreateMetricRequest request, int organizationId);
    Task DeleteMetricAsync(int id);
    Task<decimal> CalculateMetricAsync(int metricId);

    // Burndown charts
    Task<List<BurndownChartResponse>> GetBurndownChartsAsync(int projectId);
    Task<BurndownChartResponse?> GetBurndownChartAsync(int id);
    Task<BurndownChartResponse> CreateBurndownChartAsync(CreateBurndownChartRequest request);
    Task DeleteBurndownChartAsync(int id);
    Task<ChartDataResponse> GetBurndownChartDataAsync(int chartId);

    // Agile metrics
    Task<List<AgileMetricsResponse>> GetAgileMetricsAsync(int projectId);
    Task<AgileMetricsResponse?> GetAgileMetricsAsync(int id);
    Task<AgileMetricsResponse> CreateAgileMetricsAsync(CreateAgileMetricsRequest request);
    Task DeleteAgileMetricsAsync(int id);
    Task<ChartDataResponse> GetVelocityChartDataAsync(int projectId);

    // Resource utilization
    Task<List<ResourceUtilizationResponse>> GetResourceUtilizationAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ResourceUtilizationResponse?> GetResourceUtilizationAsync(int id);
    Task<ResourceUtilizationResponse> CreateResourceUtilizationAsync(CreateResourceUtilizationRequest request, int organizationId);
    Task DeleteResourceUtilizationAsync(int id);
    Task<ChartDataResponse> GetUtilizationChartDataAsync(int organizationId, DateTime startDate, DateTime endDate);

    // ROI tracking
    Task<List<ROITrackingResponse>> GetROITrackingAsync(int organizationId);
    Task<ROITrackingResponse?> GetROITrackingAsync(int id);
    Task<ROITrackingResponse> CreateROITrackingAsync(CreateROITrackingRequest request, int organizationId);
    Task DeleteROITrackingAsync(int id);
    Task<ChartDataResponse> GetROIChartDataAsync(int organizationId);

    // Predictive analytics
    Task<List<PredictiveAnalyticsResponse>> GetPredictiveAnalyticsAsync(int organizationId);
    Task<PredictiveAnalyticsResponse?> GetPredictiveAnalyticsAsync(int id);
    Task<PredictiveAnalyticsResponse> CreatePredictiveAnalyticsAsync(CreatePredictiveAnalyticsRequest request, int organizationId);
    Task DeletePredictiveAnalyticsAsync(int id);
    Task<PredictionResponse> MakePredictionAsync(MakePredictionRequest request);

    // Summary and reports
    Task<AnalyticsSummaryResponse> GetAnalyticsSummaryAsync(int organizationId);
    Task<ReportResponse> GenerateReportAsync(ReportRequest request, int organizationId);
  }

  public class AnalyticsService : IAnalyticsService
  {
    private readonly AxionDbContext _context;

    public AnalyticsService(AxionDbContext context)
    {
      _context = context;
    }

    // Dashboard operations
    public async Task<List<DashboardResponse>> GetDashboardsAsync(int organizationId)
    {
      return await _context.Dashboards
          .Where(d => d.OrganizationId == organizationId)
          .Include(d => d.CreatedBy)
          .Include(d => d.Widgets)
          .Select(d => new DashboardResponse
          {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            IsDefault = d.IsDefault,
            Layout = d.Layout,
            OrganizationId = d.OrganizationId,
            CreatedBy = $"{d.CreatedBy.FirstName} {d.CreatedBy.LastName}",
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            Widgets = d.Widgets.Select(w => new DashboardWidgetResponse
            {
              Id = w.Id,
              Name = w.Name,
              Type = w.Type,
              Configuration = w.Configuration,
              Position = w.Position,
              CreatedAt = w.CreatedAt
            }).ToList()
          })
          .ToListAsync();
    }

    public async Task<DashboardResponse?> GetDashboardAsync(int id)
    {
      var dashboard = await _context.Dashboards
          .Include(d => d.CreatedBy)
          .Include(d => d.Widgets)
          .FirstOrDefaultAsync(d => d.Id == id);

      if (dashboard == null) return null;

      return new DashboardResponse
      {
        Id = dashboard.Id,
        Name = dashboard.Name,
        Description = dashboard.Description,
        IsDefault = dashboard.IsDefault,
        Layout = dashboard.Layout,
        OrganizationId = dashboard.OrganizationId,
        CreatedBy = $"{dashboard.CreatedBy.FirstName} {dashboard.CreatedBy.LastName}",
        CreatedAt = dashboard.CreatedAt,
        UpdatedAt = dashboard.UpdatedAt,
        Widgets = dashboard.Widgets.Select(w => new DashboardWidgetResponse
        {
          Id = w.Id,
          Name = w.Name,
          Type = w.Type,
          Configuration = w.Configuration,
          Position = w.Position,
          CreatedAt = w.CreatedAt
        }).ToList()
      };
    }

    public async Task<DashboardResponse> CreateDashboardAsync(CreateDashboardRequest request, int organizationId, string userId)
    {
      var dashboard = new Dashboard
      {
        Name = request.Name,
        Description = request.Description,
        IsDefault = request.IsDefault,
        Layout = request.Layout,
        OrganizationId = organizationId,
        CreatedById = userId,
        CreatedAt = DateTime.UtcNow
      };

      _context.Dashboards.Add(dashboard);
      await _context.SaveChangesAsync();

      return await GetDashboardAsync(dashboard.Id) ?? throw new InvalidOperationException("Failed to create dashboard");
    }

    public async Task<DashboardResponse> UpdateDashboardAsync(int id, UpdateDashboardRequest request)
    {
      var dashboard = await _context.Dashboards.FindAsync(id);
      if (dashboard == null)
        throw new ArgumentException("Dashboard not found");

      dashboard.Name = request.Name;
      dashboard.Description = request.Description;
      dashboard.IsDefault = request.IsDefault;
      dashboard.Layout = request.Layout;
      dashboard.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      return await GetDashboardAsync(id) ?? throw new InvalidOperationException("Failed to update dashboard");
    }

    public async Task DeleteDashboardAsync(int id)
    {
      var dashboard = await _context.Dashboards.FindAsync(id);
      if (dashboard == null)
        throw new ArgumentException("Dashboard not found");

      _context.Dashboards.Remove(dashboard);
      await _context.SaveChangesAsync();
    }

    public async Task<DashboardWidgetResponse> AddWidgetAsync(int dashboardId, CreateWidgetRequest request)
    {
      var dashboard = await _context.Dashboards.FindAsync(dashboardId);
      if (dashboard == null)
        throw new ArgumentException("Dashboard not found");

      var widget = new DashboardWidget
      {
        Name = request.Name,
        Type = request.Type,
        Configuration = request.Configuration,
        Position = request.Position,
        DashboardId = dashboardId,
        CreatedAt = DateTime.UtcNow
      };

      _context.DashboardWidgets.Add(widget);
      await _context.SaveChangesAsync();

      return new DashboardWidgetResponse
      {
        Id = widget.Id,
        Name = widget.Name,
        Type = widget.Type,
        Configuration = widget.Configuration,
        Position = widget.Position,
        CreatedAt = widget.CreatedAt
      };
    }

    public async Task DeleteWidgetAsync(int widgetId)
    {
      var widget = await _context.DashboardWidgets.FindAsync(widgetId);
      if (widget == null)
        throw new ArgumentException("Widget not found");

      _context.DashboardWidgets.Remove(widget);
      await _context.SaveChangesAsync();
    }

    // Metrics operations
    public async Task<List<AnalyticsMetricResponse>> GetMetricsAsync(int organizationId)
    {
      return await _context.AnalyticsMetrics
          .Where(m => m.OrganizationId == organizationId)
          .Include(m => m.Values)
          .Select(m => new AnalyticsMetricResponse
          {
            Id = m.Id,
            Name = m.Name,
            Category = m.Category,
            Query = m.Query,
            Parameters = m.Parameters,
            LastCalculated = m.LastCalculated,
            OrganizationId = m.OrganizationId,
            CreatedAt = m.CreatedAt,
            Values = m.Values.Select(v => new MetricValueResponse
            {
              Id = v.Id,
              Value = v.Value,
              CalculatedAt = v.CalculatedAt,
              Context = v.Context
            }).ToList()
          })
          .ToListAsync();
    }

    public async Task<AnalyticsMetricResponse?> GetMetricAsync(int id)
    {
      var metric = await _context.AnalyticsMetrics
          .Include(m => m.Values)
          .FirstOrDefaultAsync(m => m.Id == id);

      if (metric == null) return null;

      return new AnalyticsMetricResponse
      {
        Id = metric.Id,
        Name = metric.Name,
        Category = metric.Category,
        Query = metric.Query,
        Parameters = metric.Parameters,
        LastCalculated = metric.LastCalculated,
        OrganizationId = metric.OrganizationId,
        CreatedAt = metric.CreatedAt,
        Values = metric.Values.Select(v => new MetricValueResponse
        {
          Id = v.Id,
          Value = v.Value,
          CalculatedAt = v.CalculatedAt,
          Context = v.Context
        }).ToList()
      };
    }

    public async Task<AnalyticsMetricResponse> CreateMetricAsync(CreateMetricRequest request, int organizationId)
    {
      var metric = new AnalyticsMetric
      {
        Name = request.Name,
        Category = request.Category,
        Query = request.Query,
        Parameters = request.Parameters,
        OrganizationId = organizationId,
        LastCalculated = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
      };

      _context.AnalyticsMetrics.Add(metric);
      await _context.SaveChangesAsync();

      return await GetMetricAsync(metric.Id) ?? throw new InvalidOperationException("Failed to create metric");
    }

    public async Task DeleteMetricAsync(int id)
    {
      var metric = await _context.AnalyticsMetrics.FindAsync(id);
      if (metric == null)
        throw new ArgumentException("Metric not found");

      _context.AnalyticsMetrics.Remove(metric);
      await _context.SaveChangesAsync();
    }

    public async Task<decimal> CalculateMetricAsync(int metricId)
    {
      var metric = await _context.AnalyticsMetrics.FindAsync(metricId);
      if (metric == null)
        throw new ArgumentException("Metric not found");

      // Simulate metric calculation
      var random = new Random();
      var value = (decimal)(random.NextDouble() * 100);

      var metricValue = new MetricValue
      {
        Value = value,
        CalculatedAt = DateTime.UtcNow,
        MetricId = metricId
      };

      _context.MetricValues.Add(metricValue);
      metric.LastCalculated = DateTime.UtcNow;
      await _context.SaveChangesAsync();

      return value;
    }

    // Burndown charts
    public async Task<List<BurndownChartResponse>> GetBurndownChartsAsync(int projectId)
    {
      return await _context.BurndownCharts
          .Where(b => b.ProjectId == projectId)
          .Include(b => b.Project)
          .Select(b => new BurndownChartResponse
          {
            Id = b.Id,
            Name = b.Name,
            ProjectId = b.ProjectId,
            ProjectName = b.Project.Name,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            SprintData = b.SprintData,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
          })
          .ToListAsync();
    }

    public async Task<BurndownChartResponse?> GetBurndownChartAsync(int id)
    {
      var chart = await _context.BurndownCharts
          .Include(b => b.Project)
          .FirstOrDefaultAsync(b => b.Id == id);

      if (chart == null) return null;

      return new BurndownChartResponse
      {
        Id = chart.Id,
        Name = chart.Name,
        ProjectId = chart.ProjectId,
        ProjectName = chart.Project.Name,
        StartDate = chart.StartDate,
        EndDate = chart.EndDate,
        SprintData = chart.SprintData,
        CreatedAt = chart.CreatedAt,
        UpdatedAt = chart.UpdatedAt
      };
    }

    public async Task<BurndownChartResponse> CreateBurndownChartAsync(CreateBurndownChartRequest request)
    {
      var chart = new BurndownChart
      {
        Name = request.Name,
        ProjectId = request.ProjectId,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        SprintData = request.SprintData,
        CreatedAt = DateTime.UtcNow
      };

      _context.BurndownCharts.Add(chart);
      await _context.SaveChangesAsync();

      return await GetBurndownChartAsync(chart.Id) ?? throw new InvalidOperationException("Failed to create burndown chart");
    }

    public async Task DeleteBurndownChartAsync(int id)
    {
      var chart = await _context.BurndownCharts.FindAsync(id);
      if (chart == null)
        throw new ArgumentException("Burndown chart not found");

      _context.BurndownCharts.Remove(chart);
      await _context.SaveChangesAsync();
    }

    public async Task<ChartDataResponse> GetBurndownChartDataAsync(int chartId)
    {
      var chart = await _context.BurndownCharts.FindAsync(chartId);
      if (chart == null)
        throw new ArgumentException("Burndown chart not found");

      // Simulate burndown chart data
      var days = (chart.EndDate - chart.StartDate).Days + 1;
      var labels = Enumerable.Range(0, days).Select(i => chart.StartDate.AddDays(i).ToString("MM/dd")).ToList();

      var idealLine = Enumerable.Range(0, days).Select(i => 100 - (100.0m / days * i)).ToList();
      var actualLine = Enumerable.Range(0, days).Select(i =>
      {
        var random = new Random();
        return Math.Max(0, 100 - (100.0m / days * i) + (decimal)(random.NextDouble() * 20 - 10));
      }).ToList();

      return new ChartDataResponse
      {
        ChartType = "line",
        Labels = labels,
        Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Label = "Ideal Burndown",
                        Data = idealLine,
                        BackgroundColor = "rgba(54, 162, 235, 0.2)",
                        BorderColor = "rgba(54, 162, 235, 1)",
                        BorderWidth = 2
                    },
                    new ChartDataset
                    {
                        Label = "Actual Burndown",
                        Data = actualLine,
                        BackgroundColor = "rgba(255, 99, 132, 0.2)",
                        BorderColor = "rgba(255, 99, 132, 1)",
                        BorderWidth = 2
                    }
                },
        Options = new ChartOptions
        {
          Title = $"Burndown Chart - {chart.Name}",
          XAxisLabel = "Days",
          YAxisLabel = "Story Points Remaining"
        }
      };
    }

    // Agile metrics
    public async Task<List<AgileMetricsResponse>> GetAgileMetricsAsync(int projectId)
    {
      return await _context.AgileMetrics
          .Where(a => a.ProjectId == projectId)
          .Include(a => a.Project)
          .Select(a => new AgileMetricsResponse
          {
            Id = a.Id,
            ProjectId = a.ProjectId,
            ProjectName = a.Project.Name,
            SprintId = a.SprintId,
            TotalStoryPoints = a.TotalStoryPoints,
            CompletedStoryPoints = a.CompletedStoryPoints,
            RemainingStoryPoints = a.RemainingStoryPoints,
            Velocity = a.Velocity,
            BurndownRate = a.BurndownRate,
            SprintStartDate = a.SprintStartDate,
            SprintEndDate = a.SprintEndDate,
            CalculatedAt = a.CalculatedAt
          })
          .ToListAsync();
    }

    public async Task<AgileMetricsResponse?> GetAgileMetricsAsync(int id)
    {
      var metrics = await _context.AgileMetrics
          .Include(a => a.Project)
          .FirstOrDefaultAsync(a => a.Id == id);

      if (metrics == null) return null;

      return new AgileMetricsResponse
      {
        Id = metrics.Id,
        ProjectId = metrics.ProjectId,
        ProjectName = metrics.Project.Name,
        SprintId = metrics.SprintId,
        TotalStoryPoints = metrics.TotalStoryPoints,
        CompletedStoryPoints = metrics.CompletedStoryPoints,
        RemainingStoryPoints = metrics.RemainingStoryPoints,
        Velocity = metrics.Velocity,
        BurndownRate = metrics.BurndownRate,
        SprintStartDate = metrics.SprintStartDate,
        SprintEndDate = metrics.SprintEndDate,
        CalculatedAt = metrics.CalculatedAt
      };
    }

    public async Task<AgileMetricsResponse> CreateAgileMetricsAsync(CreateAgileMetricsRequest request)
    {
      var metrics = new AgileMetrics
      {
        ProjectId = request.ProjectId,
        SprintId = request.SprintId,
        TotalStoryPoints = request.TotalStoryPoints,
        CompletedStoryPoints = request.CompletedStoryPoints,
        RemainingStoryPoints = request.TotalStoryPoints - request.CompletedStoryPoints,
        SprintStartDate = request.SprintStartDate,
        SprintEndDate = request.SprintEndDate,
        CalculatedAt = DateTime.UtcNow
      };

      // Calculate velocity and burndown rate
      var sprintDays = (metrics.SprintEndDate - metrics.SprintStartDate).Days + 1;
      metrics.Velocity = metrics.CompletedStoryPoints / sprintDays;
      metrics.BurndownRate = metrics.TotalStoryPoints / sprintDays;

      _context.AgileMetrics.Add(metrics);
      await _context.SaveChangesAsync();

      return await GetAgileMetricsAsync(metrics.Id) ?? throw new InvalidOperationException("Failed to create agile metrics");
    }

    public async Task DeleteAgileMetricsAsync(int id)
    {
      var metrics = await _context.AgileMetrics.FindAsync(id);
      if (metrics == null)
        throw new ArgumentException("Agile metrics not found");

      _context.AgileMetrics.Remove(metrics);
      await _context.SaveChangesAsync();
    }

    public async Task<ChartDataResponse> GetVelocityChartDataAsync(int projectId)
    {
      var metrics = await _context.AgileMetrics
          .Where(a => a.ProjectId == projectId)
          .OrderBy(a => a.SprintStartDate)
          .ToListAsync();

      var labels = metrics.Select(m => m.SprintId).ToList();
      var velocityData = metrics.Select(m => m.Velocity).ToList();
      var burndownData = metrics.Select(m => m.BurndownRate).ToList();

      return new ChartDataResponse
      {
        ChartType = "bar",
        Labels = labels,
        Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Label = "Velocity",
                        Data = velocityData,
                        BackgroundColor = "rgba(75, 192, 192, 0.2)",
                        BorderColor = "rgba(75, 192, 192, 1)",
                        BorderWidth = 1
                    },
                    new ChartDataset
                    {
                        Label = "Burndown Rate",
                        Data = burndownData,
                        BackgroundColor = "rgba(255, 159, 64, 0.2)",
                        BorderColor = "rgba(255, 159, 64, 1)",
                        BorderWidth = 1
                    }
                },
        Options = new ChartOptions
        {
          Title = "Sprint Velocity & Burndown Rate",
          XAxisLabel = "Sprint",
          YAxisLabel = "Story Points per Day"
        }
      };
    }

    // Resource utilization
    public async Task<List<ResourceUtilizationResponse>> GetResourceUtilizationAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null)
    {
      var query = _context.ResourceUtilizations
          .Where(r => r.OrganizationId == organizationId)
          .Include(r => r.User);

      if (startDate.HasValue)
        query = query.Where(r => r.Date >= startDate.Value);

      if (endDate.HasValue)
        query = query.Where(r => r.Date <= endDate.Value);

      return await query
          .Select(r => new ResourceUtilizationResponse
          {
            Id = r.Id,
            UserId = r.UserId,
            UserName = $"{r.User.FirstName} {r.User.LastName}",
            OrganizationId = r.OrganizationId,
            Date = r.Date,
            HoursWorked = r.HoursWorked,
            HoursAllocated = r.HoursAllocated,
            UtilizationRate = r.UtilizationRate,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
          })
          .ToListAsync();
    }

    public async Task<ResourceUtilizationResponse?> GetResourceUtilizationAsync(int id)
    {
      var utilization = await _context.ResourceUtilizations
          .Include(r => r.User)
          .FirstOrDefaultAsync(r => r.Id == id);

      if (utilization == null) return null;

      return new ResourceUtilizationResponse
      {
        Id = utilization.Id,
        UserId = utilization.UserId,
        UserName = $"{utilization.User.FirstName} {utilization.User.LastName}",
        OrganizationId = utilization.OrganizationId,
        Date = utilization.Date,
        HoursWorked = utilization.HoursWorked,
        HoursAllocated = utilization.HoursAllocated,
        UtilizationRate = utilization.UtilizationRate,
        Notes = utilization.Notes,
        CreatedAt = utilization.CreatedAt
      };
    }

    public async Task<ResourceUtilizationResponse> CreateResourceUtilizationAsync(CreateResourceUtilizationRequest request, int organizationId)
    {
      var utilization = new ResourceUtilization
      {
        UserId = request.UserId,
        OrganizationId = organizationId,
        Date = request.Date,
        HoursWorked = request.HoursWorked,
        HoursAllocated = request.HoursAllocated,
        UtilizationRate = request.HoursAllocated > 0 ? request.HoursWorked / request.HoursAllocated : 0,
        Notes = request.Notes,
        CreatedAt = DateTime.UtcNow
      };

      _context.ResourceUtilizations.Add(utilization);
      await _context.SaveChangesAsync();

      return await GetResourceUtilizationAsync(utilization.Id) ?? throw new InvalidOperationException("Failed to create resource utilization");
    }

    public async Task DeleteResourceUtilizationAsync(int id)
    {
      var utilization = await _context.ResourceUtilizations.FindAsync(id);
      if (utilization == null)
        throw new ArgumentException("Resource utilization not found");

      _context.ResourceUtilizations.Remove(utilization);
      await _context.SaveChangesAsync();
    }

    public async Task<ChartDataResponse> GetUtilizationChartDataAsync(int organizationId, DateTime startDate, DateTime endDate)
    {
      var utilizations = await _context.ResourceUtilizations
          .Where(r => r.OrganizationId == organizationId && r.Date >= startDate && r.Date <= endDate)
          .Include(r => r.User)
          .OrderBy(r => r.Date)
          .ToListAsync();

      var labels = utilizations.Select(r => r.Date.ToString("MM/dd")).ToList();
      var utilizationData = utilizations.Select(r => r.UtilizationRate * 100).ToList(); // Convert to percentage

      return new ChartDataResponse
      {
        ChartType = "line",
        Labels = labels,
        Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Label = "Utilization Rate (%)",
                        Data = utilizationData,
                        BackgroundColor = "rgba(153, 102, 255, 0.2)",
                        BorderColor = "rgba(153, 102, 255, 1)",
                        BorderWidth = 2
                    }
                },
        Options = new ChartOptions
        {
          Title = "Resource Utilization Over Time",
          XAxisLabel = "Date",
          YAxisLabel = "Utilization Rate (%)"
        }
      };
    }

    // ROI tracking
    public async Task<List<ROITrackingResponse>> GetROITrackingAsync(int organizationId)
    {
      return await _context.ROITrackings
          .Where(r => r.OrganizationId == organizationId)
          .Include(r => r.Project)
          .Select(r => new ROITrackingResponse
          {
            Id = r.Id,
            ProjectName = r.ProjectName,
            ProjectId = r.ProjectId,
            Investment = r.Investment,
            Return = r.Return,
            ROI = r.ROI,
            LaborCost = r.LaborCost,
            InfrastructureCost = r.InfrastructureCost,
            OtherCosts = r.OtherCosts,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            Notes = r.Notes,
            OrganizationId = r.OrganizationId,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
          })
          .ToListAsync();
    }

    public async Task<ROITrackingResponse?> GetROITrackingAsync(int id)
    {
      var roi = await _context.ROITrackings
          .Include(r => r.Project)
          .FirstOrDefaultAsync(r => r.Id == id);

      if (roi == null) return null;

      return new ROITrackingResponse
      {
        Id = roi.Id,
        ProjectName = roi.ProjectName,
        ProjectId = roi.ProjectId,
        Investment = roi.Investment,
        Return = roi.Return,
        ROI = roi.ROI,
        LaborCost = roi.LaborCost,
        InfrastructureCost = roi.InfrastructureCost,
        OtherCosts = roi.OtherCosts,
        StartDate = roi.StartDate,
        EndDate = roi.EndDate,
        Notes = roi.Notes,
        OrganizationId = roi.OrganizationId,
        CreatedAt = roi.CreatedAt,
        UpdatedAt = roi.UpdatedAt
      };
    }

    public async Task<ROITrackingResponse> CreateROITrackingAsync(CreateROITrackingRequest request, int organizationId)
    {
      var totalCost = request.LaborCost + request.InfrastructureCost + request.OtherCosts;
      var roi = new ROITracking
      {
        ProjectName = request.ProjectName,
        ProjectId = request.ProjectId,
        Investment = request.Investment,
        Return = request.Return,
        ROI = totalCost > 0 ? (request.Return - totalCost) / totalCost : 0,
        LaborCost = request.LaborCost,
        InfrastructureCost = request.InfrastructureCost,
        OtherCosts = request.OtherCosts,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        Notes = request.Notes,
        OrganizationId = organizationId,
        CreatedAt = DateTime.UtcNow
      };

      _context.ROITrackings.Add(roi);
      await _context.SaveChangesAsync();

      return await GetROITrackingAsync(roi.Id) ?? throw new InvalidOperationException("Failed to create ROI tracking");
    }

    public async Task DeleteROITrackingAsync(int id)
    {
      var roi = await _context.ROITrackings.FindAsync(id);
      if (roi == null)
        throw new ArgumentException("ROI tracking not found");

      _context.ROITrackings.Remove(roi);
      await _context.SaveChangesAsync();
    }

    public async Task<ChartDataResponse> GetROIChartDataAsync(int organizationId)
    {
      var rois = await _context.ROITrackings
          .Where(r => r.OrganizationId == organizationId)
          .OrderBy(r => r.StartDate)
          .ToListAsync();

      var labels = rois.Select(r => r.ProjectName).ToList();
      var roiData = rois.Select(r => r.ROI * 100).ToList(); // Convert to percentage

      return new ChartDataResponse
      {
        ChartType = "bar",
        Labels = labels,
        Datasets = new List<ChartDataset>
                {
                    new ChartDataset
                    {
                        Label = "ROI (%)",
                        Data = roiData,
                        BackgroundColor = "rgba(255, 205, 86, 0.2)",
                        BorderColor = "rgba(255, 205, 86, 1)",
                        BorderWidth = 1
                    }
                },
        Options = new ChartOptions
        {
          Title = "Project ROI Comparison",
          XAxisLabel = "Project",
          YAxisLabel = "ROI (%)"
        }
      };
    }

    // Predictive analytics
    public async Task<List<PredictiveAnalyticsResponse>> GetPredictiveAnalyticsAsync(int organizationId)
    {
      return await _context.PredictiveAnalytics
          .Where(p => p.OrganizationId == organizationId)
          .Include(p => p.Predictions)
          .Select(p => new PredictiveAnalyticsResponse
          {
            Id = p.Id,
            ModelName = p.ModelName,
            Type = p.Type,
            TrainingData = p.TrainingData,
            ModelParameters = p.ModelParameters,
            Accuracy = p.Accuracy,
            LastTrained = p.LastTrained,
            OrganizationId = p.OrganizationId,
            CreatedAt = p.CreatedAt,
            Predictions = p.Predictions.Select(pr => new PredictionResponse
            {
              Id = pr.Id,
              InputData = pr.InputData,
              PredictionResult = pr.PredictionResult,
              Confidence = pr.Confidence,
              PredictedAt = pr.PredictedAt
            }).ToList()
          })
          .ToListAsync();
    }

    public async Task<PredictiveAnalyticsResponse?> GetPredictiveAnalyticsAsync(int id)
    {
      var model = await _context.PredictiveAnalytics
          .Include(p => p.Predictions)
          .FirstOrDefaultAsync(p => p.Id == id);

      if (model == null) return null;

      return new PredictiveAnalyticsResponse
      {
        Id = model.Id,
        ModelName = model.ModelName,
        Type = model.Type,
        TrainingData = model.TrainingData,
        ModelParameters = model.ModelParameters,
        Accuracy = model.Accuracy,
        LastTrained = model.LastTrained,
        OrganizationId = model.OrganizationId,
        CreatedAt = model.CreatedAt,
        Predictions = model.Predictions.Select(pr => new PredictionResponse
        {
          Id = pr.Id,
          InputData = pr.InputData,
          PredictionResult = pr.PredictionResult,
          Confidence = pr.Confidence,
          PredictedAt = pr.PredictedAt
        }).ToList()
      };
    }

    public async Task<PredictiveAnalyticsResponse> CreatePredictiveAnalyticsAsync(CreatePredictiveAnalyticsRequest request, int organizationId)
    {
      var model = new PredictiveAnalytics
      {
        ModelName = request.ModelName,
        Type = request.Type,
        TrainingData = request.TrainingData,
        ModelParameters = request.ModelParameters,
        Accuracy = 0.85m, // Simulated accuracy
        LastTrained = DateTime.UtcNow,
        OrganizationId = organizationId,
        CreatedAt = DateTime.UtcNow
      };

      _context.PredictiveAnalytics.Add(model);
      await _context.SaveChangesAsync();

      return await GetPredictiveAnalyticsAsync(model.Id) ?? throw new InvalidOperationException("Failed to create predictive analytics model");
    }

    public async Task DeletePredictiveAnalyticsAsync(int id)
    {
      var model = await _context.PredictiveAnalytics.FindAsync(id);
      if (model == null)
        throw new ArgumentException("Predictive analytics model not found");

      _context.PredictiveAnalytics.Remove(model);
      await _context.SaveChangesAsync();
    }

    public async Task<PredictionResponse> MakePredictionAsync(MakePredictionRequest request)
    {
      var model = await _context.PredictiveAnalytics.FindAsync(request.ModelId);
      if (model == null)
        throw new ArgumentException("Predictive analytics model not found");

      // Simulate prediction
      var random = new Random();
      var confidence = (decimal)(random.NextDouble() * 0.3 + 0.7); // 70-100% confidence
      var predictionResult = JsonSerializer.Serialize(new { predictedValue = random.Next(1, 100), confidence = confidence });

      var prediction = new Prediction
      {
        InputData = request.InputData,
        PredictionResult = predictionResult,
        Confidence = confidence,
        PredictedAt = DateTime.UtcNow,
        ModelId = request.ModelId
      };

      _context.Predictions.Add(prediction);
      await _context.SaveChangesAsync();

      return new PredictionResponse
      {
        Id = prediction.Id,
        InputData = prediction.InputData,
        PredictionResult = prediction.PredictionResult,
        Confidence = prediction.Confidence,
        PredictedAt = prediction.PredictedAt
      };
    }

    // Summary and reports
    public async Task<AnalyticsSummaryResponse> GetAnalyticsSummaryAsync(int organizationId)
    {
      var dashboards = await _context.Dashboards.Where(d => d.OrganizationId == organizationId).ToListAsync();
      var metrics = await _context.AnalyticsMetrics.Where(m => m.OrganizationId == organizationId).ToListAsync();
      var agileMetrics = await _context.AgileMetrics.Include(a => a.Project).Where(a => a.Project.OrganizationId == organizationId).ToListAsync();
      var utilizations = await _context.ResourceUtilizations.Where(r => r.OrganizationId == organizationId).ToListAsync();
      var rois = await _context.ROITrackings.Where(r => r.OrganizationId == organizationId).ToListAsync();
      var predictiveModels = await _context.PredictiveAnalytics.Where(p => p.OrganizationId == organizationId).ToListAsync();

      return new AnalyticsSummaryResponse
      {
        DashboardSummary = new DashboardSummary
        {
          TotalDashboards = dashboards.Count,
          DefaultDashboards = dashboards.Count(d => d.IsDefault),
          TotalWidgets = dashboards.Sum(d => d.Widgets.Count),
          WidgetTypes = dashboards.SelectMany(d => d.Widgets).Select(w => w.Type).Distinct().ToList()
        },
        MetricsSummary = new MetricsSummary
        {
          TotalMetrics = metrics.Count,
          MetricsCalculatedToday = metrics.Count(m => m.LastCalculated.Date == DateTime.UtcNow.Date),
          Categories = metrics.Select(m => m.Category).Distinct().ToList(),
          AverageAccuracy = metrics.Any() ? metrics.Average(m => 0.85m) : 0 // Simulated accuracy
        },
        AgileSummary = new AgileSummary
        {
          TotalSprints = agileMetrics.Count,
          ActiveSprints = agileMetrics.Count(a => a.SprintEndDate >= DateTime.UtcNow),
          AverageVelocity = agileMetrics.Any() ? agileMetrics.Average(a => a.Velocity) : 0,
          AverageBurndownRate = agileMetrics.Any() ? agileMetrics.Average(a => a.BurndownRate) : 0,
          TotalStoryPoints = agileMetrics.Sum(a => a.TotalStoryPoints),
          CompletedStoryPoints = agileMetrics.Sum(a => a.CompletedStoryPoints)
        },
        ResourceSummary = new ResourceSummary
        {
          TotalUsers = utilizations.Select(u => u.UserId).Distinct().Count(),
          AverageUtilizationRate = utilizations.Any() ? utilizations.Average(u => u.UtilizationRate) : 0,
          TotalHoursWorked = utilizations.Sum(u => u.HoursWorked),
          TotalHoursAllocated = utilizations.Sum(u => u.HoursAllocated),
          TopUtilizedUsers = utilizations
                  .GroupBy(u => u.UserId)
                  .Select(g => new { UserId = g.Key, AvgRate = g.Average(u => u.UtilizationRate) })
                  .OrderByDescending(x => x.AvgRate)
                  .Take(5)
                  .Select(x => $"User {x.UserId}")
                  .ToList()
        },
        ROISummary = new ROISummary
        {
          TotalProjects = rois.Count,
          TotalInvestment = rois.Sum(r => r.Investment),
          TotalReturn = rois.Sum(r => r.Return),
          AverageROI = rois.Any() ? rois.Average(r => r.ROI) : 0,
          TotalLaborCost = rois.Sum(r => r.LaborCost),
          TotalInfrastructureCost = rois.Sum(r => r.InfrastructureCost)
        },
        PredictiveSummary = new PredictiveSummary
        {
          TotalModels = predictiveModels.Count,
          ActiveModels = predictiveModels.Count(m => m.LastTrained >= DateTime.UtcNow.AddDays(-30)),
          AverageAccuracy = predictiveModels.Any() ? predictiveModels.Average(m => m.Accuracy) : 0,
          TotalPredictions = predictiveModels.Sum(m => m.Predictions.Count),
          ModelTypes = predictiveModels.Select(m => m.Type).Distinct().ToList()
        }
      };
    }

    public async Task<ReportResponse> GenerateReportAsync(ReportRequest request, int organizationId)
    {
      // Simulate report generation
      var reportData = new
      {
        ReportType = request.ReportType,
        GeneratedAt = DateTime.UtcNow,
        OrganizationId = organizationId,
        Filters = new
        {
          StartDate = request.StartDate,
          EndDate = request.EndDate,
          ProjectId = request.ProjectId,
          UserId = request.UserId,
          Category = request.Category
        },
        Summary = await GetAnalyticsSummaryAsync(organizationId)
      };

      return new ReportResponse
      {
        ReportType = request.ReportType,
        GeneratedAt = DateTime.UtcNow,
        Format = request.Format,
        Data = reportData,
        DownloadUrl = $"/api/analytics/reports/{Guid.NewGuid()}.{request.Format}"
      };
    }
  }
}
