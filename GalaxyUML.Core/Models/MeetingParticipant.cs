namespace GalaxyUML.Core.Models
{
    public class MeetingParticipant
    {
        public Guid UserId { get; }
        public bool CanDraw { get; private set; }
        public DateTime JoinedAt { get; } = DateTime.UtcNow;

        public MeetingParticipant(Guid userId, bool canDraw) { UserId = userId; CanDraw = canDraw; }
        public void SetDraw(bool value) => CanDraw = value;
    }
}
