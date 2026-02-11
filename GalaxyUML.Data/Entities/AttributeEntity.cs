using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class AttributeEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public Guid IdClassBox { get; set; }
        public virtual ClassBoxEntity ClassBox { get; set; } = null!;
    }
}