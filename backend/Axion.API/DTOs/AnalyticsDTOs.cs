using System.ComponentModel.DataAnnotations;

namespace Axion.API.DTOs
{
  // Dashboard DTOs
  public class DashboardResponse
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public string Layout { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DashboardWidgetResponse> Widgets { get; set; } = new List<DashboardWidgetResponse>();
  }

  public class CreateDashboardRequest
  {
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsDefault { get; set; }

    public string Layout { get; set; } = "[]";
  }

  public class UpdateDashboardRequest
  {
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsDefault { get; set; }

    public string Layout { get; set; } = "[]";
  }

  public class DashboardWidgetResponse
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Configuration { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
  }

  public class CreateWidgetRequest
  {
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    public string Configuration { get; set; } = "{}";

    public int Position { get; set; }
  }

  // Analytics Metrics DTOs
  public class AnalyticsMetricResponse
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty;
    public DateTime LastCalculated { get; set; }
    public int OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MetricValueResponse> Values { get; set; } = new List<MetricValueResponse>();
  }

  public class CreateMetricRequest
  {
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Query { get; set; } = string.Empty;

    public string Parameters { get; set; } = "{}";
  }

  public class MetricValueResponse
  {
    public int Id { get; set; }
    public decimal Value { get; set; }
    public DateTime CalculatedAt { get; set; }
    public string? Context { get; set; }
  }

  // Burndown Chart DTOs
  public class BurndownChartResponse
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string SprintData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
  }

  public class CreateBurndownChartRequest
  {
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string SprintData { get; set; } = "[]";
  }

  // Agile Metrics DTOs
  public class AgileMetricsResponse
  {
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string SprintId { get; set; } = string.Empty;
    public int TotalStoryPoints { get; set; }
    public int CompletedStoryPoints { get; set; }
    public int RemainingStoryPoints { get; set; }
    public decimal Velocity { get; set; }
    public decimal BurndownRate { get; set; }
    public DateTime SprintStartDate { get; set; }
    public DateTime SprintEndDate { get; set; }
    public DateTime CalculatedAt { get; set; }
  }

  public class CreateAgileMetricsRequest
  {
    [Required]
    public int ProjectId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SprintId { get; set; } = string.Empty;

    [Required]
    public int TotalStoryPoints { get; set; }

    [Required]
    public int CompletedStoryPoints { get; set; }

    [Required]
    public DateTime SprintStartDate { get; set; }

    [Required]
    public DateTime SprintEndDate { get; set; }
  }

  // Resource Utilization DTOs
  public class ResourceUtilizationResponse
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public DateTime Date { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal HoursAllocated { get; set; }
    public decimal UtilizationRate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
  }

  public class CreateResourceUtilizationRequest
  {
    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Range(0, 24)]
    public decimal HoursWorked { get; set; }

    [Required]
    [Range(0, 24)]
    public decimal HoursAllocated { get; set; }

    public string? Notes { get; set; }
  }

  // ROI Tracking DTOs
  public class ROITrackingResponse
  {
    public int Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public decimal Investment { get; set; }
    public decimal Return { get; set; }
    public decimal ROI { get; set; }
    public decimal LaborCost { get; set; }
    public decimal InfrastructureCost { get; set; }
    public decimal OtherCosts { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public int OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
  }

  public class CreateROITrackingRequest
  {
    [Required]
    [MaxLength(100)]
    public string ProjectName { get; set; } = string.Empty;

    [Required]
    public int ProjectId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Investment { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Return { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal LaborCost { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal InfrastructureCost { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal OtherCosts { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Notes { get; set; }
  }

  // Predictive Analytics DTOs
  public class PredictiveAnalyticsResponse
  {
    public int Id { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string TrainingData { get; set; } = string.Empty;
    public string ModelParameters { get; set; } = string.Empty;
    public decimal Accuracy { get; set; }
    public DateTime LastTrained { get; set; }
    public int OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PredictionResponse> Predictions { get; set; } = new List<PredictionResponse>();
  }

  public class CreatePredictiveAnalyticsRequest
  {
    [Required]
    [MaxLength(100)]
    public string ModelName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string TrainingData { get; set; } = string.Empty;

    public string ModelParameters { get; set; } = "{}";
  }

  public class PredictionResponse
  {
    public int Id { get; set; }
    public string InputData { get; set; } = string.Empty;
    public string PredictionResult { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public DateTime PredictedAt { get; set; }
  }

  public class MakePredictionRequest
  {
    [Required]
    public int ModelId { get; set; }

    [Required]
    public string InputData { get; set; } = string.Empty;
  }

  // Analytics Summary DTOs
  public class AnalyticsSummaryResponse
  {
    public DashboardSummary DashboardSummary { get; set; } = new DashboardSummary();
    public MetricsSummary MetricsSummary { get; set; } = new MetricsSummary();
    public AgileSummary AgileSummary { get; set; } = new AgileSummary();
    public ResourceSummary ResourceSummary { get; set; } = new ResourceSummary();
    public ROISummary ROISummary { get; set; } = new ROISummary();
    public PredictiveSummary PredictiveSummary { get; set; } = new PredictiveSummary();
  }

  public class DashboardSummary
  {
    public int TotalDashboards { get; set; }
    public int DefaultDashboards { get; set; }
    public int TotalWidgets { get; set; }
    public List<string> WidgetTypes { get; set; } = new List<string>();
  }

  public class MetricsSummary
  {
    public int TotalMetrics { get; set; }
    public int MetricsCalculatedToday { get; set; }
    public List<string> Categories { get; set; } = new List<string>();
    public decimal AverageAccuracy { get; set; }
  }

  public class AgileSummary
  {
    public int TotalSprints { get; set; }
    public int ActiveSprints { get; set; }
    public decimal AverageVelocity { get; set; }
    public decimal AverageBurndownRate { get; set; }
    public int TotalStoryPoints { get; set; }
    public int CompletedStoryPoints { get; set; }
  }

  public class ResourceSummary
  {
    public int TotalUsers { get; set; }
    public decimal AverageUtilizationRate { get; set; }
    public decimal TotalHoursWorked { get; set; }
    public decimal TotalHoursAllocated { get; set; }
    public List<string> TopUtilizedUsers { get; set; } = new List<string>();
  }

  public class ROISummary
  {
    public int TotalProjects { get; set; }
    public decimal TotalInvestment { get; set; }
    public decimal TotalReturn { get; set; }
    public decimal AverageROI { get; set; }
    public decimal TotalLaborCost { get; set; }
    public decimal TotalInfrastructureCost { get; set; }
  }

  public class PredictiveSummary
  {
    public int TotalModels { get; set; }
    public int ActiveModels { get; set; }
    public decimal AverageAccuracy { get; set; }
    public int TotalPredictions { get; set; }
    public List<string> ModelTypes { get; set; } = new List<string>();
  }

  // Chart Data DTOs
  public class ChartDataResponse
  {
    public string ChartType { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = new List<string>();
    public List<ChartDataset> Datasets { get; set; } = new List<ChartDataset>();
    public ChartOptions Options { get; set; } = new ChartOptions();
  }

  public class ChartDataset
  {
    public string Label { get; set; } = string.Empty;
    public List<decimal> Data { get; set; } = new List<decimal>();
    public string BackgroundColor { get; set; } = string.Empty;
    public string BorderColor { get; set; } = string.Empty;
    public int BorderWidth { get; set; } = 1;
  }

  public class ChartOptions
  {
    public bool Responsive { get; set; } = true;
    public bool MaintainAspectRatio { get; set; } = false;
    public string? Title { get; set; }
    public string? XAxisLabel { get; set; }
    public string? YAxisLabel { get; set; }
  }

  // Report DTOs
  public class ReportRequest
  {
    [Required]
    public string ReportType { get; set; } = string.Empty; // dashboard, metrics, agile, resource, roi, predictive

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ProjectId { get; set; }
    public int? UserId { get; set; }
    public string? Category { get; set; }
    public string? Format { get; set; } = "json"; // json, csv, pdf
  }

  public class ReportResponse
  {
    public string ReportType { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string Format { get; set; } = string.Empty;
    public object Data { get; set; } = new object();
    public string? DownloadUrl { get; set; }
  }
}
