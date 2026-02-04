using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class BoxEntity
    {
        [Key]
        public Guid IdBox { get; set; }
        
        [Required]
        public Guid IdDiagram { get; set; }
        
        [ForeignKey("IdDiagram")]
        public virtual DiagramEntity Diagram { get; set; } = null!;
        
        [Required]
        public string Type { get; set; } = null!;
        
        [Required]
        public int X { get; set; }
        
        [Required]
        public int Y { get; set; }
        
        [Required]
        public int Width { get; set; }
        
        [Required]
        public int Height { get; set; }
        
        public string Content { get; set; } = null!;
    }
}