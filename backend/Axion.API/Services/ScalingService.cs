using Axion.API.Data;
using Axion.API.DTOs;
using Axion.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Axion.API.Services
{
    public interface IScalingService
    {
        // Load Balancer operations
        Task<List<LoadBalancerResponse>> GetLoadBalancersAsync(int organizationId);
        Task<LoadBalancerResponse?> GetLoadBalancerAsync(int id);
        Task<LoadBalancerResponse> CreateLoadBalancerAsync(CreateLoadBalancerRequest request, int organizationId);
        Task<LoadBalancerResponse> UpdateLoadBalancerAsync(int id, UpdateLoadBalancerRequest request);
        Task DeleteLoadBalancerAsync(int id);

        // Load Balancer Instance operations
        Task<List<LoadBalancerInstanceResponse>> GetLoadBalancerInstancesAsync(int loadBalancerId);
        Task<LoadBalancerInstanceResponse?> GetLoadBalancerInstanceAsync(int id);
        Task<LoadBalancerInstanceResponse> CreateLoadBalancerInstanceAsync(int loadBalancerId, CreateLoadBalancerInstanceRequest request);
        Task<LoadBalancerInstanceResponse> UpdateLoadBalancerInstanceAsync(int id, UpdateLoadBalancerInstanceRequest request);
        Task DeleteLoadBalancerInstanceAsync(int id);

        // Load Balancer Rule operations
        Task<List<LoadBalancerRuleResponse>> GetLoadBalancerRulesAsync(int loadBalancerId);
        Task<LoadBalancerRuleResponse?> GetLoadBalancerRuleAsync(int id);
        Task<LoadBalancerRuleResponse> CreateLoadBalancerRuleAsync(int loadBalancerId, CreateLoadBalancerRuleRequest request);
        Task DeleteLoadBalancerRuleAsync(int id);

        // Scaling Policy operations
        Task<List<ScalingPolicyResponse>> GetScalingPoliciesAsync(int organizationId);
        Task<ScalingPolicyResponse?> GetScalingPolicyAsync(int id);
        Task<ScalingPolicyResponse> CreateScalingPolicyAsync(CreateScalingPolicyRequest request, int organizationId);
        Task<ScalingPolicyResponse> UpdateScalingPolicyAsync(int id, UpdateScalingPolicyRequest request);
        Task DeleteScalingPolicyAsync(int id);

        // Scaling Event operations
        Task<List<ScalingEventResponse>> GetScalingEventsAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ScalingEventResponse?> GetScalingEventAsync(int id);

        // Auto Scaling Group operations
        Task<List<AutoScalingGroupResponse>> GetAutoScalingGroupsAsync(int organizationId);
        Task<AutoScalingGroupResponse?> GetAutoScalingGroupAsync(int id);
        Task<AutoScalingGroupResponse> CreateAutoScalingGroupAsync(CreateAutoScalingGroupRequest request, int organizationId);
        Task<AutoScalingGroupResponse> UpdateAutoScalingGroupAsync(int id, UpdateAutoScalingGroupRequest request);
        Task DeleteAutoScalingGroupAsync(int id);

        // Auto Scaling Instance operations
        Task<List<AutoScalingInstanceResponse>> GetAutoScalingInstancesAsync(int autoScalingGroupId);
        Task<AutoScalingInstanceResponse?> GetAutoScalingInstanceAsync(int id);

        // Metrics operations
        Task<List<LoadBalancerMetricsResponse>> GetLoadBalancerMetricsAsync(int loadBalancerId, DateTime? startDate = null, DateTime? endDate = null);
        Task<LoadBalancerMetricsResponse> CreateLoadBalancerMetricsAsync(int loadBalancerId, CreateLoadBalancerMetricsRequest request);
        Task<List<ScalingMetricsResponse>> GetScalingMetricsAsync(int autoScalingGroupId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ScalingMetricsResponse> CreateScalingMetricsAsync(int autoScalingGroupId, CreateScalingMetricsRequest request);

        // Summary and health check operations
        Task<ScalingSummaryResponse> GetScalingSummaryAsync(int organizationId);
        Task<HealthCheckResponse> PerformHealthCheckAsync(HealthCheckRequest request);
        Task<ScalingActionResponse> ExecuteScalingActionAsync(ScalingActionRequest request);
        Task<ScalingChartDataResponse> GetScalingChartDataAsync(int organizationId, string chartType, DateTime startDate, DateTime endDate);
    }

    public class ScalingService : IScalingService
    {
        private readonly AxionDbContext _context;

        public ScalingService(AxionDbContext context)
        {
            _context = context;
        }

        // Load Balancer operations
        public async Task<List<LoadBalancerResponse>> GetLoadBalancersAsync(int organizationId)
        {
            return await _context.LoadBalancers
                .Where(lb => lb.OrganizationId == organizationId)
                .Include(lb => lb.Instances)
                .Select(lb => new LoadBalancerResponse
                {
                    Id = lb.Id,
                    Name = lb.Name,
                    Type = lb.Type,
                    Algorithm = lb.Algorithm,
                    IsActive = lb.IsActive,
                    HealthCheckInterval = lb.HealthCheckInterval,
                    HealthCheckTimeout = lb.HealthCheckTimeout,
                    UnhealthyThreshold = lb.UnhealthyThreshold,
                    HealthyThreshold = lb.HealthyThreshold,
                    HealthCheckPath = lb.HealthCheckPath,
                    Configuration = lb.Configuration,
                    CreatedAt = lb.CreatedAt,
                    UpdatedAt = lb.UpdatedAt,
                    OrganizationId = lb.OrganizationId,
                    InstanceCount = lb.Instances.Count,
                    HealthyInstanceCount = lb.Instances.Count(i => i.Status == "Healthy")
                })
                .ToListAsync();
        }

        public async Task<LoadBalancerResponse?> GetLoadBalancerAsync(int id)
        {
            var loadBalancer = await _context.LoadBalancers
                .Include(lb => lb.Instances)
                .FirstOrDefaultAsync(lb => lb.Id == id);

            if (loadBalancer == null) return null;

            return new LoadBalancerResponse
            {
                Id = loadBalancer.Id,
                Name = loadBalancer.Name,
                Type = loadBalancer.Type,
                Algorithm = loadBalancer.Algorithm,
                IsActive = loadBalancer.IsActive,
                HealthCheckInterval = loadBalancer.HealthCheckInterval,
                HealthCheckTimeout = loadBalancer.HealthCheckTimeout,
                UnhealthyThreshold = loadBalancer.UnhealthyThreshold,
                HealthyThreshold = loadBalancer.HealthyThreshold,
                HealthCheckPath = loadBalancer.HealthCheckPath,
                Configuration = loadBalancer.Configuration,
                CreatedAt = loadBalancer.CreatedAt,
                UpdatedAt = loadBalancer.UpdatedAt,
                OrganizationId = loadBalancer.OrganizationId,
                InstanceCount = loadBalancer.Instances.Count,
                HealthyInstanceCount = loadBalancer.Instances.Count(i => i.Status == "Healthy")
            };
        }

        public async Task<LoadBalancerResponse> CreateLoadBalancerAsync(CreateLoadBalancerRequest request, int organizationId)
        {
            var loadBalancer = new LoadBalancer
            {
                Name = request.Name,
                Type = request.Type,
                Algorithm = request.Algorithm,
                HealthCheckInterval = request.HealthCheckInterval,
                HealthCheckTimeout = request.HealthCheckTimeout,
                UnhealthyThreshold = request.UnhealthyThreshold,
                HealthyThreshold = request.HealthyThreshold,
                HealthCheckPath = request.HealthCheckPath,
                Configuration = request.Configuration,
                OrganizationId = organizationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.LoadBalancers.Add(loadBalancer);
            await _context.SaveChangesAsync();

            return await GetLoadBalancerAsync(loadBalancer.Id) ?? throw new InvalidOperationException("Failed to create load balancer");
        }

        public async Task<LoadBalancerResponse> UpdateLoadBalancerAsync(int id, UpdateLoadBalancerRequest request)
        {
            var loadBalancer = await _context.LoadBalancers.FindAsync(id);
            if (loadBalancer == null)
                throw new ArgumentException("Load balancer not found");

            loadBalancer.Name = request.Name;
            loadBalancer.Type = request.Type;
            loadBalancer.Algorithm = request.Algorithm;
            loadBalancer.IsActive = request.IsActive;
            loadBalancer.HealthCheckInterval = request.HealthCheckInterval;
            loadBalancer.HealthCheckTimeout = request.HealthCheckTimeout;
            loadBalancer.UnhealthyThreshold = request.UnhealthyThreshold;
            loadBalancer.HealthyThreshold = request.HealthyThreshold;
            loadBalancer.HealthCheckPath = request.HealthCheckPath;
            loadBalancer.Configuration = request.Configuration;
            loadBalancer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetLoadBalancerAsync(id) ?? throw new InvalidOperationException("Failed to update load balancer");
        }

        public async Task DeleteLoadBalancerAsync(int id)
        {
            var loadBalancer = await _context.LoadBalancers.FindAsync(id);
            if (loadBalancer == null)
                throw new ArgumentException("Load balancer not found");

            _context.LoadBalancers.Remove(loadBalancer);
            await _context.SaveChangesAsync();
        }

        // Load Balancer Instance operations
        public async Task<List<LoadBalancerInstanceResponse>> GetLoadBalancerInstancesAsync(int loadBalancerId)
        {
            return await _context.LoadBalancerInstances
                .Where(li => li.LoadBalancerId == loadBalancerId)
                .Select(li => new LoadBalancerInstanceResponse
                {
                    Id = li.Id,
                    InstanceId = li.InstanceId,
                    Status = li.Status,
                    IpAddress = li.IpAddress,
                    Port = li.Port,
                    Weight = li.Weight,
                    IsEnabled = li.IsEnabled,
                    LastHealthCheck = li.LastHealthCheck,
                    LastResponseTime = li.LastResponseTime,
                    ResponseTime = li.ResponseTime,
                    ActiveConnections = li.ActiveConnections,
                    TotalRequests = li.TotalRequests,
                    FailedRequests = li.FailedRequests,
                    CreatedAt = li.CreatedAt,
                    UpdatedAt = li.UpdatedAt,
                    LoadBalancerId = li.LoadBalancerId
                })
                .ToListAsync();
        }

        public async Task<LoadBalancerInstanceResponse?> GetLoadBalancerInstanceAsync(int id)
        {
            var instance = await _context.LoadBalancerInstances.FindAsync(id);
            if (instance == null) return null;

            return new LoadBalancerInstanceResponse
            {
                Id = instance.Id,
                InstanceId = instance.InstanceId,
                Status = instance.Status,
                IpAddress = instance.IpAddress,
                Port = instance.Port,
                Weight = instance.Weight,
                IsEnabled = instance.IsEnabled,
                LastHealthCheck = instance.LastHealthCheck,
                LastResponseTime = instance.LastResponseTime,
                ResponseTime = instance.ResponseTime,
                ActiveConnections = instance.ActiveConnections,
                TotalRequests = instance.TotalRequests,
                FailedRequests = instance.FailedRequests,
                CreatedAt = instance.CreatedAt,
                UpdatedAt = instance.UpdatedAt,
                LoadBalancerId = instance.LoadBalancerId
            };
        }

        public async Task<LoadBalancerInstanceResponse> CreateLoadBalancerInstanceAsync(int loadBalancerId, CreateLoadBalancerInstanceRequest request)
        {
            var instance = new LoadBalancerInstance
            {
                InstanceId = request.InstanceId,
                Status = "Healthy",
                IpAddress = request.IpAddress,
                Port = request.Port,
                Weight = request.Weight,
                IsEnabled = request.IsEnabled,
                LoadBalancerId = loadBalancerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastHealthCheck = DateTime.UtcNow,
                LastResponseTime = DateTime.UtcNow
            };

            _context.LoadBalancerInstances.Add(instance);
            await _context.SaveChangesAsync();

            return await GetLoadBalancerInstanceAsync(instance.Id) ?? throw new InvalidOperationException("Failed to create load balancer instance");
        }

        public async Task<LoadBalancerInstanceResponse> UpdateLoadBalancerInstanceAsync(int id, UpdateLoadBalancerInstanceRequest request)
        {
            var instance = await _context.LoadBalancerInstances.FindAsync(id);
            if (instance == null)
                throw new ArgumentException("Load balancer instance not found");

            instance.Status = request.Status;
            instance.Weight = request.Weight;
            instance.IsEnabled = request.IsEnabled;
            instance.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetLoadBalancerInstanceAsync(id) ?? throw new InvalidOperationException("Failed to update load balancer instance");
        }

        public async Task DeleteLoadBalancerInstanceAsync(int id)
        {
            var instance = await _context.LoadBalancerInstances.FindAsync(id);
            if (instance == null)
                throw new ArgumentException("Load balancer instance not found");

            _context.LoadBalancerInstances.Remove(instance);
            await _context.SaveChangesAsync();
        }

        // Load Balancer Rule operations
        public async Task<List<LoadBalancerRuleResponse>> GetLoadBalancerRulesAsync(int loadBalancerId)
        {
            return await _context.LoadBalancerRules
                .Where(lr => lr.LoadBalancerId == loadBalancerId)
                .Select(lr => new LoadBalancerRuleResponse
                {
                    Id = lr.Id,
                    Name = lr.Name,
                    Type = lr.Type,
                    Condition = lr.Condition,
                    Action = lr.Action,
                    Priority = lr.Priority,
                    IsEnabled = lr.IsEnabled,
                    CreatedAt = lr.CreatedAt,
                    UpdatedAt = lr.UpdatedAt,
                    LoadBalancerId = lr.LoadBalancerId
                })
                .ToListAsync();
        }

        public async Task<LoadBalancerRuleResponse?> GetLoadBalancerRuleAsync(int id)
        {
            var rule = await _context.LoadBalancerRules.FindAsync(id);
            if (rule == null) return null;

            return new LoadBalancerRuleResponse
            {
                Id = rule.Id,
                Name = rule.Name,
                Type = rule.Type,
                Condition = rule.Condition,
                Action = rule.Action,
                Priority = rule.Priority,
                IsEnabled = rule.IsEnabled,
                CreatedAt = rule.CreatedAt,
                UpdatedAt = rule.UpdatedAt,
                LoadBalancerId = rule.LoadBalancerId
            };
        }

        public async Task<LoadBalancerRuleResponse> CreateLoadBalancerRuleAsync(int loadBalancerId, CreateLoadBalancerRuleRequest request)
        {
            var rule = new LoadBalancerRule
            {
                Name = request.Name,
                Type = request.Type,
                Condition = request.Condition,
                Action = request.Action,
                Priority = request.Priority,
                IsEnabled = request.IsEnabled,
                LoadBalancerId = loadBalancerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.LoadBalancerRules.Add(rule);
            await _context.SaveChangesAsync();

            return await GetLoadBalancerRuleAsync(rule.Id) ?? throw new InvalidOperationException("Failed to create load balancer rule");
        }

        public async Task DeleteLoadBalancerRuleAsync(int id)
        {
            var rule = await _context.LoadBalancerRules.FindAsync(id);
            if (rule == null)
                throw new ArgumentException("Load balancer rule not found");

            _context.LoadBalancerRules.Remove(rule);
            await _context.SaveChangesAsync();
        }

        // Scaling Policy operations
        public async Task<List<ScalingPolicyResponse>> GetScalingPoliciesAsync(int organizationId)
        {
            return await _context.ScalingPolicies
                .Where(sp => sp.OrganizationId == organizationId)
                .Include(sp => sp.ScalingEvents)
                .Select(sp => new ScalingPolicyResponse
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    Type = sp.Type,
                    MetricName = sp.MetricName,
                    Threshold = sp.Threshold,
                    CooldownPeriod = sp.CooldownPeriod,
                    ScaleUpAdjustment = sp.ScaleUpAdjustment,
                    ScaleDownAdjustment = sp.ScaleDownAdjustment,
                    MinInstances = sp.MinInstances,
                    MaxInstances = sp.MaxInstances,
                    IsEnabled = sp.IsEnabled,
                    Schedule = sp.Schedule,
                    Conditions = sp.Conditions,
                    CreatedAt = sp.CreatedAt,
                    UpdatedAt = sp.UpdatedAt,
                    OrganizationId = sp.OrganizationId,
                    EventCount = sp.ScalingEvents.Count,
                    LastEventId = sp.ScalingEvents.OrderByDescending(e => e.Id).FirstOrDefault()?.Id ?? 0
                })
                .ToListAsync();
        }

        public async Task<ScalingPolicyResponse?> GetScalingPolicyAsync(int id)
        {
            var policy = await _context.ScalingPolicies
                .Include(sp => sp.ScalingEvents)
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (policy == null) return null;

            return new ScalingPolicyResponse
            {
                Id = policy.Id,
                Name = policy.Name,
                Type = policy.Type,
                MetricName = policy.MetricName,
                Threshold = policy.Threshold,
                CooldownPeriod = policy.CooldownPeriod,
                ScaleUpAdjustment = policy.ScaleUpAdjustment,
                ScaleDownAdjustment = policy.ScaleDownAdjustment,
                MinInstances = policy.MinInstances,
                MaxInstances = policy.MaxInstances,
                IsEnabled = policy.IsEnabled,
                Schedule = policy.Schedule,
                Conditions = policy.Conditions,
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt,
                OrganizationId = policy.OrganizationId,
                EventCount = policy.ScalingEvents.Count,
                LastEventId = policy.ScalingEvents.OrderByDescending(e => e.Id).FirstOrDefault()?.Id ?? 0
            };
        }

        public async Task<ScalingPolicyResponse> CreateScalingPolicyAsync(CreateScalingPolicyRequest request, int organizationId)
        {
            var policy = new ScalingPolicy
            {
                Name = request.Name,
                Type = request.Type,
                MetricName = request.MetricName,
                Threshold = request.Threshold,
                CooldownPeriod = request.CooldownPeriod,
                ScaleUpAdjustment = request.ScaleUpAdjustment,
                ScaleDownAdjustment = request.ScaleDownAdjustment,
                MinInstances = request.MinInstances,
                MaxInstances = request.MaxInstances,
                IsEnabled = request.IsEnabled,
                Schedule = request.Schedule,
                Conditions = request.Conditions,
                OrganizationId = organizationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ScalingPolicies.Add(policy);
            await _context.SaveChangesAsync();

            return await GetScalingPolicyAsync(policy.Id) ?? throw new InvalidOperationException("Failed to create scaling policy");
        }

        public async Task<ScalingPolicyResponse> UpdateScalingPolicyAsync(int id, UpdateScalingPolicyRequest request)
        {
            var policy = await _context.ScalingPolicies.FindAsync(id);
            if (policy == null)
                throw new ArgumentException("Scaling policy not found");

            policy.Name = request.Name;
            policy.Type = request.Type;
            policy.MetricName = request.MetricName;
            policy.Threshold = request.Threshold;
            policy.CooldownPeriod = request.CooldownPeriod;
            policy.ScaleUpAdjustment = request.ScaleUpAdjustment;
            policy.ScaleDownAdjustment = request.ScaleDownAdjustment;
            policy.MinInstances = request.MinInstances;
            policy.MaxInstances = request.MaxInstances;
            policy.IsEnabled = request.IsEnabled;
            policy.Schedule = request.Schedule;
            policy.Conditions = request.Conditions;
            policy.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetScalingPolicyAsync(id) ?? throw new InvalidOperationException("Failed to update scaling policy");
        }

        public async Task DeleteScalingPolicyAsync(int id)
        {
            var policy = await _context.ScalingPolicies.FindAsync(id);
            if (policy == null)
                throw new ArgumentException("Scaling policy not found");

            _context.ScalingPolicies.Remove(policy);
            await _context.SaveChangesAsync();
        }

        // Scaling Event operations
        public async Task<List<ScalingEventResponse>> GetScalingEventsAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.ScalingEvents
                .Include(se => se.ScalingPolicy)
                .Where(se => se.ScalingPolicy.OrganizationId == organizationId);

            if (startDate.HasValue)
                query = query.Where(se => se.TriggeredAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(se => se.TriggeredAt <= endDate.Value);

            return await query
                .Select(se => new ScalingEventResponse
                {
                    Id = se.Id,
                    Action = se.Action,
                    Reason = se.Reason,
                    MetricValue = se.MetricValue,
                    Threshold = se.Threshold,
                    InstancesBefore = se.InstancesBefore,
                    InstancesAfter = se.InstancesAfter,
                    Adjustment = se.Adjustment,
                    IsSuccessful = se.IsSuccessful,
                    ErrorMessage = se.ErrorMessage,
                    TriggeredAt = se.TriggeredAt,
                    CompletedAt = se.CompletedAt,
                    ScalingPolicyId = se.ScalingPolicyId,
                    PolicyName = se.ScalingPolicy.Name
                })
                .OrderByDescending(se => se.TriggeredAt)
                .ToListAsync();
        }

        public async Task<ScalingEventResponse?> GetScalingEventAsync(int id)
        {
            var scalingEvent = await _context.ScalingEvents
                .Include(se => se.ScalingPolicy)
                .FirstOrDefaultAsync(se => se.Id == id);

            if (scalingEvent == null) return null;

            return new ScalingEventResponse
            {
                Id = scalingEvent.Id,
                Action = scalingEvent.Action,
                Reason = scalingEvent.Reason,
                MetricValue = scalingEvent.MetricValue,
                Threshold = scalingEvent.Threshold,
                InstancesBefore = scalingEvent.InstancesBefore,
                InstancesAfter = scalingEvent.InstancesAfter,
                Adjustment = scalingEvent.Adjustment,
                IsSuccessful = scalingEvent.IsSuccessful,
                ErrorMessage = scalingEvent.ErrorMessage,
                TriggeredAt = scalingEvent.TriggeredAt,
                CompletedAt = scalingEvent.CompletedAt,
                ScalingPolicyId = scalingEvent.ScalingPolicyId,
                PolicyName = scalingEvent.ScalingPolicy.Name
            };
        }

        // Auto Scaling Group operations
        public async Task<List<AutoScalingGroupResponse>> GetAutoScalingGroupsAsync(int organizationId)
        {
            return await _context.AutoScalingGroups
                .Where(asg => asg.OrganizationId == organizationId)
                .Include(asg => asg.Instances)
                .Include(asg => asg.ScalingPolicies)
                .Select(asg => new AutoScalingGroupResponse
                {
                    Id = asg.Id,
                    Name = asg.Name,
                    Status = asg.Status,
                    DesiredCapacity = asg.DesiredCapacity,
                    MinSize = asg.MinSize,
                    MaxSize = asg.MaxSize,
                    CurrentCapacity = asg.CurrentCapacity,
                    PendingCapacity = asg.PendingCapacity,
                    InServiceCapacity = asg.InServiceCapacity,
                    LaunchTemplate = asg.LaunchTemplate,
                    VpcConfig = asg.VpcConfig,
                    Tags = asg.Tags,
                    CreatedAt = asg.CreatedAt,
                    UpdatedAt = asg.UpdatedAt,
                    OrganizationId = asg.OrganizationId,
                    InstanceCount = asg.Instances.Count,
                    PolicyCount = asg.ScalingPolicies.Count
                })
                .ToListAsync();
        }

        public async Task<AutoScalingGroupResponse?> GetAutoScalingGroupAsync(int id)
        {
            var group = await _context.AutoScalingGroups
                .Include(asg => asg.Instances)
                .Include(asg => asg.ScalingPolicies)
                .FirstOrDefaultAsync(asg => asg.Id == id);

            if (group == null) return null;

            return new AutoScalingGroupResponse
            {
                Id = group.Id,
                Name = group.Name,
                Status = group.Status,
                DesiredCapacity = group.DesiredCapacity,
                MinSize = group.MinSize,
                MaxSize = group.MaxSize,
                CurrentCapacity = group.CurrentCapacity,
                PendingCapacity = group.PendingCapacity,
                InServiceCapacity = group.InServiceCapacity,
                LaunchTemplate = group.LaunchTemplate,
                VpcConfig = group.VpcConfig,
                Tags = group.Tags,
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt,
                OrganizationId = group.OrganizationId,
                InstanceCount = group.Instances.Count,
                PolicyCount = group.ScalingPolicies.Count
            };
        }

        public async Task<AutoScalingGroupResponse> CreateAutoScalingGroupAsync(CreateAutoScalingGroupRequest request, int organizationId)
        {
            var group = new AutoScalingGroup
            {
                Name = request.Name,
                Status = request.Status,
                DesiredCapacity = request.DesiredCapacity,
                MinSize = request.MinSize,
                MaxSize = request.MaxSize,
                CurrentCapacity = request.DesiredCapacity,
                PendingCapacity = 0,
                InServiceCapacity = request.DesiredCapacity,
                LaunchTemplate = request.LaunchTemplate,
                VpcConfig = request.VpcConfig,
                Tags = request.Tags,
                OrganizationId = organizationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.AutoScalingGroups.Add(group);
            await _context.SaveChangesAsync();

            return await GetAutoScalingGroupAsync(group.Id) ?? throw new InvalidOperationException("Failed to create auto scaling group");
        }

        public async Task<AutoScalingGroupResponse> UpdateAutoScalingGroupAsync(int id, UpdateAutoScalingGroupRequest request)
        {
            var group = await _context.AutoScalingGroups.FindAsync(id);
            if (group == null)
                throw new ArgumentException("Auto scaling group not found");

            group.Name = request.Name;
            group.Status = request.Status;
            group.DesiredCapacity = request.DesiredCapacity;
            group.MinSize = request.MinSize;
            group.MaxSize = request.MaxSize;
            group.LaunchTemplate = request.LaunchTemplate;
            group.VpcConfig = request.VpcConfig;
            group.Tags = request.Tags;
            group.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetAutoScalingGroupAsync(id) ?? throw new InvalidOperationException("Failed to update auto scaling group");
        }

        public async Task DeleteAutoScalingGroupAsync(int id)
        {
            var group = await _context.AutoScalingGroups.FindAsync(id);
            if (group == null)
                throw new ArgumentException("Auto scaling group not found");

            _context.AutoScalingGroups.Remove(group);
            await _context.SaveChangesAsync();
        }

        // Auto Scaling Instance operations
        public async Task<List<AutoScalingInstanceResponse>> GetAutoScalingInstancesAsync(int autoScalingGroupId)
        {
            return await _context.AutoScalingInstances
                .Where(asi => asi.AutoScalingGroupId == autoScalingGroupId)
                .Include(asi => asi.AutoScalingGroup)
                .Select(asi => new AutoScalingInstanceResponse
                {
                    Id = asi.Id,
                    InstanceId = asi.InstanceId,
                    Status = asi.Status,
                    IpAddress = asi.IpAddress,
                    PrivateIpAddress = asi.PrivateIpAddress,
                    InstanceType = asi.InstanceType,
                    AvailabilityZone = asi.AvailabilityZone,
                    LaunchTime = asi.LaunchTime,
                    TerminationTime = asi.TerminationTime,
                    TerminationReason = asi.TerminationReason,
                    CpuUtilization = asi.CpuUtilization,
                    MemoryUtilization = asi.MemoryUtilization,
                    ActiveConnections = asi.ActiveConnections,
                    LastHealthCheck = asi.LastHealthCheck,
                    CreatedAt = asi.CreatedAt,
                    UpdatedAt = asi.UpdatedAt,
                    AutoScalingGroupId = asi.AutoScalingGroupId,
                    GroupName = asi.AutoScalingGroup.Name
                })
                .ToListAsync();
        }

        public async Task<AutoScalingInstanceResponse?> GetAutoScalingInstanceAsync(int id)
        {
            var instance = await _context.AutoScalingInstances
                .Include(asi => asi.AutoScalingGroup)
                .FirstOrDefaultAsync(asi => asi.Id == id);

            if (instance == null) return null;

            return new AutoScalingInstanceResponse
            {
                Id = instance.Id,
                InstanceId = instance.InstanceId,
                Status = instance.Status,
                IpAddress = instance.IpAddress,
                PrivateIpAddress = instance.PrivateIpAddress,
                InstanceType = instance.InstanceType,
                AvailabilityZone = instance.AvailabilityZone,
                LaunchTime = instance.LaunchTime,
                TerminationTime = instance.TerminationTime,
                TerminationReason = instance.TerminationReason,
                CpuUtilization = instance.CpuUtilization,
                MemoryUtilization = instance.MemoryUtilization,
                ActiveConnections = instance.ActiveConnections,
                LastHealthCheck = instance.LastHealthCheck,
                CreatedAt = instance.CreatedAt,
                UpdatedAt = instance.UpdatedAt,
                AutoScalingGroupId = instance.AutoScalingGroupId,
                GroupName = instance.AutoScalingGroup.Name
            };
        }

        // Metrics operations
        public async Task<List<LoadBalancerMetricsResponse>> GetLoadBalancerMetricsAsync(int loadBalancerId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.LoadBalancerMetrics
                .Where(lbm => lbm.LoadBalancerId == loadBalancerId);

            if (startDate.HasValue)
                query = query.Where(lbm => lbm.RecordedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(lbm => lbm.RecordedAt <= endDate.Value);

            return await query
                .Include(lbm => lbm.LoadBalancer)
                .Select(lbm => new LoadBalancerMetricsResponse
                {
                    Id = lbm.Id,
                    LoadBalancerId = lbm.LoadBalancerId,
                    RequestCount = lbm.RequestCount,
                    TargetResponseTime = lbm.TargetResponseTime,
                    HealthyHostCount = lbm.HealthyHostCount,
                    UnhealthyHostCount = lbm.UnhealthyHostCount,
                    TargetConnectionErrorCount = lbm.TargetConnectionErrorCount,
                    TargetTLSNegotiationErrorCount = lbm.TargetTLSNegotiationErrorCount,
                    RequestCountPerTarget = lbm.RequestCountPerTarget,
                    UnHealthyHostCount = lbm.UnHealthyHostCount,
                    RecordedAt = lbm.RecordedAt,
                    LoadBalancerName = lbm.LoadBalancer.Name
                })
                .OrderByDescending(lbm => lbm.RecordedAt)
                .ToListAsync();
        }

        public async Task<LoadBalancerMetricsResponse> CreateLoadBalancerMetricsAsync(int loadBalancerId, CreateLoadBalancerMetricsRequest request)
        {
            var metrics = new LoadBalancerMetrics
            {
                LoadBalancerId = loadBalancerId,
                RequestCount = request.RequestCount,
                TargetResponseTime = request.TargetResponseTime,
                HealthyHostCount = request.HealthyHostCount,
                UnhealthyHostCount = request.UnhealthyHostCount,
                TargetConnectionErrorCount = request.TargetConnectionErrorCount,
                TargetTLSNegotiationErrorCount = request.TargetTLSNegotiationErrorCount,
                RequestCountPerTarget = request.RequestCountPerTarget,
                UnHealthyHostCount = request.UnHealthyHostCount,
                RecordedAt = DateTime.UtcNow
            };

            _context.LoadBalancerMetrics.Add(metrics);
            await _context.SaveChangesAsync();

            return await GetLoadBalancerMetricsAsync(loadBalancerId).ContinueWith(t => t.Result.FirstOrDefault()) ?? throw new InvalidOperationException("Failed to create load balancer metrics");
        }

        public async Task<List<ScalingMetricsResponse>> GetScalingMetricsAsync(int autoScalingGroupId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.ScalingMetrics
                .Where(sm => sm.AutoScalingGroupId == autoScalingGroupId);

            if (startDate.HasValue)
                query = query.Where(sm => sm.RecordedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(sm => sm.RecordedAt <= endDate.Value);

            return await query
                .Include(sm => sm.AutoScalingGroup)
                .Select(sm => new ScalingMetricsResponse
                {
                    Id = sm.Id,
                    AutoScalingGroupId = sm.AutoScalingGroupId,
                    CpuUtilization = sm.CpuUtilization,
                    MemoryUtilization = sm.MemoryUtilization,
                    NetworkIn = sm.NetworkIn,
                    NetworkOut = sm.NetworkOut,
                    DiskReadOps = sm.DiskReadOps,
                    DiskWriteOps = sm.DiskWriteOps,
                    InstanceCount = sm.InstanceCount,
                    RecordedAt = sm.RecordedAt,
                    GroupName = sm.AutoScalingGroup.Name
                })
                .OrderByDescending(sm => sm.RecordedAt)
                .ToListAsync();
        }

        public async Task<ScalingMetricsResponse> CreateScalingMetricsAsync(int autoScalingGroupId, CreateScalingMetricsRequest request)
        {
            var metrics = new ScalingMetrics
            {
                AutoScalingGroupId = autoScalingGroupId,
                CpuUtilization = request.CpuUtilization,
                MemoryUtilization = request.MemoryUtilization,
                NetworkIn = request.NetworkIn,
                NetworkOut = request.NetworkOut,
                DiskReadOps = request.DiskReadOps,
                DiskWriteOps = request.DiskWriteOps,
                InstanceCount = request.InstanceCount,
                RecordedAt = DateTime.UtcNow
            };

            _context.ScalingMetrics.Add(metrics);
            await _context.SaveChangesAsync();

            return await GetScalingMetricsAsync(autoScalingGroupId).ContinueWith(t => t.Result.FirstOrDefault()) ?? throw new InvalidOperationException("Failed to create scaling metrics");
        }

        // Summary and health check operations
        public async Task<ScalingSummaryResponse> GetScalingSummaryAsync(int organizationId)
        {
            var loadBalancers = await _context.LoadBalancers
                .Include(lb => lb.Instances)
                .Where(lb => lb.OrganizationId == organizationId)
                .ToListAsync();

            var autoScalingGroups = await _context.AutoScalingGroups
                .Include(asg => asg.Instances)
                .Include(asg => asg.ScalingPolicies)
                .Where(asg => asg.OrganizationId == organizationId)
                .ToListAsync();

            var scalingPolicies = await _context.ScalingPolicies
                .Include(sp => sp.ScalingEvents)
                .Where(sp => sp.OrganizationId == organizationId)
                .ToListAsync();

            var scalingEvents = await _context.ScalingEvents
                .Include(se => se.ScalingPolicy)
                .Where(se => se.ScalingPolicy.OrganizationId == organizationId)
                .ToListAsync();

            return new ScalingSummaryResponse
            {
                LoadBalancerSummary = new LoadBalancerSummary
                {
                    TotalLoadBalancers = loadBalancers.Count,
                    ActiveLoadBalancers = loadBalancers.Count(lb => lb.IsActive),
                    TotalInstances = loadBalancers.Sum(lb => lb.Instances.Count),
                    HealthyInstances = loadBalancers.Sum(lb => lb.Instances.Count(i => i.Status == "Healthy")),
                    AverageResponseTime = loadBalancers.SelectMany(lb => lb.Instances).Any()
                        ? loadBalancers.SelectMany(lb => lb.Instances).Average(i => i.ResponseTime)
                        : 0,
                    TotalRequests = loadBalancers.SelectMany(lb => lb.Instances).Sum(i => i.TotalRequests),
                    LoadBalancerTypes = loadBalancers.Select(lb => lb.Type).Distinct().ToList()
                },
                AutoScalingSummary = new AutoScalingSummary
                {
                    TotalGroups = autoScalingGroups.Count,
                    ActiveGroups = autoScalingGroups.Count(asg => asg.Status == "Active"),
                    TotalInstances = autoScalingGroups.Sum(asg => asg.Instances.Count),
                    InServiceInstances = autoScalingGroups.Sum(asg => asg.InServiceCapacity),
                    PendingInstances = autoScalingGroups.Sum(asg => asg.PendingCapacity),
                    AverageCpuUtilization = autoScalingGroups.SelectMany(asg => asg.Instances).Any()
                        ? autoScalingGroups.SelectMany(asg => asg.Instances).Average(i => i.CpuUtilization)
                        : 0,
                    AverageMemoryUtilization = autoScalingGroups.SelectMany(asg => asg.Instances).Any()
                        ? autoScalingGroups.SelectMany(asg => asg.Instances).Average(i => i.MemoryUtilization)
                        : 0,
                    GroupStatuses = autoScalingGroups.Select(asg => asg.Status).Distinct().ToList()
                },
                PolicySummary = new PolicySummary
                {
                    TotalPolicies = scalingPolicies.Count,
                    ActivePolicies = scalingPolicies.Count(sp => sp.IsEnabled),
                    CpuBasedPolicies = scalingPolicies.Count(sp => sp.Type == "CPU"),
                    MemoryBasedPolicies = scalingPolicies.Count(sp => sp.Type == "Memory"),
                    ScheduledPolicies = scalingPolicies.Count(sp => sp.Type == "Schedule"),
                    PolicyTypes = scalingPolicies.Select(sp => sp.Type).Distinct().ToList()
                },
                EventSummary = new EventSummary
                {
                    TotalEvents = scalingEvents.Count,
                    EventsToday = scalingEvents.Count(se => se.TriggeredAt.Date == DateTime.UtcNow.Date),
                    SuccessfulEvents = scalingEvents.Count(se => se.IsSuccessful),
                    FailedEvents = scalingEvents.Count(se => !se.IsSuccessful),
                    ScaleUpEvents = scalingEvents.Count(se => se.Action == "ScaleUp"),
                    ScaleDownEvents = scalingEvents.Count(se => se.Action == "ScaleDown"),
                    AverageAdjustment = scalingEvents.Any() ? scalingEvents.Average(se => se.Adjustment) : 0
                }
            };
        }

        public async Task<HealthCheckResponse> PerformHealthCheckAsync(HealthCheckRequest request)
        {
            var loadBalancer = await _context.LoadBalancers
                .Include(lb => lb.Instances)
                .FirstOrDefaultAsync(lb => lb.Id == request.LoadBalancerId);

            if (loadBalancer == null)
                throw new ArgumentException("Load balancer not found");

            var instances = request.InstanceId.HasValue
                ? loadBalancer.Instances.Where(i => i.Id == request.InstanceId.Value).ToList()
                : loadBalancer.Instances.ToList();

            var instanceStatuses = new List<InstanceHealthStatus>();
            var healthyCount = 0;
            var unhealthyCount = 0;

            foreach (var instance in instances)
            {
                // Simulate health check
                var isHealthy = instance.Status == "Healthy";
                if (isHealthy) healthyCount++;
                else unhealthyCount++;

                instanceStatuses.Add(new InstanceHealthStatus
                {
                    InstanceId = instance.Id,
                    InstanceIdStr = instance.InstanceId,
                    Status = instance.Status,
                    IpAddress = instance.IpAddress,
                    ResponseTime = instance.ResponseTime,
                    LastHealthCheck = instance.LastHealthCheck,
                    ErrorMessage = instance.Status != "Healthy" ? "Health check failed" : null
                });
            }

            return new HealthCheckResponse
            {
                LoadBalancerId = loadBalancer.Id,
                LoadBalancerName = loadBalancer.Name,
                IsHealthy = healthyCount > 0,
                TotalInstances = instances.Count,
                HealthyInstances = healthyCount,
                UnhealthyInstances = unhealthyCount,
                InstanceStatuses = instanceStatuses,
                CheckedAt = DateTime.UtcNow
            };
        }

        public async Task<ScalingActionResponse> ExecuteScalingActionAsync(ScalingActionRequest request)
        {
            var group = await _context.AutoScalingGroups.FindAsync(request.AutoScalingGroupId);
            if (group == null)
                throw new ArgumentException("Auto scaling group not found");

            var instancesBefore = group.CurrentCapacity;
            var instancesAfter = instancesBefore;
            var adjustment = 0;

            switch (request.Action.ToLower())
            {
                case "scaleup":
                    adjustment = request.Adjustment ?? 1;
                    instancesAfter = Math.Min(instancesBefore + adjustment, group.MaxSize);
                    break;
                case "scaledown":
                    adjustment = request.Adjustment ?? -1;
                    instancesAfter = Math.Max(instancesBefore + adjustment, group.MinSize);
                    break;
                case "setcapacity":
                    instancesAfter = request.DesiredCapacity ?? group.DesiredCapacity;
                    adjustment = instancesAfter - instancesBefore;
                    break;
                default:
                    throw new ArgumentException("Invalid scaling action");
            }

            group.CurrentCapacity = instancesAfter;
            group.DesiredCapacity = instancesAfter;
            group.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ScalingActionResponse
            {
                AutoScalingGroupId = group.Id,
                GroupName = group.Name,
                Action = request.Action,
                IsSuccessful = true,
                InstancesBefore = instancesBefore,
                InstancesAfter = instancesAfter,
                Adjustment = adjustment,
                ExecutedAt = DateTime.UtcNow
            };
        }

        public async Task<ScalingChartDataResponse> GetScalingChartDataAsync(int organizationId, string chartType, DateTime startDate, DateTime endDate)
        {
            var labels = new List<string>();
            var datasets = new List<ScalingChartDataset>();

            switch (chartType.ToLower())
            {
                case "load_balancer_requests":
                    var lbMetrics = await _context.LoadBalancerMetrics
                        .Include(lbm => lbm.LoadBalancer)
                        .Where(lbm => lbm.LoadBalancer.OrganizationId == organizationId &&
                                     lbm.RecordedAt >= startDate && lbm.RecordedAt <= endDate)
                        .OrderBy(lbm => lbm.RecordedAt)
                        .ToListAsync();

                    labels = lbMetrics.Select(lbm => lbm.RecordedAt.ToString("HH:mm")).ToList();
                    datasets.Add(new ScalingChartDataset
                    {
                        Label = "Request Count",
                        Data = lbMetrics.Select(lbm => lbm.RequestCount).ToList(),
                        BackgroundColor = "rgba(54, 162, 235, 0.2)",
                        BorderColor = "rgba(54, 162, 235, 1)",
                        BorderWidth = 2,
                        Fill = true
                    });
                    break;

                case "scaling_events":
                    var events = await _context.ScalingEvents
                        .Include(se => se.ScalingPolicy)
                        .Where(se => se.ScalingPolicy.OrganizationId == organizationId &&
                                   se.TriggeredAt >= startDate && se.TriggeredAt <= endDate)
                        .OrderBy(se => se.TriggeredAt)
                        .ToListAsync();

                    labels = events.Select(e => e.TriggeredAt.ToString("HH:mm")).ToList();
                    datasets.Add(new ScalingChartDataset
                    {
                        Label = "Adjustment",
                        Data = events.Select(e => (decimal)e.Adjustment).ToList(),
                        BackgroundColor = "rgba(255, 99, 132, 0.2)",
                        BorderColor = "rgba(255, 99, 132, 1)",
                        BorderWidth = 2,
                        Fill = false
                    });
                    break;

                case "instance_count":
                    var scalingMetrics = await _context.ScalingMetrics
                        .Include(sm => sm.AutoScalingGroup)
                        .Where(sm => sm.AutoScalingGroup.OrganizationId == organizationId &&
                                   sm.RecordedAt >= startDate && sm.RecordedAt <= endDate)
                        .OrderBy(sm => sm.RecordedAt)
                        .ToListAsync();

                    labels = scalingMetrics.Select(sm => sm.RecordedAt.ToString("HH:mm")).ToList();
                    datasets.Add(new ScalingChartDataset
                    {
                        Label = "Instance Count",
                        Data = scalingMetrics.Select(sm => (decimal)sm.InstanceCount).ToList(),
                        BackgroundColor = "rgba(75, 192, 192, 0.2)",
                        BorderColor = "rgba(75, 192, 192, 1)",
                        BorderWidth = 2,
                        Fill = true
                    });
                    break;
            }

            return new ScalingChartDataResponse
            {
                ChartType = "line",
                Labels = labels,
                Datasets = datasets,
                Options = new ScalingChartOptions
                {
                    Title = $"{chartType.Replace("_", " ").ToUpper()}",
                    XAxisLabel = "Time",
                    YAxisLabel = chartType.ToLower() switch
                    {
                        "load_balancer_requests" => "Request Count",
                        "scaling_events" => "Adjustment",
                        "instance_count" => "Instance Count",
                        _ => "Value"
                    }
                }
            };
        }
    }
}

