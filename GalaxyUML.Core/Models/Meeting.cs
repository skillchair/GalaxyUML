namespace GalaxyUML.Core.Models
{
    public class Meeting
    {
        private readonly List<MeetingParticipant> _participants = new();
        private readonly Chat _chat;

        public Guid Id { get; }
        public Guid TeamId { get; }
        public Guid OrganizedBy { get; }
        public Diagram Board { get; }
        public Chat Chat => _chat;
        public IReadOnlyCollection<MeetingParticipant> Participants => _participants.AsReadOnly();

        private Meeting(Guid id, Guid teamId, Guid organizerId, Diagram board, Chat chat)
        {
            Id = id; TeamId = teamId; OrganizedBy = organizerId; Board = board; _chat = chat;
            _participants.Add(new MeetingParticipant(organizerId, canDraw: true));
        }

        public static Meeting Create(Guid teamId, Guid organizerId) =>
            new(Guid.NewGuid(), teamId, organizerId,
                new Diagram(), new Chat());

        public void Join(Guid userId)
        {
            if (_participants.Any(p => p.UserId == userId)) return;
            _participants.Add(new MeetingParticipant(userId, false));
        }

        public void Leave(Guid userId)
        {
            var p = _participants.SingleOrDefault(x => x.UserId == userId);
            if (p == null) return;
            bool hadDraw = p.CanDraw;
            _participants.Remove(p);
            if (hadDraw)
            {
                var org = _participants.SingleOrDefault(x => x.UserId == OrganizedBy);
                if (org != null) org.SetDraw(true);
            }
        }

        public void GrantDraw(Guid actorId, Guid targetUserId, bool canDraw)
        {
            if (actorId != OrganizedBy) throw new InvalidOperationException("Only organizer can grant");
            var target = _participants.SingleOrDefault(p => p.UserId == targetUserId)
                         ?? throw new InvalidOperationException("Participant missing");
            target.SetDraw(canDraw);
        }

        public void AddMessage(Guid senderId, string content) => _chat.AddMessage(senderId, content);
    }
}
