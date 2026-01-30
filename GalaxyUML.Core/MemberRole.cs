namespace GalaxyUML.Core
{
    class MemberRole
    {
        public IRole Role { get; private set; }
        public TeamMember Member { get; private set; }

        public MemberRole(IRole role, TeamMember member)
        {
            Role = role;
            Member = member;
        }
    }
}