using System.Text.Json.Serialization;
using PasswordHelper = GalaxyUML.Core.Security.PasswordHelper;

namespace GalaxyUML.Core.Models
{
    public class User
    {
        public Guid IdUser { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public List<Team> Teams { get; private set; }
        public List<BannedUser> Bans { get; private set; }
        
        [JsonConstructor] // Kažeš JSON-u: "Koristi BAŠ ovaj konstruktor"
        public User(Guid idUser, string firstName, string lastName, string username, string email, string password)
        {
            IdUser = idUser; // Parametar se sada zove isto kao property (idUser -> IdUser)
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
            Password = password;
            Teams = new List<Team>();
            Bans = new List<BannedUser>();
        }

        public Team CreateTeam(Guid idOwner, Guid idTeam, string teamName) { return new Team(idTeam, idOwner, teamName, this); }
        public void JoinTeam(Team team)
        {
            Teams.Add(team);
        }

        public void LeaveTeam(Team team)
        {
            var teamInAList = Teams.FirstOrDefault(team);
            if (teamInAList == null)
                throw new Exception("User is not this team's member.");

            Teams.Remove(teamInAList);
        }
    }
}