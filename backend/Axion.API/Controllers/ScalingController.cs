using Axion.API.DTOs;
using Axion.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Axion.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class ScalingController : ControllerBase
  {
    private readonly IScalingService _scalingService;

    public ScalingController(IScalingService scalingService)
    {
      _scalingService = scalingService;
    }

    private int GetOrganizationId()
    {
      var organizationIdClaim = User.FindFirst("OrganizationId")?.Value;
      if (int.TryParse(organizationIdClaim, out int organizationId))
      {
        return organizationId;
      }
      throw new UnauthorizedAccessException("Organization ID not found in token");
    }

    // Load Balancer endpoints
    [HttpGet("load-balancers")]
    public async Task<ActionResult<List<LoadBalancerResponse>>> GetLoadBalancers()
    {
      try
      {
        var organizationId = GetOrganizationId();
        var loadBalancers = await _scalingService.GetLoadBalancersAsync(organizationId);
        return Ok(loadBalancers);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("load-balancers/{id}")]
    public async Task<ActionResult<LoadBalancerResponse>> GetLoadBalancer(int id)
    {
      try
      {
        var loadBalancer = await _scalingService.GetLoadBalancerAsync(id);
        if (loadBalancer == null)
          return NotFound();

        return Ok(loadBalancer);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("load-balancers")]
    public async Task<ActionResult<LoadBalancerResponse>> CreateLoadBalancer(CreateLoadBalancerRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var loadBalancer = await _scalingService.CreateLoadBalancerAsync(request, organizationId);
        return CreatedAtAction(nameof(GetLoadBalancer), new { id = loadBalancer.Id }, loadBalancer);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPut("load-balancers/{id}")]
    public async Task<ActionResult<LoadBalancerResponse>> UpdateLoadBalancer(int id, UpdateLoadBalancerRequest request)
    {
      try
      {
        var loadBalancer = await _scalingService.UpdateLoadBalancerAsync(id, request);
        return Ok(loadBalancer);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpDelete("load-balancers/{id}")]
    public async Task<ActionResult> DeleteLoadBalancer(int id)
    {
      try
      {
        await _scalingService.DeleteLoadBalancerAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    // Load Balancer Instance endpoints
    [HttpGet("load-balancers/{loadBalancerId}/instances")]
    public async Task<ActionResult<List<LoadBalancerInstanceResponse>>> GetLoadBalancerInstances(int loadBalancerId)
    {
      try
      {
        var instances = await _scalingService.GetLoadBalancerInstancesAsync(loadBalancerId);
        return Ok(instances);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("load-balancer-instances/{id}")]
    public async Task<ActionResult<LoadBalancerInstanceResponse>> GetLoadBalancerInstance(int id)
    {
      try
      {
        var instance = await _scalingService.GetLoadBalancerInstanceAsync(id);
        if (instance == null)
          return NotFound();

        return Ok(instance);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("load-balancers/{loadBalancerId}/instances")]
    public async Task<ActionResult<LoadBalancerInstanceResponse>> CreateLoadBalancerInstance(int loadBalancerId, CreateLoadBalancerInstanceRequest request)
    {
      try
      {
        var instance = await _scalingService.CreateLoadBalancerInstanceAsync(loadBalancerId, request);
        return CreatedAtAction(nameof(GetLoadBalancerInstance), new { id = instance.Id }, instance);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPut("load-balancer-instances/{id}")]
    public async Task<ActionResult<LoadBalancerInstanceResponse>> UpdateLoadBalancerInstance(int id, UpdateLoadBalancerInstanceRequest request)
    {
      try
      {
        var instance = await _scalingService.UpdateLoadBalancerInstanceAsync(id, request);
        return Ok(instance);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpDelete("load-balancer-instances/{id}")]
    public async Task<ActionResult> DeleteLoadBalancerInstance(int id)
    {
      try
      {
        await _scalingService.DeleteLoadBalancerInstanceAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    // Load Balancer Rule endpoints
    [HttpGet("load-balancers/{loadBalancerId}/rules")]
    public async Task<ActionResult<List<LoadBalancerRuleResponse>>> GetLoadBalancerRules(int loadBalancerId)
    {
      try
      {
        var rules = await _scalingService.GetLoadBalancerRulesAsync(loadBalancerId);
        return Ok(rules);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("load-balancer-rules/{id}")]
    public async Task<ActionResult<LoadBalancerRuleResponse>> GetLoadBalancerRule(int id)
    {
      try
      {
        var rule = await _scalingService.GetLoadBalancerRuleAsync(id);
        if (rule == null)
          return NotFound();

        return Ok(rule);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("load-balancers/{loadBalancerId}/rules")]
    public async Task<ActionResult<LoadBalancerRuleResponse>> CreateLoadBalancerRule(int loadBalancerId, CreateLoadBalancerRuleRequest request)
    {
      try
      {
        var rule = await _scalingService.CreateLoadBalancerRuleAsync(loadBalancerId, request);
        return CreatedAtAction(nameof(GetLoadBalancerRule), new { id = rule.Id }, rule);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpDelete("load-balancer-rules/{id}")]
    public async Task<ActionResult> DeleteLoadBalancerRule(int id)
    {
      try
      {
        await _scalingService.DeleteLoadBalancerRuleAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    // Scaling Policy endpoints
    [HttpGet("scaling-policies")]
    public async Task<ActionResult<List<ScalingPolicyResponse>>> GetScalingPolicies()
    {
      try
      {
        var organizationId = GetOrganizationId();
        var policies = await _scalingService.GetScalingPoliciesAsync(organizationId);
        return Ok(policies);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("scaling-policies/{id}")]
    public async Task<ActionResult<ScalingPolicyResponse>> GetScalingPolicy(int id)
    {
      try
      {
        var policy = await _scalingService.GetScalingPolicyAsync(id);
        if (policy == null)
          return NotFound();

        return Ok(policy);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("scaling-policies")]
    public async Task<ActionResult<ScalingPolicyResponse>> CreateScalingPolicy(CreateScalingPolicyRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var policy = await _scalingService.CreateScalingPolicyAsync(request, organizationId);
        return CreatedAtAction(nameof(GetScalingPolicy), new { id = policy.Id }, policy);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPut("scaling-policies/{id}")]
    public async Task<ActionResult<ScalingPolicyResponse>> UpdateScalingPolicy(int id, UpdateScalingPolicyRequest request)
    {
      try
      {
        var policy = await _scalingService.UpdateScalingPolicyAsync(id, request);
        return Ok(policy);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpDelete("scaling-policies/{id}")]
    public async Task<ActionResult> DeleteScalingPolicy(int id)
    {
      try
      {
        await _scalingService.DeleteScalingPolicyAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    // Scaling Event endpoints
    [HttpGet("scaling-events")]
    public async Task<ActionResult<List<ScalingEventResponse>>> GetScalingEvents([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var events = await _scalingService.GetScalingEventsAsync(organizationId, startDate, endDate);
        return Ok(events);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("scaling-events/{id}")]
    public async Task<ActionResult<ScalingEventResponse>> GetScalingEvent(int id)
    {
      try
      {
        var scalingEvent = await _scalingService.GetScalingEventAsync(id);
        if (scalingEvent == null)
          return NotFound();

        return Ok(scalingEvent);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    // Auto Scaling Group endpoints
    [HttpGet("auto-scaling-groups")]
    public async Task<ActionResult<List<AutoScalingGroupResponse>>> GetAutoScalingGroups()
    {
      try
      {
        var organizationId = GetOrganizationId();
        var groups = await _scalingService.GetAutoScalingGroupsAsync(organizationId);
        return Ok(groups);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("auto-scaling-groups/{id}")]
    public async Task<ActionResult<AutoScalingGroupResponse>> GetAutoScalingGroup(int id)
    {
      try
      {
        var group = await _scalingService.GetAutoScalingGroupAsync(id);
        if (group == null)
          return NotFound();

        return Ok(group);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("auto-scaling-groups")]
    public async Task<ActionResult<AutoScalingGroupResponse>> CreateAutoScalingGroup(CreateAutoScalingGroupRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var group = await _scalingService.CreateAutoScalingGroupAsync(request, organizationId);
        return CreatedAtAction(nameof(GetAutoScalingGroup), new { id = group.Id }, group);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPut("auto-scaling-groups/{id}")]
    public async Task<ActionResult<AutoScalingGroupResponse>> UpdateAutoScalingGroup(int id, UpdateAutoScalingGroupRequest request)
    {
      try
      {
        var group = await _scalingService.UpdateAutoScalingGroupAsync(id, request);
        return Ok(group);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpDelete("auto-scaling-groups/{id}")]
    public async Task<ActionResult> DeleteAutoScalingGroup(int id)
    {
      try
      {
        await _scalingService.DeleteAutoScalingGroupAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    // Auto Scaling Instance endpoints
    [HttpGet("auto-scaling-groups/{autoScalingGroupId}/instances")]
    public async Task<ActionResult<List<AutoScalingInstanceResponse>>> GetAutoScalingInstances(int autoScalingGroupId)
    {
      try
      {
        var instances = await _scalingService.GetAutoScalingInstancesAsync(autoScalingGroupId);
        return Ok(instances);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("auto-scaling-instances/{id}")]
    public async Task<ActionResult<AutoScalingInstanceResponse>> GetAutoScalingInstance(int id)
    {
      try
      {
        var instance = await _scalingService.GetAutoScalingInstanceAsync(id);
        if (instance == null)
          return NotFound();

        return Ok(instance);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    // Metrics endpoints
    [HttpGet("load-balancers/{loadBalancerId}/metrics")]
    public async Task<ActionResult<List<LoadBalancerMetricsResponse>>> GetLoadBalancerMetrics(int loadBalancerId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
      try
      {
        var metrics = await _scalingService.GetLoadBalancerMetricsAsync(loadBalancerId, startDate, endDate);
        return Ok(metrics);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("load-balancers/{loadBalancerId}/metrics")]
    public async Task<ActionResult<LoadBalancerMetricsResponse>> CreateLoadBalancerMetrics(int loadBalancerId, CreateLoadBalancerMetricsRequest request)
    {
      try
      {
        var metrics = await _scalingService.CreateLoadBalancerMetricsAsync(loadBalancerId, request);
        return Ok(metrics);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("auto-scaling-groups/{autoScalingGroupId}/metrics")]
    public async Task<ActionResult<List<ScalingMetricsResponse>>> GetScalingMetrics(int autoScalingGroupId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
      try
      {
        var metrics = await _scalingService.GetScalingMetricsAsync(autoScalingGroupId, startDate, endDate);
        return Ok(metrics);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("auto-scaling-groups/{autoScalingGroupId}/metrics")]
    public async Task<ActionResult<ScalingMetricsResponse>> CreateScalingMetrics(int autoScalingGroupId, CreateScalingMetricsRequest request)
    {
      try
      {
        var metrics = await _scalingService.CreateScalingMetricsAsync(autoScalingGroupId, request);
        return Ok(metrics);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    // Summary and utility endpoints
    [HttpGet("summary")]
    public async Task<ActionResult<ScalingSummaryResponse>> GetScalingSummary()
    {
      try
      {
        var organizationId = GetOrganizationId();
        var summary = await _scalingService.GetScalingSummaryAsync(organizationId);
        return Ok(summary);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("health-check")]
    public async Task<ActionResult<HealthCheckResponse>> PerformHealthCheck(HealthCheckRequest request)
    {
      try
      {
        var result = await _scalingService.PerformHealthCheckAsync(request);
        return Ok(result);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpPost("scaling-action")]
    public async Task<ActionResult<ScalingActionResponse>> ExecuteScalingAction(ScalingActionRequest request)
    {
      try
      {
        var result = await _scalingService.ExecuteScalingActionAsync(request);
        return Ok(result);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }

    [HttpGet("charts/{chartType}")]
    public async Task<ActionResult<ScalingChartDataResponse>> GetScalingChartData(string chartType, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var chartData = await _scalingService.GetScalingChartDataAsync(organizationId, chartType, startDate, endDate);
        return Ok(chartData);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = ex.Message });
      }
    }
  }
}
