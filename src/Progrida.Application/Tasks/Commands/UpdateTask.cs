using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Exceptions;
using Progrida.Domain.Interfaces;

namespace Progrida.Application.Tasks.Commands;

public record UpdateTaskRequest(Guid TaskId, string Title, string? Description, Guid? SectionId, DateTime? DueDate);

/// <summary>Caso de uso: atualizar título, descrição, seção e prazo de uma tarefa.</summary>
public class UpdateTaskHandler
{
    private readonly ITaskItemRepository _tasks;
    private readonly ITaskSectionRepository _sections;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskHandler(
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

    public async Task<TaskDto> Handle(UpdateTaskRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        var task = await _tasks.GetByIdAsync(request.TaskId, ct)
            ?? throw new NotFoundException("Tarefa", request.TaskId);

        // Regra 3: ownership sempre verificado no backend, nunca confiando no ID recebido.
        task.EnsureOwnedBy(userId);

        if (request.SectionId is { } sectionId)
        {
            var section = await _sections.GetByIdAsync(sectionId, ct)
                ?? throw new NotFoundException("Seção", sectionId);
            section.EnsureOwnedBy(userId);
        }

        task.SetTitle(request.Title);
        task.SetDescription(request.Description);
        task.SetDueDate(request.DueDate);
        task.MoveToSection(request.SectionId);

        await _unitOfWork.SaveChangesAsync(ct);
        return TaskDto.FromEntity(task);
    }
}
