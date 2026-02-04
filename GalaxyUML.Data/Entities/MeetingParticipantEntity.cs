using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data
{
    class MeetingParticipantEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Meeting")]
        public Guid IdMeeting { get; set; }

        [Required]
        [ForeignKey("Participant")]
        public Guid IdParticipant { get; set; }

        [Required]
        public required string Code { get; set; }
    }
}