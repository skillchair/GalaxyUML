namespace GalaxyUML.Core.Models
{
    public class Team
    {
        private readonly List<TeamMember> _members = new();
        private readonly List<BannedUser> _bans = new();

        public Guid Id { get; }
        public string TeamName { get; private set; }
        public string TeamCode { get; private set; }
        public Guid OwnerId { get; private set; }
        public Guid? CurrentMeetingId { get; private set; }

        public IReadOnlyCollection<TeamMember> Members => _members.AsReadOnly();
        public IReadOnlyCollection<BannedUser> BannedUsers => _bans.AsReadOnly();

        public Team(Guid id, Guid ownerId, string teamName, string teamCode)
        {
            Id = id;
            OwnerId = ownerId;
            TeamName = teamName;
            TeamCode = teamCode;
            _members.Add(new TeamMember(ownerId, RoleEnum.Owner));
        }

        public static Team Create(string name, Guid ownerId) =>
            new(Guid.NewGuid(), ownerId, name, Guid.NewGuid().ToString("N")[..6].ToUpper());

        public void Join(Guid userId, string code)
        {
            if (code != TeamCode) throw new InvalidOperationException("Bad join code");
            if (_bans.Any(b => b.UserId == userId)) throw new InvalidOperationException("User banned");
            if (_members.Any(m => m.UserId == userId)) return;
            _members.Add(new TeamMember(userId, RoleEnum.Member));
        }

        public void Leave(Guid userId)
        {
            var member = _members.SingleOrDefault(m => m.UserId == userId)
                         ?? throw new InvalidOperationException("Not a member");
            if (member.Role == RoleEnum.Owner) throw new InvalidOperationException("Owner cannot leave");
            _members.Remove(member);
        }

        public void ChangeRole(Guid actorId, Guid targetUserId, RoleEnum newRole)
        {
            var actor = RequireMember(actorId);
            if (actor.Role is not (RoleEnum.Owner or RoleEnum.Organizer))
                throw new InvalidOperationException("Forbidden");
            var target = RequireMember(targetUserId);
            if (target.Role == RoleEnum.Owner && actor.Role != RoleEnum.Owner)
                throw new InvalidOperationException("Cannot change owner");
            target.SetRole(newRole);
        }

        public void Ban(Guid actorId, Guid targetUserId, string? reason = null)
        {
            var actor = RequireMember(actorId);
            if (actor.Role != RoleEnum.Owner) throw new InvalidOperationException("Only owner bans");
            if (_bans.Any(b => b.UserId == targetUserId)) return;
            _bans.Add(new BannedUser(targetUserId, reason));
            _members.RemoveAll(m => m.UserId == targetUserId);
        }

        public void StartMeeting(Guid meetingId, Guid organizerId)
        {
            var org = RequireMember(organizerId);
            if (org.Role is not (RoleEnum.Owner or RoleEnum.Organizer))
                throw new InvalidOperationException("Only owner/organizer");
            if (CurrentMeetingId != null) throw new InvalidOperationException("Meeting already active");
            CurrentMeetingId = meetingId;
        }

        public void EndMeeting(Guid meetingId)
        {
            if (CurrentMeetingId == meetingId) CurrentMeetingId = null;
        }

        private TeamMember RequireMember(Guid userId) =>
            _members.SingleOrDefault(m => m.UserId == userId)
            ?? throw new InvalidOperationException("Member missing");
    }
}
