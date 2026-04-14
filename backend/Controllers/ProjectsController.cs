using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow_API.DTOs;
using TaskFlow_API.Services;

namespace TaskFlow_API.Controllers;

[ApiController]
[Route("projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var userId = GetCurrentUserId();
        var projects = await _projectService.GetProjectsForUserAsync(userId);
        return Ok(new { projects });
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ErrorResponse { Error = "validation failed", Fields = new Dictionary<string, string> { ["name"] = "is required" } });

        var userId = GetCurrentUserId();
        var project = await _projectService.CreateProjectAsync(userId, request);
        return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var userId = GetCurrentUserId();
        try
        {
            var project = await _projectService.GetProjectByIdAsync(userId, id);
            return Ok(project);
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

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
    {
        if (request.Name is not null && string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ErrorResponse { Error = "validation failed", Fields = new Dictionary<string, string> { ["name"] = "is required" } });

        var userId = GetCurrentUserId();
        try
        {
            var updated = await _projectService.UpdateProjectAsync(userId, id, request);
            return Ok(updated);
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
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var userId = GetCurrentUserId();
        try
        {
            await _projectService.DeleteProjectAsync(userId, id);
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