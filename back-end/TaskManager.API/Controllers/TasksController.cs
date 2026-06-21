using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Models.DTOs;
using TaskManager.API.Services.Contracts;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    private int CurrentUserId => HttpContext.Items.TryGetValue("UserId", out var value) && value is int id
        ? id
        : throw new InvalidOperationException("UserId not set by authentication middleware.");

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskFilterParams filters)
    {
        var tasks = await taskService.GetAllAsync(CurrentUserId, filters);
        return Ok(ApiResponse<IEnumerable<TaskDto>>.Ok(tasks));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await taskService.GetByIdAsync(id, CurrentUserId);
        if (task is null)
            return NotFound(ApiResponse<TaskDto>.Fail($"Task {id} not found."));

        return Ok(ApiResponse<TaskDto>.Ok(task));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var created = await taskService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<TaskDto>.Ok(created, "Task created."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var updated = await taskService.UpdateAsync(id, CurrentUserId, dto);
        if (updated is null)
            return NotFound(ApiResponse<TaskDto>.Fail($"Task {id} not found."));

        return Ok(ApiResponse<TaskDto>.Ok(updated, "Task updated."));
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> ToggleComplete(int id)
    {
        var updated = await taskService.ToggleCompleteAsync(id, CurrentUserId);
        if (updated is null)
            return NotFound(ApiResponse<TaskDto>.Fail($"Task {id} not found."));

        return Ok(ApiResponse<TaskDto>.Ok(updated, "Task completion toggled."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await taskService.DeleteAsync(id, CurrentUserId);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Task {id} not found."));

        return Ok(ApiResponse<string>.Ok(string.Empty, "Task deleted."));
    }
}
