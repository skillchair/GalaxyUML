using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    class TeamEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TeamName { get; set; } = null!;
        
        [Required]
        public Guid IdTeamOwner { get; set; }
        public virtual UserEntity TeamOwner { get; set; } = null!;
        
        [Required]
        [StringLength(6)]
        public string TeamCode { get; set; } = null!;
        
        public virtual ICollection<MeetingEntity> Meetings { get; set; } = null!;
        public virtual ICollection<TeamMemberEntity> Members { get; set; } = null!;
        public virtual ICollection<BannedUserEntity> BannedUsers { get; set; } = null!;
    }
}