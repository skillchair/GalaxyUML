using System.Drawing;
using GalaxyUML.Core.Models;
using GalaxyUML.Data.Repositories;

namespace GalaxyUML.Core.Services;

public class DiagramService
{
    private readonly IDiagramRepo _diagrams;
    public DiagramService(IDiagramRepo diagrams) => _diagrams = diagrams;

    public async Task MoveAsync(Guid id, int dx, int dy)
    {
        var el = await _diagrams.GetByIdAsync(id) ?? throw new InvalidOperationException("Not found");
        el.Move(new Point(el.StartingPoint.X + dx, el.StartingPoint.Y + dy));
        await _diagrams.SaveAsync();
    }

    public async Task ResizeAsync(Guid id, int width, int height)
    {
        var el = await _diagrams.GetByIdAsync(id) ?? throw new InvalidOperationException("Not found");
        el.Resize(new Point(el.StartingPoint.X + width, el.StartingPoint.Y + height));
        await _diagrams.SaveAsync();
    }

    public async Task EditTextAsync(Guid id, string content, int fontSize, string color, string? format)
    {
        if (await _diagrams.GetByIdAsync(id) is not Text t) throw new InvalidOperationException("Not a text");
        t.Update(content, fontSize, color, format);
        await _diagrams.SaveAsync();
    }

    public Task DeleteAsync(Guid id) => _diagrams.RemoveAsync(id);
}
