using Progrida.Domain.Exceptions;

namespace Progrida.Domain.Entities;

/// <summary>
/// Regras do Usuário:
///  - Nome obrigatório.
///  - Email obrigatório e válido.
///  - Senha nunca é armazenada em texto puro (apenas o hash).
/// </summary>
public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    private readonly List<TaskSection> _sections = new();
    public IReadOnlyCollection<TaskSection> Sections => _sections.AsReadOnly();

    private readonly List<TaskItem> _tasks = new();
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    protected User() { } // EF Core

    public static User Create(string name, string email, string passwordHash)
    {
        var user = new User();
        user.SetName(name);
        user.SetEmail(email);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("A senha não pode ser vazia.");

        user.PasswordHash = passwordHash;
        return user;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do usuário é obrigatório.");

        if (name.Length > 150)
            throw new DomainException("O nome do usuário deve ter no máximo 150 caracteres.");

        Name = name.Trim();
        Touch();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("O email é obrigatório.");

        if (!email.Contains('@') || !email.Contains('.'))
            throw new DomainException("O email informado não é válido.");

        Email = email.Trim().ToLowerInvariant();
        Touch();
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("A senha não pode ser vazia.");

        PasswordHash = newPasswordHash;
        Touch();
    }
}
