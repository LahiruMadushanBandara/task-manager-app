using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models.DTOs;
using TaskManager.API.Models.Entities;
using TaskManager.API.Repositories.Contracts;

namespace TaskManager.API.Repositories;

public class TaskRepository(AppDbContext context) : ITaskRepository
{
    public async Task<IEnumerable<TaskItem>> GetAllAsync(int userId, TaskFilterParams filters, CancellationToken cancellationToken = default)
    {
        var query = context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            // Default SQL Server collation is case-insensitive, so no LOWER() needed.
            var term = filters.Search.Trim();
            query = query.Where(t => t.Title.Contains(term) ||
                                     (t.Description != null && t.Description.Contains(term)));
        }

        if (filters.IsCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == filters.IsCompleted.Value);

        if (filters.Priority.HasValue)
            query = query.Where(t => t.Priority == filters.Priority.Value);

        var asc = filters.SortDir.Equals("asc", StringComparison.OrdinalIgnoreCase);

        var ordered = filters.SortBy.ToLower() switch
        {
            "title"    => asc ? query.OrderBy(t => t.Title)     : query.OrderByDescending(t => t.Title),
            "priority" => asc ? query.OrderBy(t => t.Priority)  : query.OrderByDescending(t => t.Priority),
            "duedate"  => asc ? query.OrderBy(t => t.DueDate)   : query.OrderByDescending(t => t.DueDate),
            _          => asc ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
        };

        // Tie-break on a unique key so equal sort values keep a deterministic order.
        return await ordered.ThenByDescending(t => t.Id).ToListAsync(cancellationToken);
    }

    public Task<TaskItem?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default) =>
        context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, cancellationToken);

    public async Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.Tasks.Add(task);
        await context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.Tasks.Update(task);
        await context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.Tasks.Remove(task);
        await context.SaveChangesAsync(cancellationToken);
    }
}
