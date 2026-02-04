using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class MeetingParticipantEntity
    {
        [Key]
        public Guid Id { get; set; }
<<<<<<< HEAD
        
        [Required]
        public Guid IdMeeting { get; set; }
        
        [ForeignKey("IdMeeting")]
        public virtual MeetingEntity Meeting { get; set; } = null!;
        
        [Required]
        public Guid IdParticipant { get; set; }
        
        [ForeignKey("IdParticipant")]
        public virtual UserEntity Participant { get; set; } = null!;
=======

        [Required]
        [ForeignKey("Meeting")]
        public Guid IdMeeting { get; set; }

        [Required]
        [ForeignKey("Participant")]
        public Guid IdParticipant { get; set; }

        [Required]
        public required string Code { get; set; }
>>>>>>> 621de3dfa30e40a0490cad19a2bca0e7d954bb14
    }
}