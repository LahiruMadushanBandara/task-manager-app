using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Models.DTOs;
using TaskManager.API.Services.Contracts;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController(ITaskService taskService) : ControllerBase
{
    private int CurrentUserId => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)
        ? id
        : throw new InvalidOperationException("UserId claim not present on the authenticated principal.");

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskFilterParams filters, CancellationToken cancellationToken)
    {
        var tasks = await taskService.GetAllAsync(CurrentUserId, filters, cancellationToken);
        return Ok(ApiResponse<IEnumerable<TaskDto>>.Ok(tasks));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var task = await taskService.GetByIdAsync(id, CurrentUserId, cancellationToken);
        if (task is null)
            return NotFound(ApiResponse<TaskDto>.Fail($"Task {id} not found."));

        return Ok(ApiResponse<TaskDto>.Ok(task));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken cancellationToken)
    {
        var created = await taskService.CreateAsync(CurrentUserId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<TaskDto>.Ok(created, "Task created."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto, CancellationToken cancellationToken)
    {
        var updated = await taskService.UpdateAsync(id, CurrentUserId, dto, cancellationToken);
        if (updated is null)
            return NotFound(ApiResponse<TaskDto>.Fail($"Task {id} not found."));

        return Ok(ApiResponse<TaskDto>.Ok(updated, "Task updated."));
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> ToggleComplete(int id, CancellationToken cancellationToken)
    {
        var updated = await taskService.ToggleCompleteAsync(id, CurrentUserId, cancellationToken);
        if (updated is null)
            return NotFound(ApiResponse<TaskDto>.Fail($"Task {id} not found."));

        return Ok(ApiResponse<TaskDto>.Ok(updated, "Task completion toggled."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await taskService.DeleteAsync(id, CurrentUserId, cancellationToken);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Task {id} not found."));

        return NoContent();
    }
}
