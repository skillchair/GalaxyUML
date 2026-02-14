using System.ComponentModel.DataAnnotations;
using GalaxyUML.Core.Models; // ObjectType enum

namespace GalaxyUML.Data.Entities;
public abstract class DiagramElementEntity
{
    [Key] public Guid Id { get; set; }
    [Required] public ObjectType ObjectType { get; set; }

    public Guid? ParentId { get; set; }
    public DiagramElementEntity? Parent { get; set; }
    public ICollection<DiagramElementEntity> Children { get; set; } = new List<DiagramElementEntity>();

    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }
}
