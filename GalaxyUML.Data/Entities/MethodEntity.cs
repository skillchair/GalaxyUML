using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class MethodEntity
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public Guid IdClassBox { get; set; }
        public virtual ClassBoxEntity ClassBox { get; set; } = null!;

        public string Method { get; set; } = null!;
    }
}