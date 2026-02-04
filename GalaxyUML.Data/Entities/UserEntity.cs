using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    public class UserEntity
    {
        [Key]
        public Guid IdUser { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = null!;
        
        [Required]
        [StringLength(255)]
        public string Email { get; set; } = null!;
        
        [Required]
        public string Password { get; set; } = null!; // Hash
        
        public virtual ICollection<TeamEntity> Teams { get; set; } = null!;
        public virtual ICollection<MeetingEntity> OrganizedMeetings { get; set; } = null!;
        public virtual ICollection<MeetingParticipantEntity> MeetingParticipants { get; set; } = null!;
        public virtual ICollection<MessageEntity> Messages { get; set; } = null!;
        public virtual ICollection<TeamMemberEntity> TeamMembers { get; set; } = null!;
    }
}