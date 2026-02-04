using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    class MeetingParticipantEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid IdMeeting { get; set; }
        public virtual MeetingEntity Meeting { get; set; } = null!;
        
        [Required]
        public Guid IdParticipant { get; set; }
        public virtual TeamMemberEntity Participant { get; set; } = null!;
    }
}