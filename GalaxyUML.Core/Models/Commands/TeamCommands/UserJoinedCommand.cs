namespace GalaxyUML.Core.Models.Commands.TeamCommands
{
    class UserJoinedCommand : ITeamCommand
    {
        public User User { get; set; }
        public Team Team { get; set; }

        public UserJoinedCommand(User user, Team team)
        {
            User = user;
            Team = team;
        }

        public void execute()
        {
            Team.AddMember(User);
        }
    }
}