using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class LineEntity
    {
        [Key]
        public Guid IdLine { get; set; }
        
        [Required]
        public Guid IdDiagram { get; set; }
        
        [ForeignKey("IdDiagram")]
        public virtual DiagramEntity Diagram { get; set; } = null!;
        
        [Required]
        public int X1 { get; set; }
        
        [Required]
        public int Y1 { get; set; }
        
        [Required]
        public int X2 { get; set; }
        
        [Required]
        public int Y2 { get; set; }
        
        public string Type { get; set; } = null!;
    }
}