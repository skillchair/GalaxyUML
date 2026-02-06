using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    class DiagramEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DrawableEntity Drawable { get; set; } = null!;

        [Required]
        public int Type { get; set; }

        [Required]
        public Guid IdMeeting { get; set; }
        public virtual MeetingEntity Meeting { get; set; } = null!;

        [Required]
        public Guid IdParent { get; set; }
        public virtual DiagramEntity? Parent { get; set; } // nema svaki parent-a (npr board nema)

        public virtual ICollection<DiagramEntity> Objects { get; set; } = new List<DiagramEntity>();
    }
}