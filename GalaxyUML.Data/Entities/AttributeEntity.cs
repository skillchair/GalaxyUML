using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    class AttributeEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IdClassBox { get; set; }
        public virtual ClassBoxEntity ClassBox { get; set; } = null!;

        [Required]
        public string Attribute { get; set; } = null!;
    }
}