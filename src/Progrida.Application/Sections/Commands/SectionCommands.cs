using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Entities;
using Progrida.Domain.Exceptions;
using Progrida.Domain.Interfaces;

namespace Progrida.Application.Sections.Commands;

public record CreateSectionRequest(string Name);
public record UpdateSectionRequest(Guid SectionId, string Name);
public record ReorderSectionsRequest(IReadOnlyList<(Guid SectionId, int NewPosition)> Items);

/// <summary>Caso de uso: criar uma seção para o usuário autenticado.</summary>
public class CreateSectionHandler
{
    private readonly ITaskSectionRepository _sections;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSectionHandler(ITaskSectionRepository sections, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _sections = sections;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<SectionDto> Handle(CreateSectionRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        var existing = await _sections.GetByUserIdAsync(userId, ct);
        var nextPosition = existing.Count == 0 ? 0 : existing.Max(s => s.Position) + 1;

        var section = TaskSection.Create(userId, request.Name, nextPosition);
        await _sections.AddAsync(section, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return SectionDto.FromEntity(section);
    }
}

/// <summary>Caso de uso: renomear uma seção do usuário autenticado.</summary>
public class UpdateSectionHandler
{
    private readonly ITaskSectionRepository _sections;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSectionHandler(ITaskSectionRepository sections, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _sections = sections;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<SectionDto> Handle(UpdateSectionRequest request, CancellationToken ct = default)
    {
        var section = await _sections.GetByIdAsync(request.SectionId, ct)
            ?? throw new NotFoundException("Seção", request.SectionId);

        section.EnsureOwnedBy(_currentUser.UserId);
        section.Rename(request.Name);

        await _unitOfWork.SaveChangesAsync(ct);
        return SectionDto.FromEntity(section);
    }
}

/// <summary>Caso de uso: excluir uma seção do usuário autenticado.</summary>
public class DeleteSectionHandler
{
    private readonly ITaskSectionRepository _sections;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSectionHandler(ITaskSectionRepository sections, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _sections = sections;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(Guid sectionId, CancellationToken ct = default)
    {
        var section = await _sections.GetByIdAsync(sectionId, ct)
            ?? throw new NotFoundException("Seção", sectionId);

        section.EnsureOwnedBy(_currentUser.UserId);

        _sections.Remove(section);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

/// <summary>Caso de uso: reordenar as seções do usuário autenticado.</summary>
public class ReorderSectionsHandler
{
    private readonly ITaskSectionRepository _sections;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderSectionsHandler(ITaskSectionRepository sections, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _sections = sections;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReorderSectionsRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;

        foreach (var (sectionId, newPosition) in request.Items)
        {
            var section = await _sections.GetByIdAsync(sectionId, ct)
                ?? throw new NotFoundException("Seção", sectionId);

            section.EnsureOwnedBy(userId);
            section.MoveTo(newPosition);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
