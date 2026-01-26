namespace GalaxyUML.Models
{
    public abstract class IRole
    {
        public RoleEnum Role { get; set; }

        public IRole(RoleEnum role)
        {
            Role = role;
        }

        public void RaiseHand()
        {
            //
        }

        public void Draw(IDrawable obj, int x1, int y1, int x2, int y2)
        {
            obj.Draw(x1, y1, x2, y2);
        }

        public bool ReleaseBoard(Meeting m)
        {
            //
        }

        public bool LeaveMeeting(Meeting m)
        {
            //
        }

        public bool LeaveTeam(Team t)
        {
            //
        }
    }
}