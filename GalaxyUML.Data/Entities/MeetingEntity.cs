using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    class MeetingEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public DateTime StartingTime { get; set; }
        
        public DateTime? EndingTime { get; set; }
        
        [Required]
        public Guid IdOrganizer { get; set; }
        public virtual UserEntity Organizer { get; set; } = null!;
        
        [Required]
        public Guid IdTeam { get; set; }
        public virtual TeamEntity Team { get; set; } = null!;
        
        public virtual ICollection<MeetingParticipantEntity> MeetingParticipants { get; set; } = new List<MeetingParticipantEntity>();
        
        [Required]
        public Guid IdChat { get; set; }
        public virtual ChatEntity Chat { get; set; } = null!;

        [Required]
        public Guid IdBoard { get; set; }
        public virtual DiagramEntity Board { get; set; } = null!;
        
        public bool IsActive { get; set; }
    }
}