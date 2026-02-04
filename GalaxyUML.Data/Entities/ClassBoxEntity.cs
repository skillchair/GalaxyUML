namespace GalaxyUML.Data.Entities
{
    class ClassBoxEntity
    {
        [Key]
        public Guid IdBox { get; set; }     // identifikuje se pomocu box posto je svaki classBox prevashodno box

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Stereotype { get; set; } = "";

        [Required]
        public double TextSize { get; set; }
    }
}