using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class ClassBoxEntity : BoxEntity
    {
        [Required]
        public string Name { get; set; } = "Class";

        [Required]
        public string Stereotype { get; set; } = "";

        [Required]
        public double TextSize { get; set; }

        public ICollection<AttributeEntity>? Attributes { get; set; }
        public ICollection<MethodEntity>? Methods { get; set; }
    }
}