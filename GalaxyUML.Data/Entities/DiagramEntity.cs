using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    class DiagramEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public Guid IdMeeting { get; set; }
        public virtual MeetingEntity Meeting { get; set; } = null!;

        [Required]
        public Guid IdParent { get; set; }
        public virtual DiagramEntity? Diagram { get; set; } // nema svaki parent-a (npr board nema)

        public virtual ICollection<DrawableEntity> Drawables { get; set; } = new List<DrawableEntity>();
    }
}