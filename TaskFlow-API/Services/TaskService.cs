using Task = System.Threading.Tasks.Task;
using TaskFlow_API.DTOs;
using TaskFlow_API.Models;
using TaskFlow_API.Repositories;
using TaskModel = TaskFlow_API.Models.Task;
using TaskStatus = TaskFlow_API.Models.TaskStatus;

namespace TaskFlow_API.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<TaskDto>> GetTasksForProjectAsync(Guid userId, Guid projectId, string? status, Guid? assigneeId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            throw new KeyNotFoundException("project not found");

        if (!IsProjectMember(project, userId))
            throw new UnauthorizedAccessException();

        var statusEnum = ParseStatus(status);
        var tasks = await _taskRepository.GetByProjectIdAsync(projectId, statusEnum, assigneeId);
        return tasks.Select(MapTaskDto);
    }

    public async Task<TaskDto> CreateTaskAsync(Guid userId, Guid projectId, CreateTaskRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            throw new KeyNotFoundException("project not found");

        if (!IsProjectOwner(project, userId))
            throw new UnauthorizedAccessException();

        var task = new TaskModel
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status,
            Priority = request.Priority,
            ProjectId = projectId,
            AssigneeId = request.AssigneeId,
            CreatedById = userId,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(task);
        return MapTaskDto(task);
    }

    public async Task<TaskDto> UpdateTaskAsync(Guid userId, Guid taskId, UpdateTaskRequest request)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("task not found");

        var project = await _projectRepository.GetByIdAsync(task.ProjectId);
        if (project == null)
            throw new KeyNotFoundException("project not found");

        if (!CanUpdateTask(project, task, userId))
            throw new UnauthorizedAccessException();

        task.Title = request.Title?.Trim() ?? task.Title;
        task.Description = request.Description?.Trim() ?? task.Description;
        task.Status = request.Status ?? task.Status;
        task.Priority = request.Priority ?? task.Priority;
        task.AssigneeId = request.AssigneeId;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
        return MapTaskDto(task);
    }

    public async Task DeleteTaskAsync(Guid userId, Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("task not found");

        var project = await _projectRepository.GetByIdAsync(task.ProjectId);
        if (project == null)
            throw new KeyNotFoundException("project not found");

        if (!CanDeleteTask(project, task, userId))
            throw new UnauthorizedAccessException();

        await _taskRepository.DeleteAsync(taskId);
    }

    private static TaskDto MapTaskDto(TaskModel task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            ProjectId = task.ProjectId,
            AssigneeId = task.AssigneeId,
            AssigneeName = task.Assignee?.Name,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    private static TaskStatus? ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return null;

        return status.Trim().ToLowerInvariant() switch
        {
            "todo" => TaskStatus.Todo,
            "in_progress" => TaskStatus.InProgress,
            "done" => TaskStatus.Done,
            _ => throw new ArgumentException("invalid status filter")
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

    private static bool CanUpdateTask(Project project, TaskModel task, Guid userId)
    {
        return project.OwnerId == userId || task.AssigneeId == userId;
    }

    private static bool CanDeleteTask(Project project, TaskModel task, Guid userId)
    {
        return project.OwnerId == userId || task.CreatedById == userId;
    }
}