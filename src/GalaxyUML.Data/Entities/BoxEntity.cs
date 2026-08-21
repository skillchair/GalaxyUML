using ObjectType = GalaxyUML.Core.Models.ObjectType;

namespace GalaxyUML.Data.Entities;
public class BoxEntity : DiagramElementEntity
{
    public BoxEntity() { ObjectType = ObjectType.Box; }
    public ICollection<LineEntity> Outgoing { get; set; } = new List<LineEntity>();
    public ICollection<LineEntity> Incoming { get; set; } = new List<LineEntity>();
}
