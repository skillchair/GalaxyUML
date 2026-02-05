namespace GalaxyUML.Core.Models.TeamCommands
{
    public class MemberLeftCommand//: ITeamCommand
    {
        public TeamMember Member { get; private set; }
        public Team Team { get; private set; }

        public MemberLeftCommand(TeamMember member, Team team)
        {
            Member = member;
            Team = team;
        }

        public void execute()
        {
            Team.RemoveMember(Member);
        }
    }
}