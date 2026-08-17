using Progrida.Application.Common.Interfaces;
using Progrida.Domain.Interfaces;

namespace Progrida.Application.Sections.Queries;

/// <summary>Caso de uso: listar as seções do usuário autenticado.</summary>
public class GetSectionsHandler
{
    private readonly ITaskSectionRepository _sections;
    private readonly ICurrentUserService _currentUser;

    public GetSectionsHandler(ITaskSectionRepository sections, ICurrentUserService currentUser)
    {
        _sections = sections;
        _currentUser = currentUser;
    }

    public async Task<List<SectionDto>> Handle(CancellationToken ct = default)
    {
        var sections = await _sections.GetByUserIdAsync(_currentUser.UserId, ct);
        return sections.OrderBy(s => s.Position).Select(SectionDto.FromEntity).ToList();
    }
}
