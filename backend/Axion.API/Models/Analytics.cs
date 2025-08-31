using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Axion.API.Models
{
  public class Dashboard
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsDefault { get; set; }

    public string Layout { get; set; } = "[]"; // JSON string for dashboard layout

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    public int CreatedById { get; set; }
    public virtual User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<DashboardWidget> Widgets { get; set; } = new List<DashboardWidget>();
  }

  public class DashboardWidget
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty; // chart, metric, table, etc.

    public string Configuration { get; set; } = "{}"; // JSON string for widget config

    public int Position { get; set; }

    public int DashboardId { get; set; }
    public virtual Dashboard Dashboard { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }

  public class AnalyticsMetric
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // project, task, resource, etc.

    public string Query { get; set; } = string.Empty; // SQL or LINQ query

    public string Parameters { get; set; } = "{}"; // JSON string for query parameters

    public DateTime LastCalculated { get; set; }

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<MetricValue> Values { get; set; } = new List<MetricValue>();
  }

  public class MetricValue
  {
    public int Id { get; set; }

    public decimal Value { get; set; }

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    public string? Context { get; set; } // Additional context data

    public int MetricId { get; set; }
    public virtual AnalyticsMetric Metric { get; set; } = null!;
  }

  public class BurndownChart
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string SprintData { get; set; } = "[]"; // JSON string for sprint data

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
  }

  public class AgileMetrics
  {
    public int Id { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;

    public string SprintId { get; set; } = string.Empty;

    public int TotalStoryPoints { get; set; }
    public int CompletedStoryPoints { get; set; }
    public int RemainingStoryPoints { get; set; }

    public decimal Velocity { get; set; }
    public decimal BurndownRate { get; set; }

    public DateTime SprintStartDate { get; set; }
    public DateTime SprintEndDate { get; set; }

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
  }

  public class ResourceUtilization
  {
    public int Id { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    public DateTime Date { get; set; }

    public decimal HoursWorked { get; set; }
    public decimal HoursAllocated { get; set; }
    public decimal UtilizationRate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }

  public class ROITracking
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProjectName { get; set; } = string.Empty;

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;

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
    public virtual Organization Organization { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
  }

  public class PredictiveAnalytics
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ModelName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty; // completion_time, resource_needs, etc.

    public string TrainingData { get; set; } = "[]"; // JSON string for training data

    public string ModelParameters { get; set; } = "{}"; // JSON string for model parameters

    public decimal Accuracy { get; set; }

    public DateTime LastTrained { get; set; }

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
  }

  public class Prediction
  {
    public int Id { get; set; }

    public string InputData { get; set; } = "{}"; // JSON string for input data

    public string PredictionResult { get; set; } = "{}"; // JSON string for prediction result

    public decimal Confidence { get; set; }

    public DateTime PredictedAt { get; set; } = DateTime.UtcNow;

    public int ModelId { get; set; }
    public virtual PredictiveAnalytics Model { get; set; } = null!;
  }
}
