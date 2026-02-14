using ObjectType = GalaxyUML.Core.Models.ObjectType;

namespace GalaxyUML.Data.Entities;
public class TextEntity : DiagramElementEntity
{
    public TextEntity() { ObjectType = ObjectType.Text; }
    public string Content { get; set; } = "";
    public string? Format { get; set; }
    public int FontSize { get; set; } = 14;
    public string Color { get; set; } = "#000";
}
