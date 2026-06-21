using TaskManager.API.Models.Entities;

namespace TaskManager.API.Models.DTOs;

public class TaskFilterParams
{
    public string? Search { get; set; }
    public bool? IsCompleted { get; set; }
    public TaskPriority? Priority { get; set; }

    // Allowed values: title, priority, duedate, createdat
    public string SortBy { get; set; } = "createdat";

    // asc or desc
    public string SortDir { get; set; } = "desc";
}
