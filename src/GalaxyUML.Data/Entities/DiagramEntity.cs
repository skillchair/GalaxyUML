using GalaxyUML.Core.Models;

using ObjectType = GalaxyUML.Core.Models.ObjectType;
namespace GalaxyUML.Data.Entities;
public class DiagramEntity : DiagramElementEntity
{
    public DiagramEntity() { ObjectType = ObjectType.Diagram; }
    public Guid? MeetingId { get; set; }   // root board link
    public MeetingEntity? Meeting { get; set; }
}
