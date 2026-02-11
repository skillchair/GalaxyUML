using System.ComponentModel.DataAnnotations;
using System.Drawing;
using ObjectType = GalaxyUML.Core.Models.ObjectType;

namespace GalaxyUML.Data.Entities
{
    public class DiagramEntity
    {
        [Key]
        public Guid Id { get; set; }
        //public DrawableEntity Drawable { get; set; } = null!;

        [Required]
        public ObjectType Type { get; set; }

        [Required]
        public Point StartingPoint { get; set; }

        [Required]
        public Point EndingPoint { get; set; }

        [Required]
        public Guid IdMeeting { get; set; }
        public virtual MeetingEntity Meeting { get; set; } = null!;

        [Required]
        public Guid? IdParent { get; set; }
        public virtual DiagramEntity? Parent { get; set; } // nema svaki parent-a (npr board nema)

        public virtual ICollection<DiagramEntity>? Objects { get; set; }
    }
}