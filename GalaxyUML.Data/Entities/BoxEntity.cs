namespace GalaxyUML.Data.Entities
{
    public class BoxEntity : DrawableEntity
    {
        public virtual ICollection<LineEntity>? Lines { get; set; } = null!;
    }
}