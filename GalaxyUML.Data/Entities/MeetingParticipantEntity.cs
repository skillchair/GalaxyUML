using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data
{
    class MeetingParticipantEntity
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Meeting")]
        public Guid IdMeeting { get; set; }

        public MeetingEntity Meeting { get; set; }
    }
}