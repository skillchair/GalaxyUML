using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class MeetingParticipantEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid IdMeeting { get; set; }
        
        [ForeignKey("IdMeeting")]
        public virtual MeetingEntity Meeting { get; set; } = null!;
        
        [Required]
        public Guid IdParticipant { get; set; }
        
        [ForeignKey("IdParticipant")]
        public virtual UserEntity Participant { get; set; } = null!;
    }
}