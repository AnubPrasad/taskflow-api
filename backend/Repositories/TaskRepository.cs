using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using TaskFlow_API.Infrastructure;
using TaskFlow_API.Models;
using TaskModel = TaskFlow_API.Models.Task;
using TaskStatus = TaskFlow_API.Models.TaskStatus;

namespace TaskFlow_API.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskFlowDbContext _context;

    public TaskRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<TaskModel?> GetByIdAsync(Guid id)
    {
        return await _context.TaskItems
            .Include(t => t.Assignee)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskModel>> GetByProjectIdAsync(Guid projectId, TaskStatus? status = null, Guid? assigneeId = null)
    {
        var query = _context.TaskItems
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Assignee)
            .Include(t => t.Project)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (assigneeId.HasValue)
        {
            query = query.Where(t => t.AssigneeId == assigneeId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(TaskModel task)
    {
        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskModel task)
    {
        _context.TaskItems.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await GetByIdAsync(id);
        if (task != null)
        {
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}