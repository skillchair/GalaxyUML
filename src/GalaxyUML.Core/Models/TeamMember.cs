namespace GalaxyUML.Core.Models
{
    public class TeamMember
    {
        public Guid UserId { get; }
        public RoleEnum Role { get; private set; }
        public DateTime JoinedAt { get; } = DateTime.UtcNow;

        public TeamMember(Guid userId, RoleEnum role) { UserId = userId; Role = role; }
        public void SetRole(RoleEnum role) => Role = role;
    }
}
