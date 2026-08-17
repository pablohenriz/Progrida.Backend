using Progrida.Domain.Entities;

namespace Progrida.Domain.Interfaces;

/// <summary>
/// O Domain define O QUE é preciso (os contratos).
/// A Infrastructure decide COMO isso é feito (EF Core + PostgreSQL).
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
}

public interface ITaskSectionRepository
{
    Task<TaskSection?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<TaskSection>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(TaskSection section, CancellationToken ct = default);
    void Remove(TaskSection section);
}

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<TaskItem>> GetByUserIdAsync(Guid userId, Guid? sectionId = null, CancellationToken ct = default);
    Task AddAsync(TaskItem task, CancellationToken ct = default);
    void Remove(TaskItem task);
}

/// <summary>
/// Unit of Work: garante que as alterações feitas em um caso de uso
/// sejam persistidas em uma única transação lógica.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
