namespace GalaxyUML.Core.Models.Commands.TeamCommands
{
    public class UserJoinedCommand : ITeamCommand
    {
        public User User { get; set; }

        public UserJoinedCommand(Team team, User user) : base(team)
        {
            User = user;
        }

        public override void Execute(TeamEventType eventType)
        {
            if (eventType != TeamEventType.UserJoined)
                throw new Exception("Invalid event. Expected TeamEventType.UserJoined.");

            User.JoinTeam(Team);
        }
    }
}