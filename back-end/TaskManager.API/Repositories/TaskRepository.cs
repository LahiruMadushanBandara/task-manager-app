using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models.DTOs;
using TaskManager.API.Models.Entities;
using TaskManager.API.Repositories.Contracts;

namespace TaskManager.API.Repositories;

public class TaskRepository(AppDbContext context) : ITaskRepository
{
    public async Task<IEnumerable<TaskItem>> GetAllAsync(int userId, TaskFilterParams filters)
    {
        var query = context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var term = filters.Search.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(term) ||
                                     (t.Description != null && t.Description.ToLower().Contains(term)));
        }

        if (filters.IsCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == filters.IsCompleted.Value);

        if (filters.Priority.HasValue)
            query = query.Where(t => t.Priority == filters.Priority.Value);

        query = (filters.SortBy.ToLower(), filters.SortDir.ToLower()) switch
        {
            ("title",     "asc")  => query.OrderBy(t => t.Title),
            ("title",     _)      => query.OrderByDescending(t => t.Title),
            ("priority",  "asc")  => query.OrderBy(t => t.Priority),
            ("priority",  _)      => query.OrderByDescending(t => t.Priority),
            ("duedate",   "asc")  => query.OrderBy(t => t.DueDate),
            ("duedate",   _)      => query.OrderByDescending(t => t.DueDate),
            (_,           "asc")  => query.OrderBy(t => t.CreatedAt),
            _                     => query.OrderByDescending(t => t.CreatedAt),
        };

        return await query.ToListAsync();
    }

    public Task<TaskItem?> GetByIdAsync(int id, int userId) =>
        context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        context.Tasks.Update(task);
        await context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteAsync(TaskItem task)
    {
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
    }
}
