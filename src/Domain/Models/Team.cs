namespace GalaxyUML.Models
{
    public class Team
    {
        public Guid IdTeam { get; private set; }
        public string TeamName { get; private set; }
        public User Owner { get; private set; }
        public string TeamCode { get; private set; }
        public List<Meeting> Meetings { get; private set; }
        public List<TeamMember> Members { get; set; }

        public Team(string teamName, User owner)
        {
            IdTeam = Guid.NewGuid();
            TeamName = teamName;
            Owner = owner;
            TeamCode = TeamCodeGenerator();
            Members.Add(new TeamMember(TeamOwnerId, RoleEnum.Owner));
            Meetings = new List<>();
            Members = new List<>();
        }

        public MemberResult AddMember(User user)
        {
            var member = Members.FirstOrDefault(m => m.IdTeamMember == user.IdUser);
            if (member != null)
                return MemberResult.AlreadyAMember;

            Members.Add(new TeamMember(IdTeam, user.idUser, RoleEnum.Member));
            return MemberResult.Success;
        }

        public MemberResult RemoveMember(Guid memberId)
        {
            var member = Members.FirstOrDefault(m => m.IdTeamMember == memberId);
            if (member == null)
                return MemberResult.NotAMember;
            
            Members.Remove(member);
            return MemberResult.Success;
        }

        public MemberResult ChangeRole(Guid memberId, RoleEnum newRole)
        {
            var member = Members.FirstOrDefault(m => m.IdMember == memberId);

            if (member == null)
                return MemberResult.NotAMember;

            member.ChangeRole(newRole);
            return MemberResult.Success;
        }


        private string TeamCodeGenerator()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }
    }
}