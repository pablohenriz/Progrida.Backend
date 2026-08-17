using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progrida.Application.Tasks;
using Progrida.Application.Tasks.Commands;
using Progrida.Application.Tasks.Queries;

namespace Progrida.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly GetTasksHandler _getTasks;
    private readonly GetTaskByIdHandler _getTaskById;
    private readonly CreateTaskHandler _createTask;
    private readonly UpdateTaskHandler _updateTask;
    private readonly DeleteTaskHandler _deleteTask;
    private readonly CompleteTaskHandler _completeTask;
    private readonly ReopenTaskHandler _reopenTask;
    private readonly ReorderTasksHandler _reorderTasks;

    public TasksController(
        GetTasksHandler getTasks,
        GetTaskByIdHandler getTaskById,
        CreateTaskHandler createTask,
        UpdateTaskHandler updateTask,
        DeleteTaskHandler deleteTask,
        CompleteTaskHandler completeTask,
        ReopenTaskHandler reopenTask,
        ReorderTasksHandler reorderTasks)
    {
        _getTasks = getTasks;
        _getTaskById = getTaskById;
        _createTask = createTask;
        _updateTask = updateTask;
        _deleteTask = deleteTask;
        _completeTask = completeTask;
        _reopenTask = reopenTask;
        _reorderTasks = reorderTasks;
    }

    /// <summary>GET /api/tasks — sempre retorna as tarefas do usuário autenticado, nunca de todos.</summary>
    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetAll([FromQuery] Guid? sectionId, CancellationToken ct) =>
        Ok(await _getTasks.Handle(sectionId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await _getTaskById.Handle(id, ct));

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(CreateTaskRequest request, CancellationToken ct)
    {
        var result = await _createTask.Handle(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid id, UpdateTaskBody body, CancellationToken ct)
    {
        var request = new UpdateTaskRequest(id, body.Title, body.Description, body.SectionId, body.DueDate);
        return Ok(await _updateTask.Handle(request, ct));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _deleteTask.Handle(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<TaskDto>> Complete(Guid id, CancellationToken ct) =>
        Ok(await _completeTask.Handle(id, ct));

    [HttpPatch("{id:guid}/reopen")]
    public async Task<ActionResult<TaskDto>> Reopen(Guid id, CancellationToken ct) =>
        Ok(await _reopenTask.Handle(id, ct));

    /// <summary>PATCH /api/tasks/reorder — persiste a nova ordem após o drag-and-drop.</summary>
    [HttpPatch("reorder")]
    public async Task<IActionResult> Reorder(ReorderTasksRequest request, CancellationToken ct)
    {
        await _reorderTasks.Handle(request, ct);
        return NoContent();
    }
}

public record UpdateTaskBody(string Title, string? Description, Guid? SectionId, DateTime? DueDate);
