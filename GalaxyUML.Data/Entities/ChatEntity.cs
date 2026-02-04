using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class ChatEntity
    {
        [Key]
        public Guid IdChat { get; set; }
        
        public virtual ICollection<MessageEntity> Messages { get; set; } = null!;
    }
}