using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Exceptions;
using Progrida.Domain.Interfaces;

namespace Progrida.Application.Tasks.Queries;

/// <summary>Caso de uso: listar as tarefas do usuário autenticado (nunca de outro usuário).</summary>
public class GetTasksHandler
{
    private readonly ITaskItemRepository _tasks;
    private readonly ICurrentUserService _currentUser;

    public GetTasksHandler(ITaskItemRepository tasks, ICurrentUserService currentUser)
    {
        _tasks = tasks;
        _currentUser = currentUser;
    }

    public async Task<List<TaskDto>> Handle(Guid? sectionId, CancellationToken ct = default)
    {
        var tasks = await _tasks.GetByUserIdAsync(_currentUser.UserId, sectionId, ct);
        return tasks
            .OrderBy(t => t.Position)
            .Select(TaskDto.FromEntity)
            .ToList();
    }
}

/// <summary>Caso de uso: buscar uma tarefa específica, validando ownership.</summary>
public class GetTaskByIdHandler
{
    private readonly ITaskItemRepository _tasks;
    private readonly ICurrentUserService _currentUser;

    public GetTaskByIdHandler(ITaskItemRepository tasks, ICurrentUserService currentUser)
    {
        _tasks = tasks;
        _currentUser = currentUser;
    }

    public async Task<TaskDto> Handle(Guid taskId, CancellationToken ct = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct) ?? throw new NotFoundException("Tarefa", taskId);
        task.EnsureOwnedBy(_currentUser.UserId);
        return TaskDto.FromEntity(task);
    }
}
