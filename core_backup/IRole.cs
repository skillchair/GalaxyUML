using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Core
{
    public abstract class IRole
    {
        public RoleEnum Role { get; protected set; }

        public IRole()
        {
        }

        public void ReleaseBoard(Meeting m) { m.ReleaseBoard(); }

        public void LeaveMeeting(User u, Meeting m)
        {
            u.LeaveMeeting();
            m.EndMeeting();
        }

        public void LeaveTeam(User u, Team t)
        {
            u.LeaveMeeting();   // ako je na sastanku
            u.LeaveTeam(t);
            t.RemoveMember(u);
        }
    }
}