using TaskManager.API.Models.DTOs;
using TaskManager.API.Models.Entities;
using TaskManager.API.Repositories.Contracts;
using TaskManager.API.Services.Contracts;

namespace TaskManager.API.Services;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<IEnumerable<TaskDto>> GetAllAsync(int userId, TaskFilterParams filters, CancellationToken cancellationToken = default)
    {
        var tasks = await taskRepository.GetAllAsync(userId, filters, cancellationToken);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, userId, cancellationToken);
        return task is null ? null : MapToDto(task);
    }

    public async Task<TaskDto> CreateAsync(int userId, CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var task = new TaskItem
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await taskRepository.CreateAsync(task, cancellationToken);
        return MapToDto(created);
    }

    public async Task<TaskDto?> UpdateAsync(int id, int userId, UpdateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, userId, cancellationToken);
        if (task is null) return null;

        task.Title = dto.Title.Trim();
        task.Description = dto.Description?.Trim();
        task.IsCompleted = dto.IsCompleted;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        var updated = await taskRepository.UpdateAsync(task, cancellationToken);
        return MapToDto(updated);
    }

    public async Task<TaskDto?> ToggleCompleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, userId, cancellationToken);
        if (task is null) return null;

        task.IsCompleted = !task.IsCompleted;
        task.UpdatedAt = DateTime.UtcNow;

        var updated = await taskRepository.UpdateAsync(task, cancellationToken);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, userId, cancellationToken);
        if (task is null) return false;

        await taskRepository.DeleteAsync(task, cancellationToken);
        return true;
    }

    private static TaskDto MapToDto(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        IsCompleted = task.IsCompleted,
        Priority = task.Priority,
        DueDate = task.DueDate,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
    };
}
