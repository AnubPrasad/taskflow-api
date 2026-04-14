using Task = System.Threading.Tasks.Task;
using TaskFlow_API.Models;

namespace TaskFlow_API.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId);
    Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid userId);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Guid id);
}