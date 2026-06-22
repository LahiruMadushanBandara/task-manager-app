using System.ComponentModel.DataAnnotations;
using TaskManager.API.Models.Entities;

namespace TaskManager.API.Models.DTOs;

public class UpdateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    [EnumDataType(typeof(TaskPriority), ErrorMessage = "Priority must be 0 (Low), 1 (Medium), or 2 (High).")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }
}
