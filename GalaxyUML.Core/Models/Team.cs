using System.Text.Json.Serialization;
using GalaxyUML.Core.Models.Commands.TeamCommands;

namespace GalaxyUML.Core.Models
{
    public class Team : ITeamObservable
    {
        //public Guid IdTeam { get; private set; }
        public string TeamName { get; private set; } = null!;
        public TeamMember TeamOwner { get; private set; } = null!;
        public string TeamCode { get; private set; } = null!;
        public List<Meeting> Meetings { get; private set; } = null!;
        public List<TeamMember> Members { get; set; }
        public List<BannedUser> BannedUsers { get; private set; }
        public Guid IdOwner { get; private set; }

        private List<ITeamObserver> _observers;

        public Team() 
        {
            _observers = new List<ITeamObserver>();
            Meetings = new List<Meeting>();
            Members = new List<TeamMember>();
            BannedUsers = new List<BannedUser>();
        }

        [JsonConstructor] // Kažeš JSON-u: "Koristi BAŠ ovaj konstruktor"
        public Team(/*Guid id, */Guid id, Guid idTeamOwner, string teamName, User teamOwner)
        {
            //IdTeam = id;
            TeamName = teamName;
            IdOwner = idTeamOwner;
            TeamOwner = new TeamMember(id, this, teamOwner, RoleEnum.Owner);
            Members = [TeamOwner];
            TeamCode = TeamCodeGenerator();
            Meetings = new List<Meeting>();
            BannedUsers = new List<BannedUser>();

            _observers = new List<ITeamObserver>();
        }

        public void AddMember(Guid idTeam, User user)
        {
            // moze i da se napravi pa da se proba i po primarnom kljucu da se trazi; nisam siguran sta je bolja praksa...
            var memberInAList = Members.FirstOrDefault(m => m.Member.IdUser == user.IdUser);
            if (memberInAList != null)
                throw new Exception("User is already this team's member.");

            var bannedUser = BannedUsers.FirstOrDefault(b => b.User.IdUser == user.IdUser);
            if (bannedUser != null)
                throw new Exception("This user can not join because they are banned.");

            Members.Add(new TeamMember(idTeam, this, user, RoleEnum.Member));
            user.JoinTeam(this);
        }

        public void RemoveMember(TeamMember member)
        {
            var teamMember = Members.FirstOrDefault(member);
            if (teamMember == null)
                throw new Exception("User is not this team's member.");
            
            Members.Remove(teamMember);
            member.LeaveTeam(this);
        }

        public void ChangeRole(TeamMember member, RoleEnum newRole)
        {
            if (member.Role == RoleEnum.Owner)
                throw new Exception("Owner's role can not be changed.");

            var teamMember = Members.FirstOrDefault(member);
            if (teamMember == null)
                throw new Exception("User is not this team's member.");

            if (newRole == RoleEnum.Member || newRole == RoleEnum.Organizer)
                throw new Exception("There can only be one owner of a team.");
                
            teamMember.ChangeRole(newRole);
        }
        // treba razmisliti
        public void OrganizeMeeting(Guid idTeam, Guid idMeeting, Guid idChat, Guid idBoard, Guid idOrganizer, TeamMember organizer)
        {
            if (organizer.Role is not RoleEnum.Organizer || organizer.Role is not RoleEnum.Owner)
                throw new Exception("Only organizers can organize meetings");

            Meetings.Add(new Meeting(idTeam, idMeeting, idOrganizer, idChat, idBoard, organizer));
        }

        public void EndMeeting(Meeting meeting)
        {
            if (meeting == null)
                throw new Exception("Meeting does not exist.");

            meeting.EndMeeting();
        }

        public void Ban(Guid idTeam, Guid idUser, User user)
        {
            var member = Members.FirstOrDefault(m => m.Member.IdUser == user.IdUser);
            if (member == null)
                throw new Exception("User not in this team.");

            //member.ClearEntry();        // brisemo i iz team-a i member napusta
            BannedUsers.Add(new BannedUser(idTeam, idUser, this, user));      // dodajemo u lokalnu listu banovanih
        }
        private string TeamCodeGenerator()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
        }

        public void Attach(ITeamObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(ITeamObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(TeamEventType eventType, ITeamCommand command)
        {
            foreach (var observer in _observers)
                observer.Update(eventType, command);
        }
    }
}