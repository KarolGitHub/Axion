using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Axion.API.Models
{
  public class CacheEntry
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty; // JSON serialized data

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

    public int AccessCount { get; set; } = 0;

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    [MaxLength(50)]
    public string? CacheType { get; set; } // user, project, analytics, etc.
  }

  public class PerformanceMetric
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string MetricName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // api, database, cache, memory, cpu

    public decimal Value { get; set; }

    public string Unit { get; set; } = string.Empty; // ms, mb, %, etc.

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    [MaxLength(500)]
    public string? Context { get; set; } // Additional context data

    public string? Tags { get; set; } = "[]"; // JSON array of tags
  }

  public class DatabaseOptimization
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string TableName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string OptimizationType { get; set; } = string.Empty; // index, partition, cleanup

    public string Query { get; set; } = string.Empty; // SQL query for optimization

    public decimal ExecutionTime { get; set; } // Time taken to execute optimization

    public decimal ImprovementPercentage { get; set; } // Performance improvement percentage

    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    public bool IsSuccessful { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;
  }

  public class ApiPerformanceLog
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string HttpMethod { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Endpoint { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public decimal ResponseTime { get; set; } // Response time in milliseconds

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    [MaxLength(50)]
    public string? UserAgent { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    public string? RequestBody { get; set; } // JSON request body (truncated)

    public string? ResponseBody { get; set; } // JSON response body (truncated)

    public decimal RequestSize { get; set; } // Request size in bytes

    public decimal ResponseSize { get; set; } // Response size in bytes
  }

  public class MemoryUsage
  {
    public int Id { get; set; }

    public decimal TotalMemory { get; set; } // Total memory in MB

    public decimal UsedMemory { get; set; } // Used memory in MB

    public decimal AvailableMemory { get; set; } // Available memory in MB

    public decimal MemoryUsagePercentage { get; set; } // Memory usage percentage

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    [MaxLength(50)]
    public string? ServerInstance { get; set; }
  }

  public class CpuUsage
  {
    public int Id { get; set; }

    public decimal CpuUsagePercentage { get; set; } // CPU usage percentage

    public decimal LoadAverage { get; set; } // System load average

    public int ActiveThreads { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    [MaxLength(50)]
    public string? ServerInstance { get; set; }
  }
}
