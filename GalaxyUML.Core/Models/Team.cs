using GalaxyUML.Core.Models.Commands.TeamCommands;

namespace GalaxyUML.Core.Models
{
    public class Team : ITeamObservable
    {
        public Guid IdTeam { get; private set; }
        public string TeamName { get; private set; }
        public TeamMember TeamOwner { get; private set; }
        public string TeamCode { get; private set; }
        public List<Meeting> Meetings { get; private set; }
        public List<TeamMember> Members { get; set; }
        public List<BannedUser> BannedUsers { get; private set; }

        private List<ITeamObserver> _observers;

        public Team(string teamName, User owner)
        {
            IdTeam = Guid.NewGuid();
            TeamName = teamName;
            TeamOwner = new TeamMember(this, owner, RoleEnum.Owner);
            Members = [TeamOwner];
            TeamCode = TeamCodeGenerator();
            Meetings = new List<Meeting>();
            BannedUsers = new List<BannedUser>();

            _observers = new List<ITeamObserver>();
        }

        public void AddMember(User user)
        {
            // moze i da se napravi pa da se proba i po primarnom kljucu da se trazi; nisam siguran sta je bolja praksa...
            var memberInAList = Members.FirstOrDefault(m => m.Member.IdUser == user.IdUser);
            if (memberInAList != null)
                throw new Exception("User is already this team's member.");

            var bannedUser = BannedUsers.FirstOrDefault(b => b.User.IdUser == user.IdUser);
            if (bannedUser != null)
                throw new Exception("This user can not join because they are banned.");

            Members.Add(new TeamMember(this, user, RoleEnum.Member));
            user.JoinTeam(this);
        }

        public void RemoveMember(TeamMember member)
        {
            var teamMember = Members.FirstOrDefault(m => m.IdTeamMember == member.IdTeamMember);
            if (teamMember == null)
                throw new Exception("User is not this team's member.");
            
            Members.Remove(teamMember);
            member.LeaveTeam(this);
        }

        public void ChangeRole(TeamMember member, RoleEnum newRole)
        {
            if (member.IdTeamMember == TeamOwner.IdTeamMember)
                throw new Exception("Owner's role can not be changed.");

            var teamMember = Members.FirstOrDefault(m => m.IdTeamMember == member.IdTeamMember);
            if (teamMember == null)
                throw new Exception("User is not this team's member.");

            if (newRole == RoleEnum.Member || newRole == RoleEnum.Organizer)
                throw new Exception("There can only be one owner of a team.");
                
            teamMember.ChangeRole(newRole);
        }
        public void OrganizeMeeting(TeamMember owner)
        {
            if (owner.Role is not RoleEnum.Organizer || owner.Role is not RoleEnum.Owner)
                throw new Exception("Only organizers can organize meetings");

            Meetings.Add(new Meeting(owner));
        }

        public void EndMeeting(Meeting meeting)
        {
            if (meeting == null)
                throw new Exception("Meeting does not exist.");

            meeting.EndMeeting();
        }

        public void Ban(User user)
        {
            var member = Members.FirstOrDefault(m => m.Member.IdUser == user.IdUser);
            if (member == null)
                throw new Exception("User not in this team.");

            //member.ClearEntry();        // brisemo i iz team-a i member napusta
            BannedUsers.Add(new BannedUser(user, this));      // dodajemo u lokalnu listu banovanih
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