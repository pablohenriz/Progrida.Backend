using Progrida.Domain.Enums;
using Progrida.Domain.Exceptions;

namespace Progrida.Domain.Entities;

/// <summary>
/// Regras da Tarefa (seção 9 do documento):
///  1. Título obrigatório.
///  2. Título entre 1 e 200 caracteres.
///  3. Usuário só acessa suas próprias tarefas (EnsureOwnedBy).
///  4. Status: Pending ou Completed, com CompletedAt.
///  5. Pertence opcionalmente a uma seção, que precisa ser do mesmo usuário.
///  6. Possui uma posição (Order) usada para reordenação (drag-and-drop).
/// </summary>
public class TaskItem : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid? SectionId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ProgridaTaskStatus Status { get; private set; } = ProgridaTaskStatus.Pending;
    public int Position { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    protected TaskItem() { } // EF Core

    public static TaskItem Create(
        Guid userId,
        string title,
        string? description,
        Guid? sectionId,
        int position,
        DateTime? dueDate = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Uma tarefa precisa pertencer a um usuário.");

        var task = new TaskItem
        {
            UserId = userId,
            SectionId = sectionId,
            Position = position,
            DueDate = dueDate
        };

        task.SetTitle(title);
        task.SetDescription(description);
        return task;
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("O título da tarefa é obrigatório.");

        if (title.Length > 200)
            throw new DomainException("O título da tarefa deve ter no máximo 200 caracteres.");

        Title = title.Trim();
        Touch();
    }

    public void SetDescription(string? description)
    {
        if (description is not null && description.Length > 2000)
            throw new DomainException("A descrição deve ter no máximo 2000 caracteres.");

        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Touch();
    }

    public void SetDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
        Touch();
    }

    public void MoveToSection(Guid? sectionId)
    {
        SectionId = sectionId;
        Touch();
    }

    /// <summary>Regra 6: atualiza a posição da tarefa (drag-and-drop) — é regra de negócio, não só visual.</summary>
    public void Reorder(int newPosition)
    {
        if (newPosition < 0)
            throw new DomainException("A posição da tarefa não pode ser negativa.");

        Position = newPosition;
        Touch();
    }

    public void Complete()
    {
        if (Status == ProgridaTaskStatus.Completed)
            return;

        Status = ProgridaTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Touch();
    }

    public void Reopen()
    {
        if (Status == ProgridaTaskStatus.Pending)
            return;

        Status = ProgridaTaskStatus.Pending;
        CompletedAt = null;
        Touch();
    }

    /// <summary>Regra 3: usuário A nunca acessa/edita/exclui tarefa do usuário B.</summary>
    public void EnsureOwnedBy(Guid userId)
    {
        if (UserId != userId)
            throw new ForbiddenAccessException("Esta tarefa não pertence ao usuário autenticado.");
    }
}
