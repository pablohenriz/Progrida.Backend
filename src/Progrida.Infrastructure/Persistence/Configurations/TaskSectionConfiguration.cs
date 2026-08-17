using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Progrida.Domain.Entities;

namespace Progrida.Infrastructure.Persistence.Configurations;

public class TaskSectionConfiguration : IEntityTypeConfiguration<TaskSection>
{
    public void Configure(EntityTypeBuilder<TaskSection> builder)
    {
        builder.ToTable("sections");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(120).IsRequired();
        builder.Property(s => s.Position).IsRequired();

        builder.HasIndex(s => s.UserId);
    }
}
