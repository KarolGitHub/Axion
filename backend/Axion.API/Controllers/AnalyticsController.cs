using Microsoft.AspNetCore.Mvc;
using Axion.API.DTOs;
using Axion.API.Services;
using System.Security.Claims;

namespace Axion.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AnalyticsController : ControllerBase
  {
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
      _analyticsService = analyticsService;
    }

    // Dashboard endpoints
    [HttpGet("dashboards")]
    public async Task<ActionResult<List<DashboardResponse>>> GetDashboards()
    {
      var organizationId = GetOrganizationId();
      var dashboards = await _analyticsService.GetDashboardsAsync(organizationId);
      return Ok(dashboards);
    }

    [HttpGet("dashboards/{id}")]
    public async Task<ActionResult<DashboardResponse>> GetDashboard(int id)
    {
      var dashboard = await _analyticsService.GetDashboardAsync(id);
      if (dashboard == null)
        return NotFound();

      return Ok(dashboard);
    }

    [HttpPost("dashboards")]
    public async Task<ActionResult<DashboardResponse>> CreateDashboard(CreateDashboardRequest request)
    {
      var organizationId = GetOrganizationId();
      var userId = GetUserId();
      var dashboard = await _analyticsService.CreateDashboardAsync(request, organizationId, userId);
      return CreatedAtAction(nameof(GetDashboard), new { id = dashboard.Id }, dashboard);
    }

    [HttpPut("dashboards/{id}")]
    public async Task<ActionResult<DashboardResponse>> UpdateDashboard(int id, UpdateDashboardRequest request)
    {
      try
      {
        var dashboard = await _analyticsService.UpdateDashboardAsync(id, request);
        return Ok(dashboard);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpDelete("dashboards/{id}")]
    public async Task<ActionResult> DeleteDashboard(int id)
    {
      try
      {
        await _analyticsService.DeleteDashboardAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpPost("dashboards/{dashboardId}/widgets")]
    public async Task<ActionResult<DashboardWidgetResponse>> AddWidget(int dashboardId, CreateWidgetRequest request)
    {
      try
      {
        var widget = await _analyticsService.AddWidgetAsync(dashboardId, request);
        return CreatedAtAction(nameof(GetDashboard), new { id = dashboardId }, widget);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpDelete("widgets/{widgetId}")]
    public async Task<ActionResult> DeleteWidget(int widgetId)
    {
      try
      {
        await _analyticsService.DeleteWidgetAsync(widgetId);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    // Metrics endpoints
    [HttpGet("metrics")]
    public async Task<ActionResult<List<AnalyticsMetricResponse>>> GetMetrics()
    {
      var organizationId = GetOrganizationId();
      var metrics = await _analyticsService.GetMetricsAsync(organizationId);
      return Ok(metrics);
    }

    [HttpGet("metrics/{id}")]
    public async Task<ActionResult<AnalyticsMetricResponse>> GetMetric(int id)
    {
      var metric = await _analyticsService.GetMetricAsync(id);
      if (metric == null)
        return NotFound();

      return Ok(metric);
    }

    [HttpPost("metrics")]
    public async Task<ActionResult<AnalyticsMetricResponse>> CreateMetric(CreateMetricRequest request)
    {
      var organizationId = GetOrganizationId();
      var metric = await _analyticsService.CreateMetricAsync(request, organizationId);
      return CreatedAtAction(nameof(GetMetric), new { id = metric.Id }, metric);
    }

    [HttpDelete("metrics/{id}")]
    public async Task<ActionResult> DeleteMetric(int id)
    {
      try
      {
        await _analyticsService.DeleteMetricAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpPost("metrics/{id}/calculate")]
    public async Task<ActionResult<decimal>> CalculateMetric(int id)
    {
      try
      {
        var value = await _analyticsService.CalculateMetricAsync(id);
        return Ok(value);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    // Burndown chart endpoints
    [HttpGet("projects/{projectId}/burndown-charts")]
    public async Task<ActionResult<List<BurndownChartResponse>>> GetBurndownCharts(int projectId)
    {
      var charts = await _analyticsService.GetBurndownChartsAsync(projectId);
      return Ok(charts);
    }

    [HttpGet("burndown-charts/{id}")]
    public async Task<ActionResult<BurndownChartResponse>> GetBurndownChart(int id)
    {
      var chart = await _analyticsService.GetBurndownChartAsync(id);
      if (chart == null)
        return NotFound();

      return Ok(chart);
    }

    [HttpPost("burndown-charts")]
    public async Task<ActionResult<BurndownChartResponse>> CreateBurndownChart(CreateBurndownChartRequest request)
    {
      var chart = await _analyticsService.CreateBurndownChartAsync(request);
      return CreatedAtAction(nameof(GetBurndownChart), new { id = chart.Id }, chart);
    }

    [HttpDelete("burndown-charts/{id}")]
    public async Task<ActionResult> DeleteBurndownChart(int id)
    {
      try
      {
        await _analyticsService.DeleteBurndownChartAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpGet("burndown-charts/{id}/data")]
    public async Task<ActionResult<ChartDataResponse>> GetBurndownChartData(int id)
    {
      try
      {
        var data = await _analyticsService.GetBurndownChartDataAsync(id);
        return Ok(data);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    // Agile metrics endpoints
    [HttpGet("projects/{projectId}/agile-metrics")]
    public async Task<ActionResult<List<AgileMetricsResponse>>> GetAgileMetrics(int projectId)
    {
      var metrics = await _analyticsService.GetAgileMetricsAsync(projectId);
      return Ok(metrics);
    }

    [HttpGet("agile-metrics/{id}")]
    public async Task<ActionResult<AgileMetricsResponse>> GetAgileMetrics(int id)
    {
      var metrics = await _analyticsService.GetAgileMetricsAsync(id);
      if (metrics == null)
        return NotFound();

      return Ok(metrics);
    }

    [HttpPost("agile-metrics")]
    public async Task<ActionResult<AgileMetricsResponse>> CreateAgileMetrics(CreateAgileMetricsRequest request)
    {
      var metrics = await _analyticsService.CreateAgileMetricsAsync(request);
      return CreatedAtAction(nameof(GetAgileMetrics), new { id = metrics.Id }, metrics);
    }

    [HttpDelete("agile-metrics/{id}")]
    public async Task<ActionResult> DeleteAgileMetrics(int id)
    {
      try
      {
        await _analyticsService.DeleteAgileMetricsAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpGet("projects/{projectId}/velocity-chart")]
    public async Task<ActionResult<ChartDataResponse>> GetVelocityChartData(int projectId)
    {
      var data = await _analyticsService.GetVelocityChartDataAsync(projectId);
      return Ok(data);
    }

    // Resource utilization endpoints
    [HttpGet("resource-utilization")]
    public async Task<ActionResult<List<ResourceUtilizationResponse>>> GetResourceUtilization(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
      var organizationId = GetOrganizationId();
      var utilizations = await _analyticsService.GetResourceUtilizationAsync(organizationId, startDate, endDate);
      return Ok(utilizations);
    }

    [HttpGet("resource-utilization/{id}")]
    public async Task<ActionResult<ResourceUtilizationResponse>> GetResourceUtilization(int id)
    {
      var utilization = await _analyticsService.GetResourceUtilizationAsync(id);
      if (utilization == null)
        return NotFound();

      return Ok(utilization);
    }

    [HttpPost("resource-utilization")]
    public async Task<ActionResult<ResourceUtilizationResponse>> CreateResourceUtilization(CreateResourceUtilizationRequest request)
    {
      var organizationId = GetOrganizationId();
      var utilization = await _analyticsService.CreateResourceUtilizationAsync(request, organizationId);
      return CreatedAtAction(nameof(GetResourceUtilization), new { id = utilization.Id }, utilization);
    }

    [HttpDelete("resource-utilization/{id}")]
    public async Task<ActionResult> DeleteResourceUtilization(int id)
    {
      try
      {
        await _analyticsService.DeleteResourceUtilizationAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpGet("resource-utilization/chart")]
    public async Task<ActionResult<ChartDataResponse>> GetUtilizationChartData(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
      var organizationId = GetOrganizationId();
      var data = await _analyticsService.GetUtilizationChartDataAsync(organizationId, startDate, endDate);
      return Ok(data);
    }

    // ROI tracking endpoints
    [HttpGet("roi-tracking")]
    public async Task<ActionResult<List<ROITrackingResponse>>> GetROITracking()
    {
      var organizationId = GetOrganizationId();
      var rois = await _analyticsService.GetROITrackingAsync(organizationId);
      return Ok(rois);
    }

    [HttpGet("roi-tracking/{id}")]
    public async Task<ActionResult<ROITrackingResponse>> GetROITracking(int id)
    {
      var roi = await _analyticsService.GetROITrackingAsync(id);
      if (roi == null)
        return NotFound();

      return Ok(roi);
    }

    [HttpPost("roi-tracking")]
    public async Task<ActionResult<ROITrackingResponse>> CreateROITracking(CreateROITrackingRequest request)
    {
      var organizationId = GetOrganizationId();
      var roi = await _analyticsService.CreateROITrackingAsync(request, organizationId);
      return CreatedAtAction(nameof(GetROITracking), new { id = roi.Id }, roi);
    }

    [HttpDelete("roi-tracking/{id}")]
    public async Task<ActionResult> DeleteROITracking(int id)
    {
      try
      {
        await _analyticsService.DeleteROITrackingAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpGet("roi-tracking/chart")]
    public async Task<ActionResult<ChartDataResponse>> GetROIChartData()
    {
      var organizationId = GetOrganizationId();
      var data = await _analyticsService.GetROIChartDataAsync(organizationId);
      return Ok(data);
    }

    // Predictive analytics endpoints
    [HttpGet("predictive-analytics")]
    public async Task<ActionResult<List<PredictiveAnalyticsResponse>>> GetPredictiveAnalytics()
    {
      var organizationId = GetOrganizationId();
      var models = await _analyticsService.GetPredictiveAnalyticsAsync(organizationId);
      return Ok(models);
    }

    [HttpGet("predictive-analytics/{id}")]
    public async Task<ActionResult<PredictiveAnalyticsResponse>> GetPredictiveAnalytics(int id)
    {
      var model = await _analyticsService.GetPredictiveAnalyticsAsync(id);
      if (model == null)
        return NotFound();

      return Ok(model);
    }

    [HttpPost("predictive-analytics")]
    public async Task<ActionResult<PredictiveAnalyticsResponse>> CreatePredictiveAnalytics(CreatePredictiveAnalyticsRequest request)
    {
      var organizationId = GetOrganizationId();
      var model = await _analyticsService.CreatePredictiveAnalyticsAsync(request, organizationId);
      return CreatedAtAction(nameof(GetPredictiveAnalytics), new { id = model.Id }, model);
    }

    [HttpDelete("predictive-analytics/{id}")]
    public async Task<ActionResult> DeletePredictiveAnalytics(int id)
    {
      try
      {
        await _analyticsService.DeletePredictiveAnalyticsAsync(id);
        return NoContent();
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    [HttpPost("predictive-analytics/predict")]
    public async Task<ActionResult<PredictionResponse>> MakePrediction(MakePredictionRequest request)
    {
      try
      {
        var prediction = await _analyticsService.MakePredictionAsync(request);
        return Ok(prediction);
      }
      catch (ArgumentException)
      {
        return NotFound();
      }
    }

    // Summary and reports endpoints
    [HttpGet("summary")]
    public async Task<ActionResult<AnalyticsSummaryResponse>> GetAnalyticsSummary()
    {
      var organizationId = GetOrganizationId();
      var summary = await _analyticsService.GetAnalyticsSummaryAsync(organizationId);
      return Ok(summary);
    }

    [HttpPost("reports")]
    public async Task<ActionResult<ReportResponse>> GenerateReport(ReportRequest request)
    {
      var organizationId = GetOrganizationId();
      var report = await _analyticsService.GenerateReportAsync(request, organizationId);
      return Ok(report);
    }

    // Helper methods
    private int GetOrganizationId()
    {
      // In a real application, this would come from the user's claims or organization context
      // For now, return a default organization ID
      return 1;
    }

    private string GetUserId()
    {
      // In a real application, this would come from the user's claims
      // For now, return a default user ID
      return "default-user-id";
    }
  }
}
