namespace GalaxyUML.Data.Entities
{
    class MethodEntity
    {
        [Key]
        public Guid IdMethod { get; set; }

        [Required]
        public Guid IdClassBox { get; set; }
        public virtual ClassBoxEntity ClassBox { get; set; } = null!;

        public string Method { get; set; } = null!;
    }
}