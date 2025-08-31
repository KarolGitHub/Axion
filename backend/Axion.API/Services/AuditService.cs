using System.Text.Json;
using Axion.API.Data;
using Axion.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Axion.API.Services;

public interface IAuditService
{
  Task LogAsync(
      string userId,
      AuditAction action,
      AuditEntityType entityType,
      string? entityId = null,
      string? entityName = null,
      string? oldValues = null,
      string? newValues = null,
      string? ipAddress = null,
      string? userAgent = null,
      string? sessionId = null,
      string? additionalData = null,
      bool success = true,
      string? errorMessage = null,
      int? responseTimeMs = null);

  Task<AuditLogSearchResponse> SearchAsync(AuditLogSearchRequest request);
  Task<List<AuditLogResponse>> GetUserActivityAsync(string userId, int limit = 50);
  Task<List<AuditLogResponse>> GetEntityHistoryAsync(string entityType, string entityId, int limit = 50);
}

public class AuditService : IAuditService
{
  private readonly AxionDbContext _context;
  private readonly ILogger<AuditService> _logger;

  public AuditService(AxionDbContext context, ILogger<AuditService> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task LogAsync(
      string userId,
      AuditAction action,
      AuditEntityType entityType,
      string? entityId = null,
      string? entityName = null,
      string? oldValues = null,
      string? newValues = null,
      string? ipAddress = null,
      string? userAgent = null,
      string? sessionId = null,
      string? additionalData = null,
      bool success = true,
      string? errorMessage = null,
      int? responseTimeMs = null)
  {
    try
    {
      var user = await _context.Users.FindAsync(userId);

      var auditLog = new AuditLog
      {
        UserId = userId,
        UserEmail = user?.Email,
        UserName = user != null ? $"{user.FirstName} {user.LastName}" : null,
        Action = action.ToString(),
        EntityType = entityType.ToString(),
        EntityId = entityId,
        EntityName = entityName,
        OldValues = oldValues,
        NewValues = newValues,
        IpAddress = ipAddress,
        UserAgent = userAgent,
        SessionId = sessionId,
        AdditionalData = additionalData,
        Success = success,
        ErrorMessage = errorMessage,
        ResponseTimeMs = responseTimeMs,
        OrganizationId = user?.OrganizationId
      };

      _context.AuditLogs.Add(auditLog);
      await _context.SaveChangesAsync();

      // Log to console for development
      if (!success)
      {
        _logger.LogWarning("Audit Log - Failed Action: {Action} by User: {UserId} - Error: {ErrorMessage}",
            action, userId, errorMessage);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to create audit log entry for user {UserId}, action {Action}", userId, action);
    }
  }

  public async Task<AuditLogSearchResponse> SearchAsync(AuditLogSearchRequest request)
  {
    var query = _context.AuditLogs.AsQueryable();

    // Apply filters
    if (!string.IsNullOrEmpty(request.UserId))
      query = query.Where(a => a.UserId == request.UserId);

    if (!string.IsNullOrEmpty(request.Action))
      query = query.Where(a => a.Action == request.Action);

    if (!string.IsNullOrEmpty(request.EntityType))
      query = query.Where(a => a.EntityType == request.EntityType);

    if (!string.IsNullOrEmpty(request.EntityId))
      query = query.Where(a => a.EntityId == request.EntityId);

    if (request.FromDate.HasValue)
      query = query.Where(a => a.Timestamp >= request.FromDate.Value);

    if (request.ToDate.HasValue)
      query = query.Where(a => a.Timestamp <= request.ToDate.Value);

    if (request.Success.HasValue)
      query = query.Where(a => a.Success == request.Success.Value);

    // Get total count
    var totalCount = await query.CountAsync();

    // Apply pagination
    var logs = await query
        .OrderByDescending(a => a.Timestamp)
        .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(a => new AuditLogResponse
        {
          Id = a.Id,
          UserId = a.UserId,
          UserEmail = a.UserEmail,
          UserName = a.UserName,
          Action = a.Action,
          EntityType = a.EntityType,
          EntityId = a.EntityId,
          EntityName = a.EntityName,
          OldValues = a.OldValues,
          NewValues = a.NewValues,
          IpAddress = a.IpAddress,
          UserAgent = a.UserAgent,
          Timestamp = a.Timestamp,
          Success = a.Success,
          ErrorMessage = a.ErrorMessage,
          ResponseTimeMs = a.ResponseTimeMs
        })
        .ToListAsync();

    return new AuditLogSearchResponse
    {
      Logs = logs,
      TotalCount = totalCount,
      Page = request.Page,
      PageSize = request.PageSize,
      TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
    };
  }

  public async Task<List<AuditLogResponse>> GetUserActivityAsync(string userId, int limit = 50)
  {
    var logs = await _context.AuditLogs
        .Where(a => a.UserId == userId)
        .OrderByDescending(a => a.Timestamp)
        .Take(limit)
        .Select(a => new AuditLogResponse
        {
          Id = a.Id,
          UserId = a.UserId,
          UserEmail = a.UserEmail,
          UserName = a.UserName,
          Action = a.Action,
          EntityType = a.EntityType,
          EntityId = a.EntityId,
          EntityName = a.EntityName,
          OldValues = a.OldValues,
          NewValues = a.NewValues,
          IpAddress = a.IpAddress,
          UserAgent = a.UserAgent,
          Timestamp = a.Timestamp,
          Success = a.Success,
          ErrorMessage = a.ErrorMessage,
          ResponseTimeMs = a.ResponseTimeMs
        })
        .ToListAsync();

    return logs;
  }

  public async Task<List<AuditLogResponse>> GetEntityHistoryAsync(string entityType, string entityId, int limit = 50)
  {
    var logs = await _context.AuditLogs
        .Where(a => a.EntityType == entityType && a.EntityId == entityId)
        .OrderByDescending(a => a.Timestamp)
        .Take(limit)
        .Select(a => new AuditLogResponse
        {
          Id = a.Id,
          UserId = a.UserId,
          UserEmail = a.UserEmail,
          UserName = a.UserName,
          Action = a.Action,
          EntityType = a.EntityType,
          EntityId = a.EntityId,
          EntityName = a.EntityName,
          OldValues = a.OldValues,
          NewValues = a.NewValues,
          IpAddress = a.IpAddress,
          UserAgent = a.UserAgent,
          Timestamp = a.Timestamp,
          Success = a.Success,
          ErrorMessage = a.ErrorMessage,
          ResponseTimeMs = a.ResponseTimeMs
        })
        .ToListAsync();

    return logs;
  }
}
