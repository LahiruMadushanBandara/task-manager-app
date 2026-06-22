using TaskManager.API.Models.DTOs;
using TaskManager.API.Models.Entities;

namespace TaskManager.API.Repositories.Contracts;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync(int userId, TaskFilterParams filters, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default);
}
