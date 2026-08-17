using Microsoft.EntityFrameworkCore;
using Progrida.Domain.Entities;
using Progrida.Domain.Interfaces;
using Progrida.Infrastructure.Persistence;

namespace Progrida.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ProgridaDbContext _db;
    public UserRepository(ProgridaDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower(), ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await _db.Users.AddAsync(user, ct);
}

public class TaskSectionRepository : ITaskSectionRepository
{
    private readonly ProgridaDbContext _db;
    public TaskSectionRepository(ProgridaDbContext db) => _db = db;

    public Task<TaskSection?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Sections.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<List<TaskSection>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        _db.Sections.Where(s => s.UserId == userId).ToListAsync(ct);

    public async Task AddAsync(TaskSection section, CancellationToken ct = default) =>
        await _db.Sections.AddAsync(section, ct);

    public void Remove(TaskSection section) => _db.Sections.Remove(section);
}

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ProgridaDbContext _db;
    public TaskItemRepository(ProgridaDbContext db) => _db = db;

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<List<TaskItem>> GetByUserIdAsync(Guid userId, Guid? sectionId = null, CancellationToken ct = default)
    {
        var query = _db.Tasks.Where(t => t.UserId == userId);

        if (sectionId is not null)
            query = query.Where(t => t.SectionId == sectionId);

        return query.ToListAsync(ct);
    }

    public async Task AddAsync(TaskItem task, CancellationToken ct = default) =>
        await _db.Tasks.AddAsync(task, ct);

    public void Remove(TaskItem task) => _db.Tasks.Remove(task);
}
