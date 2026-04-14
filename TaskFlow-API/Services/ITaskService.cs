using TaskFlow_API.DTOs;

namespace TaskFlow_API.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetTasksForProjectAsync(Guid userId, Guid projectId, string? status, Guid? assigneeId);
    Task<TaskDto> CreateTaskAsync(Guid userId, Guid projectId, CreateTaskRequest request);
    Task<TaskDto> UpdateTaskAsync(Guid userId, Guid taskId, UpdateTaskRequest request);
    Task DeleteTaskAsync(Guid userId, Guid taskId);
}