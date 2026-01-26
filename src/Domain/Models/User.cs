namespace GalaxyUML.Models
{
    public class User
    {
        public Guid IdUser { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Team> Teams { get; private set; }

        public User(string firstName, string lastName, string username, string email, string password)
        {
            IdUser = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
            Password = password;
            Teams = new List<>();
        }

        public Team CreateTeam(string teamName) { return Team(teamName, IdUser); }
        
        public bool JoinTeam(Team team)
        {
            Teams.Add(team);
            team.AddMember(this);
        }
    }
}