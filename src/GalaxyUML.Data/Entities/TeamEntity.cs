using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities;
public class TeamEntity
{
    [Key] public Guid Id { get; set; }
    [Required, MaxLength(120)] public string TeamName { get; set; } = null!;
    [Required, MaxLength(6)]  public string TeamCode { get; set; } = null!;
    [Required] public Guid OwnerId { get; set; }

    public Guid? CurrentMeetingId { get; set; }
    public ICollection<TeamMemberEntity> Members { get; set; } = new List<TeamMemberEntity>();
    public ICollection<BannedUserEntity> BannedUsers { get; set; } = new List<BannedUserEntity>();
    public ICollection<MeetingEntity> Meetings { get; set; } = new List<MeetingEntity>();
}
