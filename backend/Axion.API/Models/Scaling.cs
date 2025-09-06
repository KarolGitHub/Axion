using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Axion.API.Models
{
    public class LoadBalancer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Application, Network, Gateway

        [Required]
        [MaxLength(20)]
        public string Algorithm { get; set; } = string.Empty; // RoundRobin, LeastConnections, Weighted, etc.

        public bool IsActive { get; set; } = true;

        public int HealthCheckInterval { get; set; } = 30; // seconds

        public int HealthCheckTimeout { get; set; } = 5; // seconds

        public int UnhealthyThreshold { get; set; } = 3;

        public int HealthyThreshold { get; set; } = 2;

        public string? HealthCheckPath { get; set; }

        public string? Configuration { get; set; } // JSON configuration

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int OrganizationId { get; set; }

        // Navigation properties
        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<LoadBalancerInstance> Instances { get; set; } = new List<LoadBalancerInstance>();
        public virtual ICollection<LoadBalancerRule> Rules { get; set; } = new List<LoadBalancerRule>();
    }

    public class LoadBalancerInstance
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string InstanceId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Healthy, Unhealthy, OutOfService

        [Required]
        [MaxLength(15)]
        public string IpAddress { get; set; } = string.Empty;

        public int Port { get; set; } = 80;

        public int Weight { get; set; } = 1;

        public bool IsEnabled { get; set; } = true;

        public DateTime LastHealthCheck { get; set; } = DateTime.UtcNow;

        public DateTime LastResponseTime { get; set; } = DateTime.UtcNow;

        public decimal ResponseTime { get; set; } = 0;

        public int ActiveConnections { get; set; } = 0;

        public int TotalRequests { get; set; } = 0;

        public int FailedRequests { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int LoadBalancerId { get; set; }

        // Navigation properties
        public virtual LoadBalancer LoadBalancer { get; set; } = null!;
    }

    public class LoadBalancerRule
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty; // Path, Host, Header, etc.

        [Required]
        public string Condition { get; set; } = string.Empty; // JSON condition

        [Required]
        public string Action { get; set; } = string.Empty; // JSON action

        public int Priority { get; set; } = 100;

        public bool IsEnabled { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int LoadBalancerId { get; set; }

        // Navigation properties
        public virtual LoadBalancer LoadBalancer { get; set; } = null!;
    }

    public class ScalingPolicy
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // CPU, Memory, Custom, Schedule

        [Required]
        public string MetricName { get; set; } = string.Empty;

        public decimal Threshold { get; set; }

        public int CooldownPeriod { get; set; } = 300; // seconds

        public int ScaleUpAdjustment { get; set; } = 1;

        public int ScaleDownAdjustment { get; set; } = -1;

        public int MinInstances { get; set; } = 1;

        public int MaxInstances { get; set; } = 10;

        public bool IsEnabled { get; set; } = true;

        public string? Schedule { get; set; } // Cron expression for scheduled scaling

        public string? Conditions { get; set; } // JSON additional conditions

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int OrganizationId { get; set; }

        // Navigation properties
        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<ScalingEvent> ScalingEvents { get; set; } = new List<ScalingEvent>();
    }

    public class ScalingEvent
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // ScaleUp, ScaleDown

        [Required]
        public string Reason { get; set; } = string.Empty;

        public decimal MetricValue { get; set; }

        public decimal Threshold { get; set; }

        public int InstancesBefore { get; set; }

        public int InstancesAfter { get; set; }

        public int Adjustment { get; set; }

        public bool IsSuccessful { get; set; } = true;

        public string? ErrorMessage { get; set; }

        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public int ScalingPolicyId { get; set; }

        // Navigation properties
        public virtual ScalingPolicy ScalingPolicy { get; set; } = null!;
    }

    public class AutoScalingGroup
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Active, Suspended, Deleted

        public int DesiredCapacity { get; set; } = 1;

        public int MinSize { get; set; } = 1;

        public int MaxSize { get; set; } = 10;

        public int CurrentCapacity { get; set; } = 1;

        public int PendingCapacity { get; set; } = 0;

        public int InServiceCapacity { get; set; } = 1;

        public string? LaunchTemplate { get; set; } // JSON launch template

        public string? VpcConfig { get; set; } // JSON VPC configuration

        public string? Tags { get; set; } // JSON tags

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int OrganizationId { get; set; }

        // Navigation properties
        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<ScalingPolicy> ScalingPolicies { get; set; } = new List<ScalingPolicy>();
        public virtual ICollection<AutoScalingInstance> Instances { get; set; } = new List<AutoScalingInstance>();
    }

    public class AutoScalingInstance
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string InstanceId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Pending, InService, Terminating, Terminated

        [Required]
        [MaxLength(15)]
        public string IpAddress { get; set; } = string.Empty;

        public string? PrivateIpAddress { get; set; }

        public string? InstanceType { get; set; }

        public string? AvailabilityZone { get; set; }

        public DateTime LaunchTime { get; set; } = DateTime.UtcNow;

        public DateTime? TerminationTime { get; set; }

        public string? TerminationReason { get; set; }

        public decimal CpuUtilization { get; set; } = 0;

        public decimal MemoryUtilization { get; set; } = 0;

        public int ActiveConnections { get; set; } = 0;

        public DateTime LastHealthCheck { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int AutoScalingGroupId { get; set; }

        // Navigation properties
        public virtual AutoScalingGroup AutoScalingGroup { get; set; } = null!;
    }

    public class LoadBalancerMetrics
    {
        public int Id { get; set; }

        public int LoadBalancerId { get; set; }

        public decimal RequestCount { get; set; }

        public decimal TargetResponseTime { get; set; }

        public decimal HealthyHostCount { get; set; }

        public decimal UnhealthyHostCount { get; set; }

        public decimal TargetConnectionErrorCount { get; set; }

        public decimal TargetTLSNegotiationErrorCount { get; set; }

        public decimal RequestCountPerTarget { get; set; }

        public decimal UnHealthyHostCount { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual LoadBalancer LoadBalancer { get; set; } = null!;
    }

    public class ScalingMetrics
    {
        public int Id { get; set; }

        public int AutoScalingGroupId { get; set; }

        public decimal CpuUtilization { get; set; }

        public decimal MemoryUtilization { get; set; }

        public decimal NetworkIn { get; set; }

        public decimal NetworkOut { get; set; }

        public decimal DiskReadOps { get; set; }

        public decimal DiskWriteOps { get; set; }

        public int InstanceCount { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual AutoScalingGroup AutoScalingGroup { get; set; } = null!;
    }
}

