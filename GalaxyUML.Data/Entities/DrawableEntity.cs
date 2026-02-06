using System.ComponentModel.DataAnnotations;
using System.Drawing;
using GalaxyUML.Core.Models;

namespace GalaxyUML.Data.Entities
{
    class DrawableEntity
    {
        [Key]
        public Guid IdDiagram { get; set; }    // isti kljuc ko u dijagramu; svaki drawable je i dijagram
        public DiagramEntity Diagram { get; set; } = null!;

        [Required]
        public int Type { get; set; }

        [Required]
        public Point StartingPoint { get; set; }

        [Required]
        public Point EndingPoint { get; set; }
    }
}