using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Exceptions;
using Progrida.Domain.Interfaces;

namespace Progrida.Application.Tasks.Commands;

/// <summary>Caso de uso: excluir uma tarefa do usuário autenticado.</summary>
public class DeleteTaskHandler
{
    private readonly ITaskItemRepository _tasks;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskHandler(ITaskItemRepository tasks, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _tasks = tasks;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(Guid taskId, CancellationToken ct = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct) ?? throw new NotFoundException("Tarefa", taskId);
        task.EnsureOwnedBy(_currentUser.UserId);

        _tasks.Remove(task);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

/// <summary>Caso de uso: marcar uma tarefa como concluída (Regra 4).</summary>
public class CompleteTaskHandler
{
    private readonly ITaskItemRepository _tasks;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteTaskHandler(ITaskItemRepository tasks, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _tasks = tasks;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(Guid taskId, CancellationToken ct = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct) ?? throw new NotFoundException("Tarefa", taskId);
        task.EnsureOwnedBy(_currentUser.UserId);

        task.Complete();
        await _unitOfWork.SaveChangesAsync(ct);
        return TaskDto.FromEntity(task);
    }
}

/// <summary>Caso de uso: reabrir uma tarefa concluída.</summary>
public class ReopenTaskHandler
{
    private readonly ITaskItemRepository _tasks;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ReopenTaskHandler(ITaskItemRepository tasks, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _tasks = tasks;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(Guid taskId, CancellationToken ct = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct) ?? throw new NotFoundException("Tarefa", taskId);
        task.EnsureOwnedBy(_currentUser.UserId);

        task.Reopen();
        await _unitOfWork.SaveChangesAsync(ct);
        return TaskDto.FromEntity(task);
    }
}
