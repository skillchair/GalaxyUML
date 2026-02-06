namespace GalaxyUML.Core.Models.Commands.TeamCommands
{
    class RoleChangeCommand : ITeamCommand
    {
        public TeamMember Member { get; private set; }
        public RoleEnum NewRole { get; private set; }

        public RoleChangeCommand(Team team, TeamMember member, RoleEnum newRole) : base(team)
        {
            Member = member;
            NewRole = newRole;
        }
        
        public override void Execute(TeamEventType eventType)
        {
            if (eventType != TeamEventType.MemberRoleChange)
                throw new Exception("Invalid event. Expected TeamEventType.MemberRoleChange.");

            Member.ChangeRole(NewRole);
        }
    }
}