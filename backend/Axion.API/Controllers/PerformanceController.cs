using Microsoft.AspNetCore.Mvc;
using Axion.API.DTOs;
using Axion.API.Services;
using Axion.API.Models;

namespace Axion.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PerformanceController : ControllerBase
  {
    private readonly IPerformanceService _performanceService;

    public PerformanceController(IPerformanceService performanceService)
    {
      _performanceService = performanceService;
    }

    // Cache endpoints
    [HttpGet("cache")]
    public async Task<ActionResult<List<CacheEntryResponse>>> GetCacheEntries()
    {
      var organizationId = GetOrganizationId();
      var entries = await _performanceService.GetCacheEntriesAsync(organizationId);
      return Ok(entries);
    }

    [HttpGet("cache/{key}")]
    public async Task<ActionResult<CacheEntryResponse>> GetCacheEntry(string key)
    {
      var organizationId = GetOrganizationId();
      var entry = await _performanceService.GetCacheEntryAsync(key, organizationId);
      if (entry == null)
        return NotFound();

      return Ok(entry);
    }

    [HttpPost("cache")]
    public async Task<ActionResult<CacheEntryResponse>> CreateCacheEntry(CreateCacheEntryRequest request)
    {
      var organizationId = GetOrganizationId();
      var entry = await _performanceService.CreateCacheEntryAsync(request, organizationId);
      return CreatedAtAction(nameof(GetCacheEntry), new { key = entry.Key }, entry);
    }

    [HttpPut("cache/{key}")]
    public async Task<ActionResult<CacheEntryResponse>> UpdateCacheEntry(string key, UpdateCacheEntryRequest request)
    {
      try
      {
        var organizationId = GetOrganizationId();
        var entry = await _performanceService.UpdateCacheEntryAsync(key, request, organizationId);
        return Ok(entry);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpDelete("cache/{key}")]
    public async Task<ActionResult> DeleteCacheEntry(string key)
    {
      try
      {
        var organizationId = GetOrganizationId();
        await _performanceService.DeleteCacheEntryAsync(key, organizationId);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpPost("cache/manage")]
    public async Task<ActionResult<CacheManagementResponse>> ManageCache(CacheManagementRequest request)
    {
      var organizationId = GetOrganizationId();
      var result = await _performanceService.ManageCacheAsync(request, organizationId);
      return Ok(result);
    }

    // Performance metrics endpoints
    [HttpGet("metrics")]
    public async Task<ActionResult<List<PerformanceMetricResponse>>> GetPerformanceMetrics([FromQuery] string? category = null)
    {
      var organizationId = GetOrganizationId();
      var metrics = await _performanceService.GetPerformanceMetricsAsync(organizationId, category);
      return Ok(metrics);
    }

    [HttpGet("metrics/{id}")]
    public async Task<ActionResult<PerformanceMetricResponse>> GetPerformanceMetric(int id)
    {
      var metric = await _performanceService.GetPerformanceMetricAsync(id);
      if (metric == null)
        return NotFound();

      return Ok(metric);
    }

    [HttpPost("metrics")]
    public async Task<ActionResult<PerformanceMetricResponse>> CreatePerformanceMetric(CreatePerformanceMetricRequest request)
    {
      var organizationId = GetOrganizationId();
      var metric = await _performanceService.CreatePerformanceMetricAsync(request, organizationId);
      return CreatedAtAction(nameof(GetPerformanceMetric), new { id = metric.Id }, metric);
    }

    [HttpDelete("metrics/{id}")]
    public async Task<ActionResult> DeletePerformanceMetric(int id)
    {
      try
      {
        await _performanceService.DeletePerformanceMetricAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    // Database optimization endpoints
    [HttpGet("database-optimizations")]
    public async Task<ActionResult<List<DatabaseOptimizationResponse>>> GetDatabaseOptimizations()
    {
      var organizationId = GetOrganizationId();
      var optimizations = await _performanceService.GetDatabaseOptimizationsAsync(organizationId);
      return Ok(optimizations);
    }

    [HttpGet("database-optimizations/{id}")]
    public async Task<ActionResult<DatabaseOptimizationResponse>> GetDatabaseOptimization(int id)
    {
      var optimization = await _performanceService.GetDatabaseOptimizationAsync(id);
      if (optimization == null)
        return NotFound();

      return Ok(optimization);
    }

    [HttpPost("database-optimizations")]
    public async Task<ActionResult<DatabaseOptimizationResponse>> CreateDatabaseOptimization(CreateDatabaseOptimizationRequest request)
    {
      var organizationId = GetOrganizationId();
      var optimization = await _performanceService.CreateDatabaseOptimizationAsync(request, organizationId);
      return CreatedAtAction(nameof(GetDatabaseOptimization), new { id = optimization.Id }, optimization);
    }

    [HttpDelete("database-optimizations/{id}")]
    public async Task<ActionResult> DeleteDatabaseOptimization(int id)
    {
      try
      {
        await _performanceService.DeleteDatabaseOptimizationAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpPost("database-optimizations/{id}/execute")]
    public async Task<ActionResult<DatabaseOptimizationResponse>> ExecuteOptimization(int id)
    {
      try
      {
        var optimization = await _performanceService.ExecuteOptimizationAsync(id);
        return Ok(optimization);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    // API performance logs endpoints
    [HttpGet("api-logs")]
    public async Task<ActionResult<List<ApiPerformanceLogResponse>>> GetApiPerformanceLogs(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
      var organizationId = GetOrganizationId();
      var logs = await _performanceService.GetApiPerformanceLogsAsync(organizationId, startDate, endDate);
      return Ok(logs);
    }

    [HttpGet("api-logs/{id}")]
    public async Task<ActionResult<ApiPerformanceLogResponse>> GetApiPerformanceLog(int id)
    {
      var log = await _performanceService.GetApiPerformanceLogAsync(id);
      if (log == null)
        return NotFound();

      return Ok(log);
    }

    // Memory usage endpoints
    [HttpGet("memory-usage")]
    public async Task<ActionResult<List<MemoryUsageResponse>>> GetMemoryUsage(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
      var organizationId = GetOrganizationId();
      var usage = await _performanceService.GetMemoryUsageAsync(organizationId, startDate, endDate);
      return Ok(usage);
    }

    [HttpGet("memory-usage/{id}")]
    public async Task<ActionResult<MemoryUsageResponse>> GetMemoryUsage(int id)
    {
      var usage = await _performanceService.GetMemoryUsageAsync(id);
      if (usage == null)
        return NotFound();

      return Ok(usage);
    }

    [HttpPost("memory-usage")]
    public async Task<ActionResult<MemoryUsageResponse>> CreateMemoryUsage(CreateMemoryUsageRequest request)
    {
      var organizationId = GetOrganizationId();
      var usage = await _performanceService.CreateMemoryUsageAsync(request, organizationId);
      return CreatedAtAction(nameof(GetMemoryUsage), new { id = usage.Id }, usage);
    }

    [HttpDelete("memory-usage/{id}")]
    public async Task<ActionResult> DeleteMemoryUsage(int id)
    {
      try
      {
        await _performanceService.DeleteMemoryUsageAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    // CPU usage endpoints
    [HttpGet("cpu-usage")]
    public async Task<ActionResult<List<CpuUsageResponse>>> GetCpuUsage(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
      var organizationId = GetOrganizationId();
      var usage = await _performanceService.GetCpuUsageAsync(organizationId, startDate, endDate);
      return Ok(usage);
    }

    [HttpGet("cpu-usage/{id}")]
    public async Task<ActionResult<CpuUsageResponse>> GetCpuUsage(int id)
    {
      var usage = await _performanceService.GetCpuUsageAsync(id);
      if (usage == null)
        return NotFound();

      return Ok(usage);
    }

    [HttpPost("cpu-usage")]
    public async Task<ActionResult<CpuUsageResponse>> CreateCpuUsage(CreateCpuUsageRequest request)
    {
      var organizationId = GetOrganizationId();
      var usage = await _performanceService.CreateCpuUsageAsync(request, organizationId);
      return CreatedAtAction(nameof(GetCpuUsage), new { id = usage.Id }, usage);
    }

    [HttpDelete("cpu-usage/{id}")]
    public async Task<ActionResult> DeleteCpuUsage(int id)
    {
      try
      {
        await _performanceService.DeleteCpuUsageAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    // Summary and charts endpoints
    [HttpGet("summary")]
    public async Task<ActionResult<PerformanceSummaryResponse>> GetPerformanceSummary()
    {
      var organizationId = GetOrganizationId();
      var summary = await _performanceService.GetPerformanceSummaryAsync(organizationId);
      return Ok(summary);
    }

    [HttpGet("charts/{chartType}")]
    public async Task<ActionResult<PerformanceChartDataResponse>> GetPerformanceChartData(
        string chartType,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
      var organizationId = GetOrganizationId();
      var data = await _performanceService.GetPerformanceChartDataAsync(organizationId, chartType, startDate, endDate);
      return Ok(data);
    }

    // Performance alerts endpoints
    [HttpGet("alerts")]
    public async Task<ActionResult<List<PerformanceAlertResponse>>> GetPerformanceAlerts()
    {
      var organizationId = GetOrganizationId();
      var alerts = await _performanceService.GetPerformanceAlertsAsync(organizationId);
      return Ok(alerts);
    }

    [HttpGet("alerts/{id}")]
    public async Task<ActionResult<PerformanceAlertResponse>> GetPerformanceAlert(int id)
    {
      var alert = await _performanceService.GetPerformanceAlertAsync(id);
      if (alert == null)
        return NotFound();

      return Ok(alert);
    }

    [HttpPost("alerts")]
    public async Task<ActionResult<PerformanceAlertResponse>> CreatePerformanceAlert(CreatePerformanceAlertRequest request)
    {
      var organizationId = GetOrganizationId();
      var alert = await _performanceService.CreatePerformanceAlertAsync(request, organizationId);
      return CreatedAtAction(nameof(GetPerformanceAlert), new { id = alert.Id }, alert);
    }

    [HttpPost("alerts/{id}/resolve")]
    public async Task<ActionResult<PerformanceAlertResponse>> ResolveAlert(int id)
    {
      var alert = await _performanceService.ResolveAlertAsync(id);
      return Ok(alert);
    }

    // Helper methods
    private int GetOrganizationId()
    {
      // In a real application, this would come from the user's claims or organization context
      // For now, return a default organization ID
      return 1;
    }
  }
}
