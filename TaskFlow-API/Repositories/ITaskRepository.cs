using Task = System.Threading.Tasks.Task;
using TaskFlow_API.Models;
using TaskModel = TaskFlow_API.Models.Task;
using TaskStatus = TaskFlow_API.Models.TaskStatus;

namespace TaskFlow_API.Repositories;

public interface ITaskRepository
{
    Task<TaskModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskModel>> GetByProjectIdAsync(Guid projectId, TaskStatus? status = null, Guid? assigneeId = null);
    Task AddAsync(TaskModel task);
    Task UpdateAsync(TaskModel task);
    Task DeleteAsync(Guid id);
}