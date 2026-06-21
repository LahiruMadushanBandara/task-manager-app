using TaskManager.API.Models.DTOs;

namespace TaskManager.API.Services.Contracts;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync(int userId, TaskFilterParams filters);
    Task<TaskDto?> GetByIdAsync(int id, int userId);
    Task<TaskDto> CreateAsync(int userId, CreateTaskDto dto);
    Task<TaskDto?> UpdateAsync(int id, int userId, UpdateTaskDto dto);
    Task<TaskDto?> ToggleCompleteAsync(int id, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}
