using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow_API.DTOs;
using TaskFlow_API.Services;

namespace TaskFlow_API.Controllers;

[ApiController]
[Route("projects/{projectId:guid}/tasks")]
[Authorize]
public class ProjectTasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public ProjectTasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(Guid projectId, [FromQuery] string? status, [FromQuery] Guid? assignee)
    {
        var userId = GetCurrentUserId();
        try
        {
            var tasks = await _taskService.GetTasksForProjectAsync(userId, projectId, status, assignee);
            return Ok(tasks);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ErrorResponse { Error = "not found" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new ErrorResponse { Error = "validation failed", Fields = new Dictionary<string, string> { ["title"] = "is required" } });

        var userId = GetCurrentUserId();
        try
        {
            var task = await _taskService.CreateTaskAsync(userId, projectId, request);
            return CreatedAtAction(nameof(GetTasks), new { projectId }, task);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ErrorResponse { Error = "not found" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst("user_id")?.Value;
        return Guid.Parse(claim ?? throw new InvalidOperationException("Missing user claim"));
    }
}
