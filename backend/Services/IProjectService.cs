using TaskFlow_API.DTOs;

namespace TaskFlow_API.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjectsForUserAsync(Guid userId);
    Task<ProjectDto> CreateProjectAsync(Guid userId, CreateProjectRequest request);
    Task<ProjectDto> GetProjectByIdAsync(Guid userId, Guid projectId);
    Task<ProjectDto> UpdateProjectAsync(Guid userId, Guid projectId, UpdateProjectRequest request);
    Task DeleteProjectAsync(Guid userId, Guid projectId);
}