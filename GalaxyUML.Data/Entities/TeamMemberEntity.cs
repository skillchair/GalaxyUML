using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class TeamMemberEntity
    {
        [Key]
        public Guid IdTeamMember { get; set; }
        
        [Required]
        public Guid IdTeam { get; set; }
        
        [ForeignKey("IdTeam")]
        public virtual TeamEntity Team { get; set; } = null!;
        
        [Required]
        public Guid IdMember { get; set; }
        
        [ForeignKey("IdMember")]
        public virtual UserEntity Member { get; set; } = null!;
        
        [Required]
        public string Role { get; set; } = null!;
    }
}