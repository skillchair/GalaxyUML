using ObjectType = GalaxyUML.Core.Models.ObjectType;
namespace GalaxyUML.Data.Entities;
public class ClassBoxEntity : BoxEntity
{
    public ClassBoxEntity() { ObjectType = ObjectType.ClassBox; }
    public ICollection<ClassAttributeEntity> Attributes { get; set; } = new List<ClassAttributeEntity>();
    public ICollection<ClassMethodEntity> Methods { get; set; } = new List<ClassMethodEntity>();
}

public class ClassAttributeEntity
{
    public Guid Id { get; set; }
    public Guid ClassBoxId { get; set; }
    public ClassBoxEntity ClassBox { get; set; } = null!;
    public string Name { get; set; } = "";
}

public class ClassMethodEntity
{
    public Guid Id { get; set; }
    public Guid ClassBoxId { get; set; }
    public ClassBoxEntity ClassBox { get; set; } = null!;
    public string Signature { get; set; } = "";
}
