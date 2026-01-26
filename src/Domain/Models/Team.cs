using System.Reflection.Metadata;

namespace GalaxyUML.Models
{
    public class Team
    {
        public Guid IdTeam { get; private set; }
        public string TeamName { get; private set; }
        public Guid TeamOwnerId { get; private set; }
        public User Owner { get; set; }
        public string TeamCode { get; private set; }
        public List<Meeting> Meetings { get; private set; }
        public List<TeamMember> Members { get; set; }

        public Team(string teamName, Guid teamOwnerId)
        {
            IdTeam = Guid.NewGuid();
            TeamName = teamName;
            TeamOwnerId = teamOwnerId;
            // Owner = ...
            TeamCode = TeamCodeGenerator();
            Members.Add(new TeamMember(TeamOwnerId, RoleEnum.Owner));
        }

        public RemoveMember(Guid idMember)
        {
            //
        }

        // treba napraviti
        public static int FindTeamWithCode()
        {
            return 0;
        }

        private string TeamCodeGenerator()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }
    }
}