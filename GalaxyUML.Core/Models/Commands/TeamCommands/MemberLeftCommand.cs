namespace GalaxyUML.Core.Models.Commands.TeamCommands
{
    public class MemberLeftCommand : ITeamCommand
    {
        public User User { get; private set; }

        public MemberLeftCommand(Team team, User user) : base(team)
        {
            User = user;
        }

        public override void Execute(TeamEventType eventType)
        {
            if (eventType != TeamEventType.MemberLeft)
                throw new Exception("Invalid event. Expected TeamEventType.MemberLeft.");

            User.LeaveTeam(Team);
        }
    }
}