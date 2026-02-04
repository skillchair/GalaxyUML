using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class DiagramEntity
    {
        [Key]
        public Guid IdDiagram { get; set; }
        
        public virtual ICollection<BoxEntity> Boxes { get; set; } = null!;
        public virtual ICollection<LineEntity> Lines { get; set; } = null!;
        public virtual ICollection<TextEntity> Texts { get; set; } = null!;
    }
}