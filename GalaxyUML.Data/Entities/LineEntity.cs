using ObjectType = GalaxyUML.Core.Models.ObjectType;

namespace GalaxyUML.Data.Entities;
public class LineEntity : DiagramElementEntity
{
    public LineEntity() { ObjectType = ObjectType.Line; }

    public Guid StartBoxId { get; set; }
    public BoxEntity StartBox { get; set; } = null!;
    public Guid EndBoxId { get; set; }
    public BoxEntity EndBox { get; set; } = null!;

    public string? MiddleText { get; set; }
    public string? Text1 { get; set; }
    public string? Text2 { get; set; }
}
