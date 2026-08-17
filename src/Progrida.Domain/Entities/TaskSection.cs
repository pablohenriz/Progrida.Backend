using Progrida.Domain.Exceptions;

namespace Progrida.Domain.Entities;

/// <summary>
/// Regras da Seção (Regra 5 do documento):
///  - Pertence a um único usuário.
///  - Nome obrigatório.
///  - Possui uma posição usada para ordenação (drag-and-drop).
/// </summary>
public class TaskSection : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Position { get; private set; }

    protected TaskSection() { } // EF Core

    public static TaskSection Create(Guid userId, string name, int position)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Uma seção precisa pertencer a um usuário.");

        var section = new TaskSection { UserId = userId, Position = position };
        section.Rename(name);
        return section;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome da seção é obrigatório.");

        if (name.Length > 120)
            throw new DomainException("O nome da seção deve ter no máximo 120 caracteres.");

        Name = name.Trim();
        Touch();
    }

    public void MoveTo(int newPosition)
    {
        if (newPosition < 0)
            throw new DomainException("A posição da seção não pode ser negativa.");

        Position = newPosition;
        Touch();
    }

    /// <summary>Regra 5: o usuário B não pode colocar tarefas na seção do usuário A.</summary>
    public void EnsureOwnedBy(Guid userId)
    {
        if (UserId != userId)
            throw new ForbiddenAccessException("Esta seção não pertence ao usuário autenticado.");
    }
}
