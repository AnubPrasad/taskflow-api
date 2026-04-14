using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow_API.DTOs;
using TaskFlow_API.Services;

namespace TaskFlow_API.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request)
    {
        if (request.Title is not null && string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new ErrorResponse { Error = "validation failed", Fields = new Dictionary<string, string> { ["title"] = "is required" } });

        var userId = GetCurrentUserId();
        try
        {
            var task = await _taskService.UpdateTaskAsync(userId, id, request);
            return Ok(task);
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var userId = GetCurrentUserId();
        try
        {
            await _taskService.DeleteTaskAsync(userId, id);
            return NoContent();
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