using Task = System.Threading.Tasks.Task;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TaskFlow_API.Models;
using TaskPriority = TaskFlow_API.Models.TaskPriority;
using TaskStatus = TaskFlow_API.Models.TaskStatus;
using TaskModel = TaskFlow_API.Models.Task;

namespace TaskFlow_API.Infrastructure;

public sealed class DatabaseSeeder
{
    private readonly TaskFlowDbContext _context;

    public DatabaseSeeder(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.Users.AnyAsync(u => u.Email == "test@example.com"))
            return;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123", workFactor: 12),
            CreatedAt = DateTime.UtcNow
        };

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Test Project",
            Description = "A seeded test project.",
            OwnerId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        var task1 = new TaskModel
        {
            Id = Guid.NewGuid(),
            Title = "Backlog task",
            Description = "Seeded TODO task",
            Status = TaskStatus.Todo,
            Priority = TaskPriority.Low,
            ProjectId = project.Id,
            AssigneeId = user.Id,
            CreatedById = user.Id,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var task2 = new TaskModel
        {
            Id = Guid.NewGuid(),
            Title = "In-progress task",
            Description = "Seeded in-progress task",
            Status = TaskStatus.InProgress,
            Priority = TaskPriority.Medium,
            ProjectId = project.Id,
            AssigneeId = user.Id,
            CreatedById = user.Id,
            DueDate = DateTime.UtcNow.AddDays(3),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var task3 = new TaskModel
        {
            Id = Guid.NewGuid(),
            Title = "Done task",
            Description = "Seeded completed task",
            Status = TaskStatus.Done,
            Priority = TaskPriority.High,
            ProjectId = project.Id,
            AssigneeId = null,
            CreatedById = user.Id,
            DueDate = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.Projects.Add(project);
        _context.TaskItems.AddRange(task1, task2, task3);
        await _context.SaveChangesAsync();
    }
}
