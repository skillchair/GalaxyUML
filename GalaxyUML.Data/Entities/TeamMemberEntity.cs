using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    class TeamMemberEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Role { get; set; }

        [Required]
        public Guid IdTeam { get; set; }
        public virtual TeamEntity Team { get; set; } = null!;

        [Required]
        public Guid IdMember { get; set; }
        public virtual UserEntity Member { get; set; } = null!;
    }
}