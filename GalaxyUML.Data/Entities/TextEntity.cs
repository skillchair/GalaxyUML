using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class TextEntity
    {
        [Key]
        public Guid IdText { get; set; }
        
        [Required]
        public Guid IdDiagram { get; set; }
        
        [ForeignKey("IdDiagram")]
        public virtual DiagramEntity Diagram { get; set; } = null!;
        
        [Required]
        public int X { get; set; }
        
        [Required]
        public int Y { get; set; }
        
        [Required]
        public string Content { get; set; } = null!;
        
        public string FontFamily { get; set; } = null!;
        
        public int FontSize { get; set; }
        
        public string FontColor { get; set; } = null!;
    }
}