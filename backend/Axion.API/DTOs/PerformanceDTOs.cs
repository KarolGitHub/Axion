using System.ComponentModel.DataAnnotations;

namespace Axion.API.DTOs
{
  // Cache DTOs
  public class CacheEntryResponse
  {
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastAccessed { get; set; }
    public int AccessCount { get; set; }
    public int OrganizationId { get; set; }
    public string? CacheType { get; set; }
  }

  public class CreateCacheEntryRequest
  {
    [Required]
    [MaxLength(200)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiresAt { get; set; }

    [MaxLength(50)]
    public string? CacheType { get; set; }
  }

  public class UpdateCacheEntryRequest
  {
    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiresAt { get; set; }
  }

  // Performance Metrics DTOs
  public class PerformanceMetricResponse
  {
    public int Id { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
    public int OrganizationId { get; set; }
    public string? Context { get; set; }
    public string? Tags { get; set; }
  }

  public class CreatePerformanceMetricRequest
  {
    [Required]
    [MaxLength(100)]
    public string MetricName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public decimal Value { get; set; }

    [Required]
    [MaxLength(10)]
    public string Unit { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Context { get; set; }

    public string? Tags { get; set; }
  }

  // Database Optimization DTOs
  public class DatabaseOptimizationResponse
  {
    public int Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string OptimizationType { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public decimal ExecutionTime { get; set; }
    public decimal ImprovementPercentage { get; set; }
    public DateTime ExecutedAt { get; set; }
    public bool IsSuccessful { get; set; }
    public string? Notes { get; set; }
    public int OrganizationId { get; set; }
  }

  public class CreateDatabaseOptimizationRequest
  {
    [Required]
    [MaxLength(100)]
    public string TableName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string OptimizationType { get; set; } = string.Empty;

    [Required]
    public string Query { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }
  }

  // API Performance Log DTOs
  public class ApiPerformanceLogResponse
  {
    public int Id { get; set; }
    public string HttpMethod { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public decimal ResponseTime { get; set; }
    public DateTime RequestedAt { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public int OrganizationId { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public decimal RequestSize { get; set; }
    public decimal ResponseSize { get; set; }
  }

  // Memory Usage DTOs
  public class MemoryUsageResponse
  {
    public int Id { get; set; }
    public decimal TotalMemory { get; set; }
    public decimal UsedMemory { get; set; }
    public decimal AvailableMemory { get; set; }
    public decimal MemoryUsagePercentage { get; set; }
    public DateTime RecordedAt { get; set; }
    public int OrganizationId { get; set; }
    public string? ServerInstance { get; set; }
  }

  public class CreateMemoryUsageRequest
  {
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalMemory { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal UsedMemory { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal AvailableMemory { get; set; }

    [MaxLength(50)]
    public string? ServerInstance { get; set; }
  }

  // CPU Usage DTOs
  public class CpuUsageResponse
  {
    public int Id { get; set; }
    public decimal CpuUsagePercentage { get; set; }
    public decimal LoadAverage { get; set; }
    public int ActiveThreads { get; set; }
    public DateTime RecordedAt { get; set; }
    public int OrganizationId { get; set; }
    public string? ServerInstance { get; set; }
  }

  public class CreateCpuUsageRequest
  {
    [Required]
    [Range(0, 100)]
    public decimal CpuUsagePercentage { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal LoadAverage { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int ActiveThreads { get; set; }

    [MaxLength(50)]
    public string? ServerInstance { get; set; }
  }

  // Performance Summary DTOs
  public class PerformanceSummaryResponse
  {
    public CacheSummary CacheSummary { get; set; } = new CacheSummary();
    public MetricsSummary MetricsSummary { get; set; } = new MetricsSummary();
    public DatabaseSummary DatabaseSummary { get; set; } = new DatabaseSummary();
    public ApiSummary ApiSummary { get; set; } = new ApiSummary();
    public SystemSummary SystemSummary { get; set; } = new SystemSummary();
  }

  public class CacheSummary
  {
    public int TotalEntries { get; set; }
    public int ExpiredEntries { get; set; }
    public decimal HitRate { get; set; }
    public decimal AverageAccessCount { get; set; }
    public List<string> CacheTypes { get; set; } = new List<string>();
  }

  public class MetricsSummary
  {
    public int TotalMetrics { get; set; }
    public int MetricsToday { get; set; }
    public List<string> Categories { get; set; } = new List<string>();
    public decimal AverageValue { get; set; }
  }

  public class DatabaseSummary
  {
    public int TotalOptimizations { get; set; }
    public int SuccessfulOptimizations { get; set; }
    public decimal AverageImprovement { get; set; }
    public decimal AverageExecutionTime { get; set; }
    public List<string> TableNames { get; set; } = new List<string>();
  }

  public class ApiSummary
  {
    public int TotalRequests { get; set; }
    public int RequestsToday { get; set; }
    public decimal AverageResponseTime { get; set; }
    public decimal SuccessRate { get; set; }
    public List<string> TopEndpoints { get; set; } = new List<string>();
  }

  public class SystemSummary
  {
    public decimal AverageMemoryUsage { get; set; }
    public decimal AverageCpuUsage { get; set; }
    public decimal AverageLoadAverage { get; set; }
    public int TotalServerInstances { get; set; }
    public List<string> ServerInstances { get; set; } = new List<string>();
  }

  // Performance Chart DTOs
  public class PerformanceChartDataResponse
  {
    public string ChartType { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = new List<string>();
    public List<PerformanceChartDataset> Datasets { get; set; } = new List<PerformanceChartDataset>();
    public PerformanceChartOptions Options { get; set; } = new PerformanceChartOptions();
  }

  public class PerformanceChartDataset
  {
    public string Label { get; set; } = string.Empty;
    public List<decimal> Data { get; set; } = new List<decimal>();
    public string BackgroundColor { get; set; } = string.Empty;
    public string BorderColor { get; set; } = string.Empty;
    public int BorderWidth { get; set; } = 1;
    public bool Fill { get; set; } = false;
  }

  public class PerformanceChartOptions
  {
    public bool Responsive { get; set; } = true;
    public bool MaintainAspectRatio { get; set; } = false;
    public string? Title { get; set; }
    public string? XAxisLabel { get; set; }
    public string? YAxisLabel { get; set; }
    public bool ShowLegend { get; set; } = true;
  }

  // Cache Management DTOs
  public class CacheManagementRequest
  {
    [Required]
    public string Action { get; set; } = string.Empty; // clear, clear_expired, clear_by_type

    public string? CacheType { get; set; }

    public string? Pattern { get; set; } // For pattern-based clearing
  }

  public class CacheManagementResponse
  {
    public string Action { get; set; } = string.Empty;
    public int EntriesAffected { get; set; }
    public DateTime ExecutedAt { get; set; }
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
  }

  // Performance Alert DTOs
  public class PerformanceAlertResponse
  {
    public int Id { get; set; }
    public string AlertType { get; set; } = string.Empty; // memory, cpu, response_time, cache_hit_rate
    public string MetricName { get; set; } = string.Empty;
    public decimal Threshold { get; set; }
    public decimal CurrentValue { get; set; }
    public string Severity { get; set; } = string.Empty; // low, medium, high, critical
    public DateTime TriggeredAt { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? Message { get; set; }
    public int OrganizationId { get; set; }
  }

  public class CreatePerformanceAlertRequest
  {
    [Required]
    [MaxLength(50)]
    public string AlertType { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string MetricName { get; set; } = string.Empty;

    [Required]
    public decimal Threshold { get; set; }

    [Required]
    [MaxLength(20)]
    public string Severity { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Message { get; set; }
  }
}
