using GalaxyUML.Core.Models.Commands.MeetingCommands;

namespace GalaxyUML.Core.Models.Commands.TeamCommands
{
    class MeetingStartedCommand : IMeetingCommand
    {
        public TeamMember TeamMember { get; private set; }
        public Team Team { get; private set; }

        public MeetingStartedCommand(TeamMember teamMember, Team team)
        {
            TeamMember = teamMember;
            Team = team;
        }

        public void Execute()
        {
            Team.OrganizeMeeting(TeamMember);
        }
    }
}