using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace GalaxyUML.Data.Entities
{
    class DrawableEntity
    {
        [Key]
        public Guid IdDiagram { get; set; }    // isti kljuc ko u dijagramu; svaki drawable je i dijagram

        [Required]
        public int Type { get; set; }

        [Required]
        public Point StartingPoint { get; set; }

        [Required]
        public Point EndingPoint { get; set; }
    }
}