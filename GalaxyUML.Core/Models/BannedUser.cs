namespace GalaxyUML.Core.Models
{
    public class BannedUser
    {
        //public Guid IdBan { get; private set; }
        public User User { get; private set; }
        public Guid IdUser { get; private set; }
        public Team Team { get; private set; }
        public Guid IdTeam { get; private set; }

        public BannedUser(/*Guid id, */Guid idTeam, Guid idUser, Team team, User user)
        {
            //IdBan = id;
            IdTeam = idTeam;
            IdUser = idUser;
            User = user;
            Team = team;
        }
    }
}