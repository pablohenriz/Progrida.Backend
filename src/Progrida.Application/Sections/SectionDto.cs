using Progrida.Domain.Entities;

namespace Progrida.Application.Sections;

public record SectionDto(Guid Id, string Name, int Position, DateTime CreatedAt)
{
    public static SectionDto FromEntity(TaskSection section) =>
        new(section.Id, section.Name, section.Position, section.CreatedAt);
}
