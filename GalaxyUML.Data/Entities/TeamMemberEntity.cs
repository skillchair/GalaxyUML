using System.ComponentModel.DataAnnotations;
using GalaxyUML.Core.Models;

namespace GalaxyUML.Data.Entities
{
    public class TeamMemberEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IdTeam { get; set; }
        public virtual TeamEntity Team { get; set; } = null!;

        [Required]
        public Guid IdMember { get; set; }
        public virtual UserEntity Member { get; set; } = null!;

        [Required]
        public RoleEnum Role { get; set; }

        public virtual ICollection<MeetingParticipantEntity>? Meetings { get; set; } = null!;
    }
}