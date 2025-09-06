using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Axion.Shared.DTOs;

namespace Axion.ApiGateway.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class GatewayController : ControllerBase
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GatewayController> _logger;

    public GatewayController(HttpClient httpClient, IConfiguration configuration, ILogger<GatewayController> logger)
    {
      _httpClient = httpClient;
      _configuration = configuration;
      _logger = logger;
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> GetUsers()
    {
      try
      {
        var userServiceUrl = _configuration["Services:UserService"];
        var response = await _httpClient.GetAsync($"{userServiceUrl}/api/users");

        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<List<UserResponse>>>(content);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<List<UserResponse>>
        {
          Success = false,
          Message = "Failed to fetch users"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching users");
        return StatusCode(500, new ApiResponse<List<UserResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUser(int id)
    {
      try
      {
        var userServiceUrl = _configuration["Services:UserService"];
        var response = await _httpClient.GetAsync($"{userServiceUrl}/api/users/{id}");

        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<UserResponse>>(content);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<UserResponse>
        {
          Success = false,
          Message = "Failed to fetch user"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching user {UserId}", id);
        return StatusCode(500, new ApiResponse<UserResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost("users")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser(CreateUserRequest request)
    {
      try
      {
        var userServiceUrl = _configuration["Services:UserService"];
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{userServiceUrl}/api/users", content);

        if (response.IsSuccessStatusCode)
        {
          var responseContent = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<UserResponse>>(responseContent);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<UserResponse>
        {
          Success = false,
          Message = "Failed to create user"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating user");
        return StatusCode(500, new ApiResponse<UserResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("projects")]
    public async Task<ActionResult<ApiResponse<List<ProjectResponse>>>> GetProjects()
    {
      try
      {
        var projectServiceUrl = _configuration["Services:ProjectService"];
        var response = await _httpClient.GetAsync($"{projectServiceUrl}/api/projects");

        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<List<ProjectResponse>>>(content);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<List<ProjectResponse>>
        {
          Success = false,
          Message = "Failed to fetch projects"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching projects");
        return StatusCode(500, new ApiResponse<List<ProjectResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("projects/{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> GetProject(int id)
    {
      try
      {
        var projectServiceUrl = _configuration["Services:ProjectService"];
        var response = await _httpClient.GetAsync($"{projectServiceUrl}/api/projects/{id}");

        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<ProjectResponse>>(content);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<ProjectResponse>
        {
          Success = false,
          Message = "Failed to fetch project"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching project {ProjectId}", id);
        return StatusCode(500, new ApiResponse<ProjectResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost("projects")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> CreateProject(CreateProjectRequest request)
    {
      try
      {
        var projectServiceUrl = _configuration["Services:ProjectService"];
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{projectServiceUrl}/api/projects", content);

        if (response.IsSuccessStatusCode)
        {
          var responseContent = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<ProjectResponse>>(responseContent);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<ProjectResponse>
        {
          Success = false,
          Message = "Failed to create project"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating project");
        return StatusCode(500, new ApiResponse<ProjectResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("tasks")]
    public async Task<ActionResult<ApiResponse<List<TaskResponse>>>> GetTasks()
    {
      try
      {
        var taskServiceUrl = _configuration["Services:TaskService"];
        var response = await _httpClient.GetAsync($"{taskServiceUrl}/api/tasks");

        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<List<TaskResponse>>>(content);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<List<TaskResponse>>
        {
          Success = false,
          Message = "Failed to fetch tasks"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching tasks");
        return StatusCode(500, new ApiResponse<List<TaskResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("tasks/{id}")]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> GetTask(int id)
    {
      try
      {
        var taskServiceUrl = _configuration["Services:TaskService"];
        var response = await _httpClient.GetAsync($"{taskServiceUrl}/api/tasks/{id}");

        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<TaskResponse>>(content);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<TaskResponse>
        {
          Success = false,
          Message = "Failed to fetch task"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching task {TaskId}", id);
        return StatusCode(500, new ApiResponse<TaskResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost("tasks")]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> CreateTask(CreateTaskRequest request)
    {
      try
      {
        var taskServiceUrl = _configuration["Services:TaskService"];
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{taskServiceUrl}/api/tasks", content);

        if (response.IsSuccessStatusCode)
        {
          var responseContent = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<TaskResponse>>(responseContent);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<TaskResponse>
        {
          Success = false,
          Message = "Failed to create task"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating task");
        return StatusCode(500, new ApiResponse<TaskResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("notifications")]
    public async Task<ActionResult<ApiResponse<List<NotificationResponse>>>> GetNotifications()
    {
      try
      {
        var notificationServiceUrl = _configuration["Services:NotificationService"];
        var response = await _httpClient.GetAsync($"{notificationServiceUrl}/api/notifications");

        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<List<NotificationResponse>>>(content);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<List<NotificationResponse>>
        {
          Success = false,
          Message = "Failed to fetch notifications"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching notifications");
        return StatusCode(500, new ApiResponse<List<NotificationResponse>>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpGet("notifications/unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadNotificationCount()
    {
      try
      {
        var notificationServiceUrl = _configuration["Services:NotificationService"];
        var response = await _httpClient.GetAsync($"{notificationServiceUrl}/api/notifications/unread-count");

        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<int>>(content);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<int>
        {
          Success = false,
          Message = "Failed to fetch unread notification count"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error fetching unread notification count");
        return StatusCode(500, new ApiResponse<int>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }

    [HttpPost("notifications")]
    public async Task<ActionResult<ApiResponse<NotificationResponse>>> CreateNotification(CreateNotificationRequest request)
    {
      try
      {
        var notificationServiceUrl = _configuration["Services:NotificationService"];
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{notificationServiceUrl}/api/notifications", content);

        if (response.IsSuccessStatusCode)
        {
          var responseContent = await response.Content.ReadAsStringAsync();
          var result = JsonSerializer.Deserialize<ApiResponse<NotificationResponse>>(responseContent);
          return Ok(result);
        }

        return StatusCode((int)response.StatusCode, new ApiResponse<NotificationResponse>
        {
          Success = false,
          Message = "Failed to create notification"
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating notification");
        return StatusCode(500, new ApiResponse<NotificationResponse>
        {
          Success = false,
          Message = ex.Message
        });
      }
    }
  }
}
