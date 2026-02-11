using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class TextEntity : DrawableEntity
    {
        [Required]
        public string Content { get; set; } = "Text";

        [Required]
        [MaxLength(50)]
        public string Format { get; set; } = null!;

        [Required]
        public double Size { get; set; }

        [Required]
        [MaxLength(9)]
        public string Color { get; set; } = null!;

        public virtual ICollection<TeamMemberEntity> TeamMembers { get; set; } = new List<TeamMemberEntity>();
    }
}