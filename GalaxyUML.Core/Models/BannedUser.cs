namespace GalaxyUML.Core.Models
{
    public class BannedUser
    {
        public Guid IdBan { get; private set; }
        public User User { get; private set; }
        public Team Team { get; private set; }

        public BannedUser(User user, Team team)
        {
            IdBan = Guid.NewGuid();
            User = user;
            Team = team;
        }
    }
}