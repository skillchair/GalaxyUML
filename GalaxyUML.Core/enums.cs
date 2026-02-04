namespace GalaxyUML.Core
{
    public enum RoleEnum
    {
        Member,
        Organizer,
        Owner
    }

    public enum DrawableType
    {
        Text,
        Box,
        ClassBox,
        Line,
        Association,
        DirectedAssociation, 
        Aggregation,
        Composition,
        Dependency,
        Generalization,
        Realization
    }

    public enum TeamEventType
    {
        UserJoined,
        MemberLeft,
        MemberRoleChange,
        MeetingStarted,
        MeetingEnded
    }

    public enum MeetingEventType
    {
        MemberJoined,
        ParticipantLeft,
        // DrawableAdded,
        // DrawableRemoved,
        // DrawableEdited,
        DiagramAdded,
        DiagramRemoved,
        DiagramEditted,
        MessageSent
    }
}