using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class MessageEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid IdChat { get; set; }
        public virtual ChatEntity Chat { get; set; } = null!;
        
        [Required]
        public Guid IdMeetingParticipant { get; set; }
        public virtual MeetingParticipantEntity Sender { get; set; } = null!;
        
        [Required]
        public string Content { get; set; } = null!;
        
        [Required]
        public DateTime Timestamp { get; set; }
    }
}