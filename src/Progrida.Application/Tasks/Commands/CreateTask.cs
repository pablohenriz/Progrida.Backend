using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Entities;
using Progrida.Domain.Exceptions;
using Progrida.Domain.Interfaces;

namespace Progrida.Application.Tasks.Commands;

public record CreateTaskRequest(string Title, string? Description, Guid? SectionId, DateTime? DueDate);

/// <summary>Caso de uso: criar uma tarefa para o usuário autenticado.</summary>
public class CreateTaskHandler
{
    private readonly ITaskItemRepository _tasks;
    private readonly ITaskSectionRepository _sections;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskHandler(
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

    public async Task<TaskDto> Handle(CreateTaskRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        // Regra 5: a seção informada precisa pertencer ao mesmo usuário.
        if (request.SectionId is { } sectionId)
        {
            var section = await _sections.GetByIdAsync(sectionId, ct)
                ?? throw new NotFoundException("Seção", sectionId);
            section.EnsureOwnedBy(userId);
        }

        var existing = await _tasks.GetByUserIdAsync(userId, request.SectionId, ct);
        var nextPosition = existing.Count == 0 ? 0 : existing.Max(t => t.Position) + 1;

        var task = TaskItem.Create(userId, request.Title, request.Description, request.SectionId, nextPosition, request.DueDate);

        await _tasks.AddAsync(task, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return TaskDto.FromEntity(task);
    }
}
