using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class MeetingParticipantEntity
    {
        [Key]
        public Guid Id { get; set; } = new Guid();
        
        [Required]
        public Guid IdMeeting { get; set; }
        public virtual MeetingEntity Meeting { get; set; } = null!;
        
        [Required]
        public Guid IdParticipant { get; set; }
        public virtual TeamMemberEntity Participant { get; set; } = null!;
    }
}