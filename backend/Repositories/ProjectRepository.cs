using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using TaskFlow_API.Infrastructure;
using TaskFlow_API.Models;

namespace TaskFlow_API.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskFlowDbContext _context;

    public ProjectRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Assignee)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Projects
            .Where(p => p.OwnerId == ownerId)
            .Include(p => p.Owner)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid userId)
    {
        return await _context.Projects
            .Where(p => p.OwnerId == userId || p.Tasks.Any(t => t.AssigneeId == userId))
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Assignee)
            .ToListAsync();
    }

    public async Task AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await GetByIdAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}