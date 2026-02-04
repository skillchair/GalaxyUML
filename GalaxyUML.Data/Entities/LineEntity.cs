using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    class LineEntity
    {
        [Key]
        public Guid IdDrawable { get; set; }
        
        [Required]
        public Guid Box1Id { get; set; }
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