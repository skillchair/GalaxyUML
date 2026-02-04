using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    class MessageEntity
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