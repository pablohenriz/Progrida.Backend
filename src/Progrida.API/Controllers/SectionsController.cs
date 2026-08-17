using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progrida.Application.Sections;
using Progrida.Application.Sections.Commands;
using Progrida.Application.Sections.Queries;

namespace Progrida.API.Controllers;

[ApiController]
[Authorize]
[Route("api/sections")]
public class SectionsController : ControllerBase
{
    private readonly GetSectionsHandler _getSections;
    private readonly CreateSectionHandler _createSection;
    private readonly UpdateSectionHandler _updateSection;
    private readonly DeleteSectionHandler _deleteSection;
    private readonly ReorderSectionsHandler _reorderSections;

    public SectionsController(
        GetSectionsHandler getSections,
        CreateSectionHandler createSection,
        UpdateSectionHandler updateSection,
        DeleteSectionHandler deleteSection,
        ReorderSectionsHandler reorderSections)
    {
        _getSections = getSections;
        _createSection = createSection;
        _updateSection = updateSection;
        _deleteSection = deleteSection;
        _reorderSections = reorderSections;
    }

    [HttpGet]
    public async Task<ActionResult<List<SectionDto>>> GetAll(CancellationToken ct) =>
        Ok(await _getSections.Handle(ct));

    [HttpPost]
    public async Task<ActionResult<SectionDto>> Create(CreateSectionRequest request, CancellationToken ct) =>
        Ok(await _createSection.Handle(request, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SectionDto>> Update(Guid id, RenameSectionBody body, CancellationToken ct) =>
        Ok(await _updateSection.Handle(new UpdateSectionRequest(id, body.Name), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _deleteSection.Handle(id, ct);
        return NoContent();
    }

    [HttpPatch("reorder")]
    public async Task<IActionResult> Reorder(ReorderSectionsBody body, CancellationToken ct)
    {
        var items = body.Items.Select(i => (i.SectionId, i.NewPosition)).ToList();
        await _reorderSections.Handle(new ReorderSectionsRequest(items), ct);
        return NoContent();
    }
}

public record RenameSectionBody(string Name);
public record ReorderSectionItem(Guid SectionId, int NewPosition);
public record ReorderSectionsBody(List<ReorderSectionItem> Items);
