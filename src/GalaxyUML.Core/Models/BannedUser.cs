namespace GalaxyUML.Core.Models
{
    public class BannedUser
    {
        public Guid UserId { get; }
        public DateTime BannedAt { get; } = DateTime.UtcNow;
        public string? Reason { get; }

        public BannedUser(Guid userId, string? reason = null) { UserId = userId; Reason = reason; }
    }
}
