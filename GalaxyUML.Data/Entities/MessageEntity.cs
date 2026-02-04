using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class MessageEntity
    {
        [Key]
        public Guid IdMessage { get; set; }
        
        [Required]
        public Guid IdChat { get; set; }
        
        [ForeignKey("IdChat")]
        public virtual ChatEntity Chat { get; set; } = null!;
        
        [Required]
        public Guid IdSender { get; set; }
        
        [ForeignKey("IdSender")]
        public virtual UserEntity Sender { get; set; } = null!;
        
        [Required]
        public string Content { get; set; } = null!;
        
        [Required]
        public DateTime Timestamp { get; set; }
    }
}