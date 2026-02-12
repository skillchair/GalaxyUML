namespace GalaxyUML.Data.Entities
{
    public class BoxEntity : DrawableEntity
{
    // Linije koje polaze iz ove kutije (Box1)
    public virtual ICollection<LineEntity> LinesAsStart { get; set; } = new HashSet<LineEntity>();
    
    // Linije koje ulaze u ovu kutiju (Box2)
    public virtual ICollection<LineEntity> LinesAsEnd { get; set; } = new HashSet<LineEntity>();
}
}