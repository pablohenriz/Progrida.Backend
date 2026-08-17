using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Progrida.Domain.Entities;

namespace Progrida.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Position).IsRequired();

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.SectionId);

        // FK "solta" (sem navegação obrigatória) — mantém o Domain independente do EF,
        // mas ainda garante integridade referencial no banco.
        builder.HasOne<TaskSection>()
            .WithMany()
            .HasForeignKey(t => t.SectionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
