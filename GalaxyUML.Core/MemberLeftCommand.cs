namespace GalaxyUML.Core
{
    class MemberLeftCommand : ITeamCommand
    {
        public TeamMember Member { get; private set; }
        public List<User> Members { get; private set; }

        public MemberLeftCommand(TeamMember member, List<User> members)
        {
            Member = member;
            Members = members;
        }

        public void execute()
        {
            foreach(User m in Members)
                m.LeaveTeam();
        }
    }
}