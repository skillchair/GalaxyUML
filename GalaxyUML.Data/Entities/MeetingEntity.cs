using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class MeetingEntity
    {
        [Key]
        public Guid IdMeeting { get; set; }
        
        [Required]
        public DateTime StartingTime { get; set; }
        
        public DateTime? EndingTime { get; set; }
        
        [Required]
        public Guid IdOrganizer { get; set; }
        
        [ForeignKey("IdOrganizer")]
        public virtual UserEntity Organizer { get; set; }
        
        [Required]
        public Guid IdTeam { get; set; }
        
        [ForeignKey("IdTeam")]
        public virtual TeamEntity Team { get; set; }
        
        public virtual ICollection<MeetingParticipantEntity> MeetingParticipants { get; set; }
        public virtual ChatEntity Chat { get; set; }
        public virtual DiagramEntity Board { get; set; }
        public virtual UserEntity ActiveParticipant { get; set; }
        
        public bool IsActive { get; set; }
    }
}