using Microsoft.EntityFrameworkCore;
using Progrida.Domain.Entities;
using Progrida.Domain.Interfaces;

namespace Progrida.Infrastructure.Persistence;

public class ProgridaDbContext : DbContext, IUnitOfWork
{
    public ProgridaDbContext(DbContextOptions<ProgridaDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskSection> Sections => Set<TaskSection>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProgridaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
