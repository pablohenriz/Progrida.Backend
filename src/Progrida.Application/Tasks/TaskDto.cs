using Progrida.Domain.Entities;

namespace Progrida.Application.Tasks;

public record TaskDto(
    Guid Id,
    Guid? SectionId,
    string Title,
    string? Description,
    string Status,
    int Position,
    DateTime? DueDate,
    DateTime? CompletedAt,
    DateTime CreatedAt)
{
    public static TaskDto FromEntity(TaskItem task) => new(
        task.Id,
        task.SectionId,
        task.Title,
        task.Description,
        task.Status.ToString(),
        task.Position,
        task.DueDate,
        task.CompletedAt,
        task.CreatedAt);
}
