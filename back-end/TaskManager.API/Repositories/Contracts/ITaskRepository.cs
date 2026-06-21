using TaskManager.API.Models.DTOs;
using TaskManager.API.Models.Entities;

namespace TaskManager.API.Repositories.Contracts;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync(int userId, TaskFilterParams filters);
    Task<TaskItem?> GetByIdAsync(int id, int userId);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}
