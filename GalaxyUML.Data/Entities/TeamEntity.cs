using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class TeamEntity
    {
        [Key]
        public Guid IdTeam { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TeamName { get; set; } = null!;
        
        [Required]
        public Guid IdTeamOwner { get; set; }
        
        [ForeignKey("IdTeamOwner")]
        public virtual UserEntity TeamOwner { get; set; } = null!;
        
        [Required]
        [StringLength(6)]
        public string TeamCode { get; set; } = null!;
        
        public virtual ICollection<MeetingEntity> Meetings { get; set; } = null!;
        public virtual ICollection<TeamMemberEntity> TeamMembers { get; set; } = null!;
        public virtual ICollection<UserEntity> BannedUsers { get; set; } = null!;
    }
}