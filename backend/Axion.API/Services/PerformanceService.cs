using Axion.API.Data;
using Axion.API.DTOs;
using Axion.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Axion.API.Services
{
  public interface IPerformanceService
  {
    // Cache operations
    Task<List<CacheEntryResponse>> GetCacheEntriesAsync(int organizationId);
    Task<CacheEntryResponse?> GetCacheEntryAsync(string key, int organizationId);
    Task<CacheEntryResponse> CreateCacheEntryAsync(CreateCacheEntryRequest request, int organizationId);
    Task<CacheEntryResponse> UpdateCacheEntryAsync(string key, UpdateCacheEntryRequest request, int organizationId);
    Task DeleteCacheEntryAsync(string key, int organizationId);
    Task<CacheManagementResponse> ManageCacheAsync(CacheManagementRequest request, int organizationId);

    // Performance metrics
    Task<List<PerformanceMetricResponse>> GetPerformanceMetricsAsync(int organizationId, string? category = null);
    Task<PerformanceMetricResponse?> GetPerformanceMetricAsync(int id);
    Task<PerformanceMetricResponse> CreatePerformanceMetricAsync(CreatePerformanceMetricRequest request, int organizationId);
    Task DeletePerformanceMetricAsync(int id);

    // Database optimization
    Task<List<DatabaseOptimizationResponse>> GetDatabaseOptimizationsAsync(int organizationId);
    Task<DatabaseOptimizationResponse?> GetDatabaseOptimizationAsync(int id);
    Task<DatabaseOptimizationResponse> CreateDatabaseOptimizationAsync(CreateDatabaseOptimizationRequest request, int organizationId);
    Task DeleteDatabaseOptimizationAsync(int id);
    Task<DatabaseOptimizationResponse> ExecuteOptimizationAsync(int id);

    // API performance logging
    Task<List<ApiPerformanceLogResponse>> GetApiPerformanceLogsAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiPerformanceLogResponse?> GetApiPerformanceLogAsync(int id);
    Task<ApiPerformanceLogResponse> CreateApiPerformanceLogAsync(ApiPerformanceLog log);

    // Memory usage
    Task<List<MemoryUsageResponse>> GetMemoryUsageAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null);
    Task<MemoryUsageResponse?> GetMemoryUsageAsync(int id);
    Task<MemoryUsageResponse> CreateMemoryUsageAsync(CreateMemoryUsageRequest request, int organizationId);
    Task DeleteMemoryUsageAsync(int id);

    // CPU usage
    Task<List<CpuUsageResponse>> GetCpuUsageAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null);
    Task<CpuUsageResponse?> GetCpuUsageAsync(int id);
    Task<CpuUsageResponse> CreateCpuUsageAsync(CreateCpuUsageRequest request, int organizationId);
    Task DeleteCpuUsageAsync(int id);

    // Summary and charts
    Task<PerformanceSummaryResponse> GetPerformanceSummaryAsync(int organizationId);
    Task<PerformanceChartDataResponse> GetPerformanceChartDataAsync(int organizationId, string chartType, DateTime startDate, DateTime endDate);

    // Performance alerts
    Task<List<PerformanceAlertResponse>> GetPerformanceAlertsAsync(int organizationId);
    Task<PerformanceAlertResponse?> GetPerformanceAlertAsync(int id);
    Task<PerformanceAlertResponse> CreatePerformanceAlertAsync(CreatePerformanceAlertRequest request, int organizationId);
    Task<PerformanceAlertResponse> ResolveAlertAsync(int id);
  }

  public class PerformanceService : IPerformanceService
  {
    private readonly AxionDbContext _context;

    public PerformanceService(AxionDbContext context)
    {
      _context = context;
    }

    // Cache operations
    public async Task<List<CacheEntryResponse>> GetCacheEntriesAsync(int organizationId)
    {
      return await _context.CacheEntries
          .Where(c => c.OrganizationId == organizationId)
          .Select(c => new CacheEntryResponse
          {
            Id = c.Id,
            Key = c.Key,
            Value = c.Value,
            ExpiresAt = c.ExpiresAt,
            CreatedAt = c.CreatedAt,
            LastAccessed = c.LastAccessed,
            AccessCount = c.AccessCount,
            OrganizationId = c.OrganizationId,
            CacheType = c.CacheType
          })
          .ToListAsync();
    }

    public async Task<CacheEntryResponse?> GetCacheEntryAsync(string key, int organizationId)
    {
      var entry = await _context.CacheEntries
          .FirstOrDefaultAsync(c => c.Key == key && c.OrganizationId == organizationId);

      if (entry == null) return null;

      // Update access count and last accessed time
      entry.AccessCount++;
      entry.LastAccessed = DateTime.UtcNow;
      await _context.SaveChangesAsync();

      return new CacheEntryResponse
      {
        Id = entry.Id,
        Key = entry.Key,
        Value = entry.Value,
        ExpiresAt = entry.ExpiresAt,
        CreatedAt = entry.CreatedAt,
        LastAccessed = entry.LastAccessed,
        AccessCount = entry.AccessCount,
        OrganizationId = entry.OrganizationId,
        CacheType = entry.CacheType
      };
    }

    public async Task<CacheEntryResponse> CreateCacheEntryAsync(CreateCacheEntryRequest request, int organizationId)
    {
      var entry = new CacheEntry
      {
        Key = request.Key,
        Value = request.Value,
        ExpiresAt = request.ExpiresAt,
        CacheType = request.CacheType,
        OrganizationId = organizationId,
        CreatedAt = DateTime.UtcNow,
        LastAccessed = DateTime.UtcNow,
        AccessCount = 0
      };

      _context.CacheEntries.Add(entry);
      await _context.SaveChangesAsync();

      return new CacheEntryResponse
      {
        Id = entry.Id,
        Key = entry.Key,
        Value = entry.Value,
        ExpiresAt = entry.ExpiresAt,
        CreatedAt = entry.CreatedAt,
        LastAccessed = entry.LastAccessed,
        AccessCount = entry.AccessCount,
        OrganizationId = entry.OrganizationId,
        CacheType = entry.CacheType
      };
    }

    public async Task<CacheEntryResponse> UpdateCacheEntryAsync(string key, UpdateCacheEntryRequest request, int organizationId)
    {
      var entry = await _context.CacheEntries
          .FirstOrDefaultAsync(c => c.Key == key && c.OrganizationId == organizationId);

      if (entry == null)
        throw new ArgumentException("Cache entry not found");

      entry.Value = request.Value;
      entry.ExpiresAt = request.ExpiresAt;
      entry.LastAccessed = DateTime.UtcNow;
      entry.AccessCount++;

      await _context.SaveChangesAsync();

      return new CacheEntryResponse
      {
        Id = entry.Id,
        Key = entry.Key,
        Value = entry.Value,
        ExpiresAt = entry.ExpiresAt,
        CreatedAt = entry.CreatedAt,
        LastAccessed = entry.LastAccessed,
        AccessCount = entry.AccessCount,
        OrganizationId = entry.OrganizationId,
        CacheType = entry.CacheType
      };
    }

    public async Task DeleteCacheEntryAsync(string key, int organizationId)
    {
      var entry = await _context.CacheEntries
          .FirstOrDefaultAsync(c => c.Key == key && c.OrganizationId == organizationId);

      if (entry == null)
        throw new ArgumentException("Cache entry not found");

      _context.CacheEntries.Remove(entry);
      await _context.SaveChangesAsync();
    }

    public async Task<CacheManagementResponse> ManageCacheAsync(CacheManagementRequest request, int organizationId)
    {
      var entriesAffected = 0;

      switch (request.Action.ToLower())
      {
        case "clear":
          var allEntries = await _context.CacheEntries
              .Where(c => c.OrganizationId == organizationId)
              .ToListAsync();
          _context.CacheEntries.RemoveRange(allEntries);
          entriesAffected = allEntries.Count;
          break;

        case "clear_expired":
          var expiredEntries = await _context.CacheEntries
              .Where(c => c.OrganizationId == organizationId && c.ExpiresAt < DateTime.UtcNow)
              .ToListAsync();
          _context.CacheEntries.RemoveRange(expiredEntries);
          entriesAffected = expiredEntries.Count;
          break;

        case "clear_by_type":
          if (string.IsNullOrEmpty(request.CacheType))
            throw new ArgumentException("Cache type is required for clear_by_type action");

          var typeEntries = await _context.CacheEntries
              .Where(c => c.OrganizationId == organizationId && c.CacheType == request.CacheType)
              .ToListAsync();
          _context.CacheEntries.RemoveRange(typeEntries);
          entriesAffected = typeEntries.Count;
          break;

        default:
          throw new ArgumentException("Invalid cache management action");
      }

      await _context.SaveChangesAsync();

      return new CacheManagementResponse
      {
        Action = request.Action,
        EntriesAffected = entriesAffected,
        ExecutedAt = DateTime.UtcNow,
        IsSuccessful = true,
        Message = $"Successfully {request.Action} - {entriesAffected} entries affected"
      };
    }

    // Performance metrics
    public async Task<List<PerformanceMetricResponse>> GetPerformanceMetricsAsync(int organizationId, string? category = null)
    {
      var query = _context.PerformanceMetrics.Where(m => m.OrganizationId == organizationId);

      if (!string.IsNullOrEmpty(category))
        query = query.Where(m => m.Category == category);

      return await query
          .Select(m => new PerformanceMetricResponse
          {
            Id = m.Id,
            MetricName = m.MetricName,
            Category = m.Category,
            Value = m.Value,
            Unit = m.Unit,
            RecordedAt = m.RecordedAt,
            OrganizationId = m.OrganizationId,
            Context = m.Context,
            Tags = m.Tags
          })
          .ToListAsync();
    }

    public async Task<PerformanceMetricResponse?> GetPerformanceMetricAsync(int id)
    {
      var metric = await _context.PerformanceMetrics.FindAsync(id);
      if (metric == null) return null;

      return new PerformanceMetricResponse
      {
        Id = metric.Id,
        MetricName = metric.MetricName,
        Category = metric.Category,
        Value = metric.Value,
        Unit = metric.Unit,
        RecordedAt = metric.RecordedAt,
        OrganizationId = metric.OrganizationId,
        Context = metric.Context,
        Tags = metric.Tags
      };
    }

    public async Task<PerformanceMetricResponse> CreatePerformanceMetricAsync(CreatePerformanceMetricRequest request, int organizationId)
    {
      var metric = new PerformanceMetric
      {
        MetricName = request.MetricName,
        Category = request.Category,
        Value = request.Value,
        Unit = request.Unit,
        Context = request.Context,
        Tags = request.Tags ?? "[]",
        OrganizationId = organizationId,
        RecordedAt = DateTime.UtcNow
      };

      _context.PerformanceMetrics.Add(metric);
      await _context.SaveChangesAsync();

      return await GetPerformanceMetricAsync(metric.Id) ?? throw new InvalidOperationException("Failed to create performance metric");
    }

    public async Task DeletePerformanceMetricAsync(int id)
    {
      var metric = await _context.PerformanceMetrics.FindAsync(id);
      if (metric == null)
        throw new ArgumentException("Performance metric not found");

      _context.PerformanceMetrics.Remove(metric);
      await _context.SaveChangesAsync();
    }

    // Database optimization
    public async Task<List<DatabaseOptimizationResponse>> GetDatabaseOptimizationsAsync(int organizationId)
    {
      return await _context.DatabaseOptimizations
          .Where(d => d.OrganizationId == organizationId)
          .Select(d => new DatabaseOptimizationResponse
          {
            Id = d.Id,
            TableName = d.TableName,
            OptimizationType = d.OptimizationType,
            Query = d.Query,
            ExecutionTime = d.ExecutionTime,
            ImprovementPercentage = d.ImprovementPercentage,
            ExecutedAt = d.ExecutedAt,
            IsSuccessful = d.IsSuccessful,
            Notes = d.Notes,
            OrganizationId = d.OrganizationId
          })
          .ToListAsync();
    }

    public async Task<DatabaseOptimizationResponse?> GetDatabaseOptimizationAsync(int id)
    {
      var optimization = await _context.DatabaseOptimizations.FindAsync(id);
      if (optimization == null) return null;

      return new DatabaseOptimizationResponse
      {
        Id = optimization.Id,
        TableName = optimization.TableName,
        OptimizationType = optimization.OptimizationType,
        Query = optimization.Query,
        ExecutionTime = optimization.ExecutionTime,
        ImprovementPercentage = optimization.ImprovementPercentage,
        ExecutedAt = optimization.ExecutedAt,
        IsSuccessful = optimization.IsSuccessful,
        Notes = optimization.Notes,
        OrganizationId = optimization.OrganizationId
      };
    }

    public async Task<DatabaseOptimizationResponse> CreateDatabaseOptimizationAsync(CreateDatabaseOptimizationRequest request, int organizationId)
    {
      var optimization = new DatabaseOptimization
      {
        TableName = request.TableName,
        OptimizationType = request.OptimizationType,
        Query = request.Query,
        Notes = request.Notes,
        OrganizationId = organizationId,
        ExecutedAt = DateTime.UtcNow,
        IsSuccessful = false,
        ExecutionTime = 0,
        ImprovementPercentage = 0
      };

      _context.DatabaseOptimizations.Add(optimization);
      await _context.SaveChangesAsync();

      return await GetDatabaseOptimizationAsync(optimization.Id) ?? throw new InvalidOperationException("Failed to create database optimization");
    }

    public async Task DeleteDatabaseOptimizationAsync(int id)
    {
      var optimization = await _context.DatabaseOptimizations.FindAsync(id);
      if (optimization == null)
        throw new ArgumentException("Database optimization not found");

      _context.DatabaseOptimizations.Remove(optimization);
      await _context.SaveChangesAsync();
    }

    public async Task<DatabaseOptimizationResponse> ExecuteOptimizationAsync(int id)
    {
      var optimization = await _context.DatabaseOptimizations.FindAsync(id);
      if (optimization == null)
        throw new ArgumentException("Database optimization not found");

      // Simulate optimization execution
      var startTime = DateTime.UtcNow;
      await Task.Delay(100); // Simulate execution time
      var executionTime = (decimal)(DateTime.UtcNow - startTime).TotalMilliseconds;

      optimization.ExecutionTime = executionTime;
      optimization.ImprovementPercentage = 15.5m; // Simulated improvement
      optimization.IsSuccessful = true;
      optimization.ExecutedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      return await GetDatabaseOptimizationAsync(id) ?? throw new InvalidOperationException("Failed to execute optimization");
    }

    // API performance logging
    public async Task<List<ApiPerformanceLogResponse>> GetApiPerformanceLogsAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null)
    {
      var query = _context.ApiPerformanceLogs
          .Where(a => a.OrganizationId == organizationId)
          .Include(a => a.User);

      if (startDate.HasValue)
        query = query.Where(a => a.RequestedAt >= startDate.Value);

      if (endDate.HasValue)
        query = query.Where(a => a.RequestedAt <= endDate.Value);

      return await query
          .Select(a => new ApiPerformanceLogResponse
          {
            Id = a.Id,
            HttpMethod = a.HttpMethod,
            Endpoint = a.Endpoint,
            StatusCode = a.StatusCode,
            ResponseTime = a.ResponseTime,
            RequestedAt = a.RequestedAt,
            UserId = a.UserId,
            UserName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : null,
            OrganizationId = a.OrganizationId,
            UserAgent = a.UserAgent,
            IpAddress = a.IpAddress,
            RequestBody = a.RequestBody,
            ResponseBody = a.ResponseBody,
            RequestSize = a.RequestSize,
            ResponseSize = a.ResponseSize
          })
          .ToListAsync();
    }

    public async Task<ApiPerformanceLogResponse?> GetApiPerformanceLogAsync(int id)
    {
      var log = await _context.ApiPerformanceLogs
          .Include(a => a.User)
          .FirstOrDefaultAsync(a => a.Id == id);

      if (log == null) return null;

      return new ApiPerformanceLogResponse
      {
        Id = log.Id,
        HttpMethod = log.HttpMethod,
        Endpoint = log.Endpoint,
        StatusCode = log.StatusCode,
        ResponseTime = log.ResponseTime,
        RequestedAt = log.RequestedAt,
        UserId = log.UserId,
        UserName = log.User != null ? $"{log.User.FirstName} {log.User.LastName}" : null,
        OrganizationId = log.OrganizationId,
        UserAgent = log.UserAgent,
        IpAddress = log.IpAddress,
        RequestBody = log.RequestBody,
        ResponseBody = log.ResponseBody,
        RequestSize = log.RequestSize,
        ResponseSize = log.ResponseSize
      };
    }

    public async Task<ApiPerformanceLogResponse> CreateApiPerformanceLogAsync(ApiPerformanceLog log)
    {
      _context.ApiPerformanceLogs.Add(log);
      await _context.SaveChangesAsync();

      return await GetApiPerformanceLogAsync(log.Id) ?? throw new InvalidOperationException("Failed to create API performance log");
    }

    // Memory usage
    public async Task<List<MemoryUsageResponse>> GetMemoryUsageAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null)
    {
      var query = _context.MemoryUsages.Where(m => m.OrganizationId == organizationId);

      if (startDate.HasValue)
        query = query.Where(m => m.RecordedAt >= startDate.Value);

      if (endDate.HasValue)
        query = query.Where(m => m.RecordedAt <= endDate.Value);

      return await query
          .Select(m => new MemoryUsageResponse
          {
            Id = m.Id,
            TotalMemory = m.TotalMemory,
            UsedMemory = m.UsedMemory,
            AvailableMemory = m.AvailableMemory,
            MemoryUsagePercentage = m.MemoryUsagePercentage,
            RecordedAt = m.RecordedAt,
            OrganizationId = m.OrganizationId,
            ServerInstance = m.ServerInstance
          })
          .ToListAsync();
    }

    public async Task<MemoryUsageResponse?> GetMemoryUsageAsync(int id)
    {
      var usage = await _context.MemoryUsages.FindAsync(id);
      if (usage == null) return null;

      return new MemoryUsageResponse
      {
        Id = usage.Id,
        TotalMemory = usage.TotalMemory,
        UsedMemory = usage.UsedMemory,
        AvailableMemory = usage.AvailableMemory,
        MemoryUsagePercentage = usage.MemoryUsagePercentage,
        RecordedAt = usage.RecordedAt,
        OrganizationId = usage.OrganizationId,
        ServerInstance = usage.ServerInstance
      };
    }

    public async Task<MemoryUsageResponse> CreateMemoryUsageAsync(CreateMemoryUsageRequest request, int organizationId)
    {
      var usage = new MemoryUsage
      {
        TotalMemory = request.TotalMemory,
        UsedMemory = request.UsedMemory,
        AvailableMemory = request.AvailableMemory,
        MemoryUsagePercentage = request.TotalMemory > 0 ? (request.UsedMemory / request.TotalMemory) * 100 : 0,
        ServerInstance = request.ServerInstance,
        OrganizationId = organizationId,
        RecordedAt = DateTime.UtcNow
      };

      _context.MemoryUsages.Add(usage);
      await _context.SaveChangesAsync();

      return await GetMemoryUsageAsync(usage.Id) ?? throw new InvalidOperationException("Failed to create memory usage");
    }

    public async Task DeleteMemoryUsageAsync(int id)
    {
      var usage = await _context.MemoryUsages.FindAsync(id);
      if (usage == null)
        throw new ArgumentException("Memory usage not found");

      _context.MemoryUsages.Remove(usage);
      await _context.SaveChangesAsync();
    }

    // CPU usage
    public async Task<List<CpuUsageResponse>> GetCpuUsageAsync(int organizationId, DateTime? startDate = null, DateTime? endDate = null)
    {
      var query = _context.CpuUsages.Where(c => c.OrganizationId == organizationId);

      if (startDate.HasValue)
        query = query.Where(c => c.RecordedAt >= startDate.Value);

      if (endDate.HasValue)
        query = query.Where(c => c.RecordedAt <= endDate.Value);

      return await query
          .Select(c => new CpuUsageResponse
          {
            Id = c.Id,
            CpuUsagePercentage = c.CpuUsagePercentage,
            LoadAverage = c.LoadAverage,
            ActiveThreads = c.ActiveThreads,
            RecordedAt = c.RecordedAt,
            OrganizationId = c.OrganizationId,
            ServerInstance = c.ServerInstance
          })
          .ToListAsync();
    }

    public async Task<CpuUsageResponse?> GetCpuUsageAsync(int id)
    {
      var usage = await _context.CpuUsages.FindAsync(id);
      if (usage == null) return null;

      return new CpuUsageResponse
      {
        Id = usage.Id,
        CpuUsagePercentage = usage.CpuUsagePercentage,
        LoadAverage = usage.LoadAverage,
        ActiveThreads = usage.ActiveThreads,
        RecordedAt = usage.RecordedAt,
        OrganizationId = usage.OrganizationId,
        ServerInstance = usage.ServerInstance
      };
    }

    public async Task<CpuUsageResponse> CreateCpuUsageAsync(CreateCpuUsageRequest request, int organizationId)
    {
      var usage = new CpuUsage
      {
        CpuUsagePercentage = request.CpuUsagePercentage,
        LoadAverage = request.LoadAverage,
        ActiveThreads = request.ActiveThreads,
        ServerInstance = request.ServerInstance,
        OrganizationId = organizationId,
        RecordedAt = DateTime.UtcNow
      };

      _context.CpuUsages.Add(usage);
      await _context.SaveChangesAsync();

      return await GetCpuUsageAsync(usage.Id) ?? throw new InvalidOperationException("Failed to create CPU usage");
    }

    public async Task DeleteCpuUsageAsync(int id)
    {
      var usage = await _context.CpuUsages.FindAsync(id);
      if (usage == null)
        throw new ArgumentException("CPU usage not found");

      _context.CpuUsages.Remove(usage);
      await _context.SaveChangesAsync();
    }

    // Summary and charts
    public async Task<PerformanceSummaryResponse> GetPerformanceSummaryAsync(int organizationId)
    {
      var cacheEntries = await _context.CacheEntries.Where(c => c.OrganizationId == organizationId).ToListAsync();
      var metrics = await _context.PerformanceMetrics.Where(m => m.OrganizationId == organizationId).ToListAsync();
      var optimizations = await _context.DatabaseOptimizations.Where(d => d.OrganizationId == organizationId).ToListAsync();
      var apiLogs = await _context.ApiPerformanceLogs.Where(a => a.OrganizationId == organizationId).ToListAsync();
      var memoryUsage = await _context.MemoryUsages.Where(m => m.OrganizationId == organizationId).ToListAsync();
      var cpuUsage = await _context.CpuUsages.Where(c => c.OrganizationId == organizationId).ToListAsync();

      return new PerformanceSummaryResponse
      {
        CacheSummary = new CacheSummary
        {
          TotalEntries = cacheEntries.Count,
          ExpiredEntries = cacheEntries.Count(c => c.ExpiresAt < DateTime.UtcNow),
          HitRate = cacheEntries.Any() ? cacheEntries.Average(c => c.AccessCount) / 100 : 0,
          AverageAccessCount = cacheEntries.Any() ? cacheEntries.Average(c => c.AccessCount) : 0,
          CacheTypes = cacheEntries.Select(c => c.CacheType).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList()
        },
        MetricsSummary = new MetricsSummary
        {
          TotalMetrics = metrics.Count,
          MetricsToday = metrics.Count(m => m.RecordedAt.Date == DateTime.UtcNow.Date),
          Categories = metrics.Select(m => m.Category).Distinct().ToList(),
          AverageValue = metrics.Any() ? metrics.Average(m => m.Value) : 0
        },
        DatabaseSummary = new DatabaseSummary
        {
          TotalOptimizations = optimizations.Count,
          SuccessfulOptimizations = optimizations.Count(o => o.IsSuccessful),
          AverageImprovement = optimizations.Any() ? optimizations.Average(o => o.ImprovementPercentage) : 0,
          AverageExecutionTime = optimizations.Any() ? optimizations.Average(o => o.ExecutionTime) : 0,
          TableNames = optimizations.Select(o => o.TableName).Distinct().ToList()
        },
        ApiSummary = new ApiSummary
        {
          TotalRequests = apiLogs.Count,
          RequestsToday = apiLogs.Count(a => a.RequestedAt.Date == DateTime.UtcNow.Date),
          AverageResponseTime = apiLogs.Any() ? apiLogs.Average(a => a.ResponseTime) : 0,
          SuccessRate = apiLogs.Any() ? (decimal)apiLogs.Count(a => a.StatusCode < 400) / apiLogs.Count * 100 : 0,
          TopEndpoints = apiLogs.GroupBy(a => a.Endpoint)
                  .OrderByDescending(g => g.Count())
                  .Take(5)
                  .Select(g => g.Key)
                  .ToList()
        },
        SystemSummary = new SystemSummary
        {
          AverageMemoryUsage = memoryUsage.Any() ? memoryUsage.Average(m => m.MemoryUsagePercentage) : 0,
          AverageCpuUsage = cpuUsage.Any() ? cpuUsage.Average(c => c.CpuUsagePercentage) : 0,
          AverageLoadAverage = cpuUsage.Any() ? cpuUsage.Average(c => c.LoadAverage) : 0,
          TotalServerInstances = memoryUsage.Select(m => m.ServerInstance).Distinct().Count(),
          ServerInstances = memoryUsage.Select(m => m.ServerInstance).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList()
        }
      };
    }

    public async Task<PerformanceChartDataResponse> GetPerformanceChartDataAsync(int organizationId, string chartType, DateTime startDate, DateTime endDate)
    {
      var labels = new List<string>();
      var datasets = new List<PerformanceChartDataset>();

      switch (chartType.ToLower())
      {
        case "memory":
          var memoryData = await _context.MemoryUsages
              .Where(m => m.OrganizationId == organizationId && m.RecordedAt >= startDate && m.RecordedAt <= endDate)
              .OrderBy(m => m.RecordedAt)
              .ToListAsync();

          labels = memoryData.Select(m => m.RecordedAt.ToString("HH:mm")).ToList();
          datasets.Add(new PerformanceChartDataset
          {
            Label = "Memory Usage (%)",
            Data = memoryData.Select(m => m.MemoryUsagePercentage).ToList(),
            BackgroundColor = "rgba(54, 162, 235, 0.2)",
            BorderColor = "rgba(54, 162, 235, 1)",
            BorderWidth = 2,
            Fill = true
          });
          break;

        case "cpu":
          var cpuData = await _context.CpuUsages
              .Where(c => c.OrganizationId == organizationId && c.RecordedAt >= startDate && c.RecordedAt <= endDate)
              .OrderBy(c => c.RecordedAt)
              .ToListAsync();

          labels = cpuData.Select(c => c.RecordedAt.ToString("HH:mm")).ToList();
          datasets.Add(new PerformanceChartDataset
          {
            Label = "CPU Usage (%)",
            Data = cpuData.Select(c => c.CpuUsagePercentage).ToList(),
            BackgroundColor = "rgba(255, 99, 132, 0.2)",
            BorderColor = "rgba(255, 99, 132, 1)",
            BorderWidth = 2,
            Fill = true
          });
          break;

        case "api_response_time":
          var apiData = await _context.ApiPerformanceLogs
              .Where(a => a.OrganizationId == organizationId && a.RequestedAt >= startDate && a.RequestedAt <= endDate)
              .OrderBy(a => a.RequestedAt)
              .ToListAsync();

          labels = apiData.Select(a => a.RequestedAt.ToString("HH:mm")).ToList();
          datasets.Add(new PerformanceChartDataset
          {
            Label = "Response Time (ms)",
            Data = apiData.Select(a => a.ResponseTime).ToList(),
            BackgroundColor = "rgba(75, 192, 192, 0.2)",
            BorderColor = "rgba(75, 192, 192, 1)",
            BorderWidth = 2,
            Fill = false
          });
          break;
      }

      return new PerformanceChartDataResponse
      {
        ChartType = "line",
        Labels = labels,
        Datasets = datasets,
        Options = new PerformanceChartOptions
        {
          Title = $"{chartType.Replace("_", " ").ToUpper()} Performance",
          XAxisLabel = "Time",
          YAxisLabel = chartType.ToLower() switch
          {
            "memory" => "Memory Usage (%)",
            "cpu" => "CPU Usage (%)",
            "api_response_time" => "Response Time (ms)",
            _ => "Value"
          }
        }
      };
    }

    // Performance alerts (placeholder implementation)
    public async Task<List<PerformanceAlertResponse>> GetPerformanceAlertsAsync(int organizationId)
    {
      // Placeholder implementation
      return new List<PerformanceAlertResponse>();
    }

    public async Task<PerformanceAlertResponse?> GetPerformanceAlertAsync(int id)
    {
      // Placeholder implementation
      return null;
    }

    public async Task<PerformanceAlertResponse> CreatePerformanceAlertAsync(CreatePerformanceAlertRequest request, int organizationId)
    {
      // Placeholder implementation
      return new PerformanceAlertResponse
      {
        Id = 1,
        AlertType = request.AlertType,
        MetricName = request.MetricName,
        Threshold = request.Threshold,
        CurrentValue = 0,
        Severity = request.Severity,
        TriggeredAt = DateTime.UtcNow,
        IsResolved = false,
        OrganizationId = organizationId,
        Message = request.Message
      };
    }

    public async Task<PerformanceAlertResponse> ResolveAlertAsync(int id)
    {
      // Placeholder implementation
      return new PerformanceAlertResponse
      {
        Id = id,
        AlertType = "memory",
        MetricName = "Memory Usage",
        Threshold = 80,
        CurrentValue = 75,
        Severity = "medium",
        TriggeredAt = DateTime.UtcNow.AddHours(-1),
        IsResolved = true,
        ResolvedAt = DateTime.UtcNow,
        OrganizationId = 1,
        Message = "Alert resolved"
      };
    }
  }
}
