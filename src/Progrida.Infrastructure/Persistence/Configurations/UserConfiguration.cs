using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Progrida.Domain.Entities;

namespace Progrida.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name).HasMaxLength(150).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        // Este DbContext nunca deve carregar essas coleções automaticamente
        // (evita o problema clássico de N+1 / vazamento de dados entre usuários).
        builder.Ignore(u => u.Sections);
        builder.Ignore(u => u.Tasks);
    }
}
