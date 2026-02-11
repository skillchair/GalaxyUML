using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class UserEntity
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
        [StringLength(255), MinLength(6)]
        public string Password { get; set; } = null!;
        
        public virtual ICollection<TeamMemberEntity>? Teams { get; set; }
        public virtual ICollection<BannedUserEntity>? BannedTeams { get; set; }
    }
}