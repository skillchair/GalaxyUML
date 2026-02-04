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
        public Meeting? Meeting { get; private set; }

        public User(string firstName, string lastName, string username, string email, string password)
        {
            IdUser = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
            Password = password;
            Teams = new List<Team>();
        }

        public Team CreateTeam(string teamName) { return new Team(teamName, this); }
        public void JoinTeam(Team team)
        {
            Teams.Add(team);
            team.AddMember(this);
        }

        public void LeaveTeam(Team team)
        {
            var teamInAList = Teams.FirstOrDefault(t => t.IdTeam == team.IdTeam);
            if (teamInAList == null)
                throw new Exception("User is not this team's member.");

            Teams.Remove(teamInAList);
        }

        public void JoinMeeting(Team team, Meeting meeting)
        {

            if (Meeting != null)
                throw new Exception("User is already in a meeting.");
                
            var teamInAList = Teams.FirstOrDefault(t => t.IdTeam == team.IdTeam);
            if (teamInAList == null)
                throw new Exception("User is not this team's member.");

            Meeting = meeting;
            Meeting.AddParticipant(this);
        }

        public void LeaveMeeting()
        {
            if (Meeting == null)
                throw new Exception("User not in a meeting.");

            Meeting.RemoveParticipant(this);
            Meeting = null;
        }
    }
}