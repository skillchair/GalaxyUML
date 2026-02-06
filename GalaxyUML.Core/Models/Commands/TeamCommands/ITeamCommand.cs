namespace GalaxyUML.Core.Models.Commands.TeamCommands
{
    public abstract class ITeamCommand
    {
        public Team Team { get; private set; }

        public ITeamCommand(Team team)
        {
            Team = team;
        }

        public abstract void Execute(TeamEventType eventType);
    }
}