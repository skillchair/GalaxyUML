namespace GalaxyUML.Core
{
    public class Team
    {
        public Guid IdTeam { get; private set; }
        public string TeamName { get; private set; }
        public User Owner { get; private set; }
        public string TeamCode { get; private set; }
        public List<Meeting> Meetings { get; private set; }
        public List<TeamMember> Members { get; set; }
        public List<User> BannedUsers { get; private set; }

        public Team(string teamName, User owner)
        {
            IdTeam = Guid.NewGuid();
            TeamName = teamName;
            Owner = owner;
            TeamCode = TeamCodeGenerator();
            Members = [new TeamMember(this, owner, new RoleOwner(this))];
            Meetings = new List<Meeting>();
            BannedUsers = new List<User>();
        }

        public void AddMember(User user)
        {
            var member = Members.FirstOrDefault(m => m.IdTeamMember == user.IdUser);
            if (member != null)
                throw new Exception("User is already this team's member.");

            var bannedUser = BannedUsers.FirstOrDefault(b => b.IdUser == user.IdUser);
            if (bannedUser != null)
                throw new Exception("This user can not join because they are banned.");

            Members.Add(new TeamMember(this, user, new RoleMember()));
        }

        public void RemoveMember(User member)
        {
            var teamMember = Members.FirstOrDefault(m => m.Member == member);
            if (teamMember == null)
                throw new Exception("User is not this team's member.");
            
            Members.Remove(teamMember);
        }

        public void ChangeRole(User member, IRole newRole)
        {
            if (member.IdUser == this.Owner.IdUser)
                throw new Exception("Owner's role can not be changed.");

            var teamMember = Members.FirstOrDefault(m => m.Member == member);
            if (teamMember == null)
                throw new Exception("User is not this team's member.");

            if (newRole == new RoleMember() || newRole == new RoleOrganizer())
                throw new Exception("There can only be one owner of a team.");
                
            teamMember.ChangeRole(newRole);
        }

        public void Ban(User user)
        {
            var member = Members.FirstOrDefault(m => m.Member.IdUser == user.IdUser);
            if (member == null)
                throw new Exception("User not in this team.");

            member.ClearEntry();        // brisemo i iz team-a i member napusta
            BannedUsers.Add(user);      // dodajemo u lokalnu listu banovanih
        }
        private string TeamCodeGenerator()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }
    }
}