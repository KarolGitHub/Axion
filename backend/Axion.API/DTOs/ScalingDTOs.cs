using System.ComponentModel.DataAnnotations;

namespace Axion.API.DTOs
{
    // Load Balancer DTOs
    public class LoadBalancerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int HealthCheckInterval { get; set; }
        public int HealthCheckTimeout { get; set; }
        public int UnhealthyThreshold { get; set; }
        public int HealthyThreshold { get; set; }
        public string? HealthCheckPath { get; set; }
        public string? Configuration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OrganizationId { get; set; }
        public int InstanceCount { get; set; }
        public int HealthyInstanceCount { get; set; }
    }

    public class CreateLoadBalancerRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Algorithm { get; set; } = string.Empty;

        public int HealthCheckInterval { get; set; } = 30;

        public int HealthCheckTimeout { get; set; } = 5;

        public int UnhealthyThreshold { get; set; } = 3;

        public int HealthyThreshold { get; set; } = 2;

        [MaxLength(200)]
        public string? HealthCheckPath { get; set; }

        public string? Configuration { get; set; }
    }

    public class UpdateLoadBalancerRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Algorithm { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int HealthCheckInterval { get; set; }

        public int HealthCheckTimeout { get; set; }

        public int UnhealthyThreshold { get; set; }

        public int HealthyThreshold { get; set; }

        public string? HealthCheckPath { get; set; }

        public string? Configuration { get; set; }
    }

    // Load Balancer Instance DTOs
    public class LoadBalancerInstanceResponse
    {
        public int Id { get; set; }
        public string InstanceId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public int Weight { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime LastHealthCheck { get; set; }
        public DateTime LastResponseTime { get; set; }
        public decimal ResponseTime { get; set; }
        public int ActiveConnections { get; set; }
        public int TotalRequests { get; set; }
        public int FailedRequests { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LoadBalancerId { get; set; }
    }

    public class CreateLoadBalancerInstanceRequest
    {
        [Required]
        [MaxLength(100)]
        public string InstanceId { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string IpAddress { get; set; } = string.Empty;

        public int Port { get; set; } = 80;

        public int Weight { get; set; } = 1;

        public bool IsEnabled { get; set; } = true;
    }

    public class UpdateLoadBalancerInstanceRequest
    {
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public int Weight { get; set; }

        public bool IsEnabled { get; set; }
    }

    // Load Balancer Rule DTOs
    public class LoadBalancerRuleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LoadBalancerId { get; set; }
    }

    public class CreateLoadBalancerRuleRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Condition { get; set; } = string.Empty;

        [Required]
        public string Action { get; set; } = string.Empty;

        public int Priority { get; set; } = 100;

        public bool IsEnabled { get; set; } = true;
    }

    // Scaling Policy DTOs
    public class ScalingPolicyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string MetricName { get; set; } = string.Empty;
        public decimal Threshold { get; set; }
        public int CooldownPeriod { get; set; }
        public int ScaleUpAdjustment { get; set; }
        public int ScaleDownAdjustment { get; set; }
        public int MinInstances { get; set; }
        public int MaxInstances { get; set; }
        public bool IsEnabled { get; set; }
        public string? Schedule { get; set; }
        public string? Conditions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OrganizationId { get; set; }
        public int EventCount { get; set; }
        public int LastEventId { get; set; }
    }

    public class CreateScalingPolicyRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string MetricName { get; set; } = string.Empty;

        [Required]
        public decimal Threshold { get; set; }

        public int CooldownPeriod { get; set; } = 300;

        public int ScaleUpAdjustment { get; set; } = 1;

        public int ScaleDownAdjustment { get; set; } = -1;

        public int MinInstances { get; set; } = 1;

        public int MaxInstances { get; set; } = 10;

        public bool IsEnabled { get; set; } = true;

        public string? Schedule { get; set; }

        public string? Conditions { get; set; }
    }

    public class UpdateScalingPolicyRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string MetricName { get; set; } = string.Empty;

        [Required]
        public decimal Threshold { get; set; }

        public int CooldownPeriod { get; set; }

        public int ScaleUpAdjustment { get; set; }

        public int ScaleDownAdjustment { get; set; }

        public int MinInstances { get; set; }

        public int MaxInstances { get; set; }

        public bool IsEnabled { get; set; }

        public string? Schedule { get; set; }

        public string? Conditions { get; set; }
    }

    // Scaling Event DTOs
    public class ScalingEventResponse
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public decimal MetricValue { get; set; }
        public decimal Threshold { get; set; }
        public int InstancesBefore { get; set; }
        public int InstancesAfter { get; set; }
        public int Adjustment { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime TriggeredAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int ScalingPolicyId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
    }

    // Auto Scaling Group DTOs
    public class AutoScalingGroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int DesiredCapacity { get; set; }
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public int CurrentCapacity { get; set; }
        public int PendingCapacity { get; set; }
        public int InServiceCapacity { get; set; }
        public string? LaunchTemplate { get; set; }
        public string? VpcConfig { get; set; }
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OrganizationId { get; set; }
        public int InstanceCount { get; set; }
        public int PolicyCount { get; set; }
    }

    public class CreateAutoScalingGroupRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public int DesiredCapacity { get; set; } = 1;

        public int MinSize { get; set; } = 1;

        public int MaxSize { get; set; } = 10;

        public string? LaunchTemplate { get; set; }

        public string? VpcConfig { get; set; }

        public string? Tags { get; set; }
    }

    public class UpdateAutoScalingGroupRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public int DesiredCapacity { get; set; }

        public int MinSize { get; set; }

        public int MaxSize { get; set; }

        public string? LaunchTemplate { get; set; }

        public string? VpcConfig { get; set; }

        public string? Tags { get; set; }
    }

    // Auto Scaling Instance DTOs
    public class AutoScalingInstanceResponse
    {
        public int Id { get; set; }
        public string InstanceId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? PrivateIpAddress { get; set; }
        public string? InstanceType { get; set; }
        public string? AvailabilityZone { get; set; }
        public DateTime LaunchTime { get; set; }
        public DateTime? TerminationTime { get; set; }
        public string? TerminationReason { get; set; }
        public decimal CpuUtilization { get; set; }
        public decimal MemoryUtilization { get; set; }
        public int ActiveConnections { get; set; }
        public DateTime LastHealthCheck { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AutoScalingGroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }

    // Load Balancer Metrics DTOs
    public class LoadBalancerMetricsResponse
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
        public DateTime RecordedAt { get; set; }
        public string LoadBalancerName { get; set; } = string.Empty;
    }

    public class CreateLoadBalancerMetricsRequest
    {
        [Required]
        public decimal RequestCount { get; set; }

        [Required]
        public decimal TargetResponseTime { get; set; }

        [Required]
        public decimal HealthyHostCount { get; set; }

        [Required]
        public decimal UnhealthyHostCount { get; set; }

        public decimal TargetConnectionErrorCount { get; set; }

        public decimal TargetTLSNegotiationErrorCount { get; set; }

        public decimal RequestCountPerTarget { get; set; }

        public decimal UnHealthyHostCount { get; set; }
    }

    // Scaling Metrics DTOs
    public class ScalingMetricsResponse
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
        public DateTime RecordedAt { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }

    public class CreateScalingMetricsRequest
    {
        [Required]
        public decimal CpuUtilization { get; set; }

        [Required]
        public decimal MemoryUtilization { get; set; }

        public decimal NetworkIn { get; set; }

        public decimal NetworkOut { get; set; }

        public decimal DiskReadOps { get; set; }

        public decimal DiskWriteOps { get; set; }

        [Required]
        public int InstanceCount { get; set; }
    }

    // Summary DTOs
    public class ScalingSummaryResponse
    {
        public LoadBalancerSummary LoadBalancerSummary { get; set; } = new LoadBalancerSummary();
        public AutoScalingSummary AutoScalingSummary { get; set; } = new AutoScalingSummary();
        public PolicySummary PolicySummary { get; set; } = new PolicySummary();
        public EventSummary EventSummary { get; set; } = new EventSummary();
    }

    public class LoadBalancerSummary
    {
        public int TotalLoadBalancers { get; set; }
        public int ActiveLoadBalancers { get; set; }
        public int TotalInstances { get; set; }
        public int HealthyInstances { get; set; }
        public decimal AverageResponseTime { get; set; }
        public decimal TotalRequests { get; set; }
        public List<string> LoadBalancerTypes { get; set; } = new List<string>();
    }

    public class AutoScalingSummary
    {
        public int TotalGroups { get; set; }
        public int ActiveGroups { get; set; }
        public int TotalInstances { get; set; }
        public int InServiceInstances { get; set; }
        public int PendingInstances { get; set; }
        public decimal AverageCpuUtilization { get; set; }
        public decimal AverageMemoryUtilization { get; set; }
        public List<string> GroupStatuses { get; set; } = new List<string>();
    }

    public class PolicySummary
    {
        public int TotalPolicies { get; set; }
        public int ActivePolicies { get; set; }
        public int CpuBasedPolicies { get; set; }
        public int MemoryBasedPolicies { get; set; }
        public int ScheduledPolicies { get; set; }
        public List<string> PolicyTypes { get; set; } = new List<string>();
    }

    public class EventSummary
    {
        public int TotalEvents { get; set; }
        public int EventsToday { get; set; }
        public int SuccessfulEvents { get; set; }
        public int FailedEvents { get; set; }
        public int ScaleUpEvents { get; set; }
        public int ScaleDownEvents { get; set; }
        public decimal AverageAdjustment { get; set; }
    }

    // Chart Data DTOs
    public class ScalingChartDataResponse
    {
        public string ChartType { get; set; } = string.Empty;
        public List<string> Labels { get; set; } = new List<string>();
        public List<ScalingChartDataset> Datasets { get; set; } = new List<ScalingChartDataset>();
        public ScalingChartOptions Options { get; set; } = new ScalingChartOptions();
    }

    public class ScalingChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public List<decimal> Data { get; set; } = new List<decimal>();
        public string BackgroundColor { get; set; } = string.Empty;
        public string BorderColor { get; set; } = string.Empty;
        public int BorderWidth { get; set; } = 1;
        public bool Fill { get; set; } = false;
    }

    public class ScalingChartOptions
    {
        public bool Responsive { get; set; } = true;
        public bool MaintainAspectRatio { get; set; } = false;
        public string? Title { get; set; }
        public string? XAxisLabel { get; set; }
        public string? YAxisLabel { get; set; }
        public bool ShowLegend { get; set; } = true;
    }

    // Health Check DTOs
    public class HealthCheckRequest
    {
        [Required]
        public int LoadBalancerId { get; set; }

        public int? InstanceId { get; set; } // Optional: check specific instance
    }

    public class HealthCheckResponse
    {
        public int LoadBalancerId { get; set; }
        public string LoadBalancerName { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public int TotalInstances { get; set; }
        public int HealthyInstances { get; set; }
        public int UnhealthyInstances { get; set; }
        public List<InstanceHealthStatus> InstanceStatuses { get; set; } = new List<InstanceHealthStatus>();
        public DateTime CheckedAt { get; set; }
    }

    public class InstanceHealthStatus
    {
        public int InstanceId { get; set; }
        public string InstanceIdStr { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public decimal ResponseTime { get; set; }
        public DateTime LastHealthCheck { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // Scaling Action DTOs
    public class ScalingActionRequest
    {
        [Required]
        public int AutoScalingGroupId { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty; // ScaleUp, ScaleDown, SetCapacity

        public int? Adjustment { get; set; }

        public int? DesiredCapacity { get; set; }

        public string? Reason { get; set; }
    }

    public class ScalingActionResponse
    {
        public int AutoScalingGroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public int InstancesBefore { get; set; }
        public int InstancesAfter { get; set; }
        public int Adjustment { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime ExecutedAt { get; set; }
    }
}

