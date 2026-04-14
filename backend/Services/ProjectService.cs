using Task = System.Threading.Tasks.Task;
using TaskFlow_API.DTOs;
using TaskFlow_API.Models;
using TaskFlow_API.Repositories;

namespace TaskFlow_API.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsForUserAsync(Guid userId)
    {
        var projects = await _projectRepository.GetProjectsForUserAsync(userId);
        return projects.Select(MapProjectDto);
    }

    public async Task<ProjectDto> CreateProjectAsync(Guid userId, CreateProjectRequest request)
    {
        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _projectRepository.AddAsync(project);
        var owner = await _userRepository.GetByIdAsync(userId);
        project.Owner = owner;

        return MapProjectDto(project);
    }

    public async Task<ProjectDto> GetProjectByIdAsync(Guid userId, Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            throw new KeyNotFoundException("project not found");

        if (!IsProjectMember(project, userId))
            throw new UnauthorizedAccessException();

        return MapProjectDto(project);
    }

    public async Task<ProjectDto> UpdateProjectAsync(Guid userId, Guid projectId, UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            throw new KeyNotFoundException("project not found");

        if (!IsProjectOwner(project, userId))
            throw new UnauthorizedAccessException();

        project.Name = request.Name?.Trim() ?? project.Name;
        project.Description = request.Description?.Trim();

        await _projectRepository.UpdateAsync(project);
        return MapProjectDto(project);
    }

    public async Task DeleteProjectAsync(Guid userId, Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            throw new KeyNotFoundException("project not found");

        if (!IsProjectOwner(project, userId))
            throw new UnauthorizedAccessException();

        await _projectRepository.DeleteAsync(projectId);
    }

    private static ProjectDto MapProjectDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerName = project.Owner?.Name ?? string.Empty,
            CreatedAt = project.CreatedAt,
            Tasks = project.Tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                ProjectId = t.ProjectId,
                AssigneeId = t.AssigneeId,
                AssigneeName = t.Assignee?.Name,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList()
        };
    }

    private static bool IsProjectOwner(Project project, Guid userId)
    {
        return project.OwnerId == userId;
    }

    private static bool IsProjectMember(Project project, Guid userId)
    {
        return project.OwnerId == userId || project.Tasks.Any(t => t.AssigneeId == userId);
    }
}