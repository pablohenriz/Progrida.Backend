using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Exceptions;
using Progrida.Domain.Interfaces;

namespace Progrida.Application.Tasks.Commands;

/// <summary>Uma entrada do drag-and-drop: qual tarefa foi para qual posição/seção.</summary>
public record ReorderTaskItem(Guid TaskId, int NewPosition, Guid? NewSectionId);

public record ReorderTasksRequest(IReadOnlyList<ReorderTaskItem> Items);

/// <summary>
/// Caso de uso: persistir a nova ordem das tarefas após o usuário arrastar (Regra 6).
/// Sem isso, a ordem se perde ao fechar e reabrir a aplicação (item 10 do documento).
/// </summary>
public class ReorderTasksHandler
{
    private readonly ITaskItemRepository _tasks;
    private readonly ITaskSectionRepository _sections;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderTasksHandler(
        ITaskItemRepository tasks,
        ITaskSectionRepository sections,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _tasks = tasks;
        _sections = sections;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReorderTasksRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        foreach (var item in request.Items)
        {
            var task = await _tasks.GetByIdAsync(item.TaskId, ct)
                ?? throw new NotFoundException("Tarefa", item.TaskId);

            task.EnsureOwnedBy(userId);

            if (item.NewSectionId is { } sectionId)
            {
                var section = await _sections.GetByIdAsync(sectionId, ct)
                    ?? throw new NotFoundException("Seção", sectionId);
                section.EnsureOwnedBy(userId);
            }

            task.MoveToSection(item.NewSectionId);
            task.Reorder(item.NewPosition);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
