using System.ComponentModel.DataAnnotations;

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
        public virtual TeamMemberEntity TeamOwner { get; set; } = null!;

        [Required]
        [StringLength(6)]
        public string TeamCode { get; set; } = null!;

        public virtual ICollection<TeamMemberEntity> Members { get; set; } = null!;
        public virtual ICollection<MeetingEntity>? Meetings { get; set; }
        public virtual ICollection<BannedUserEntity>? BannedUsers { get; set; }
    }
}