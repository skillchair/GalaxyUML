namespace GalaxyUML.Core.Models
{
    public class BannedUser
    {
        public User User { get; private set; }
        public Team Team { get; private set; }

        public BannedUser(User user, Team team)
        {
            User = user;
            Team = team;
        }
    }
}