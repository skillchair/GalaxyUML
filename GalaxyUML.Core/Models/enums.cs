namespace GalaxyUML.Core.Models
{
    public enum RoleEnum
    {
        Member,
        Organizer,
        Owner
    }

    public enum MemberStatus
    {
        Valid,
        Banned
    }

    public enum ObjectType
    {
        Diagram,
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
        MeetingStarted,
        MeetingEnded,
        MemberJoined,
        ParticipantLeft,
        // DrawableAdded,
        // DrawableRemoved,
        // DrawableEdited,
        ObjectAdded,
        ObjectRemoved,
        ObjectResized,
        ObjectEditted,
        MessageSent
    }
}