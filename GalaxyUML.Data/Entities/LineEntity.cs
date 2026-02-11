using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class LineEntity : DrawableEntity
    {   
        [Required]
        public Guid IdBox1 { get; set; }
        public virtual BoxEntity Box1 { get; set; } = null!;

        [Required]
        public Guid IdBox2 { get; set; }
        public virtual BoxEntity Box2 { get; set; } = null!;

        [MaxLength(50)]
        public string Text1 { get; set; } = "";

        [MaxLength(50)]
        public string Text2 { get; set; } = "";

        [MaxLength(50)]
        public string MiddleText { get; set; } = "";
    }
}