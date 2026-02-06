using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    class UserEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = null!;
        
        [Required]
        [StringLength(255)]
        public string Email { get; set; } = null!;
        
        [Required]
        public string Password { get; set; } = null!;
        
        public virtual ICollection<TeamMemberEntity> Teams { get; set; } = new List<TeamMemberEntity>();
    }
}