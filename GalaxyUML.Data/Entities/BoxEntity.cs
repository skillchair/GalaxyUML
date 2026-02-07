namespace GalaxyUML.Data.Entities
{
    class BoxEntity : DrawableEntity
    {
        public virtual ICollection<LineEntity>? Lines { get; set; } = null!;
    }
}