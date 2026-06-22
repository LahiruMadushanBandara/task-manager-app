using TaskManager.API.Models.DTOs;

namespace TaskManager.API.Services.Contracts;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync(int userId, TaskFilterParams filters, CancellationToken cancellationToken = default);
    Task<TaskDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateAsync(int userId, CreateTaskDto dto, CancellationToken cancellationToken = default);
    Task<TaskDto?> UpdateAsync(int id, int userId, UpdateTaskDto dto, CancellationToken cancellationToken = default);
    Task<TaskDto?> ToggleCompleteAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}
