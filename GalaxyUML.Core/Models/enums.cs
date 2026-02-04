namespace GalaxyUML.Core.Models
{
    public enum RoleEnum
    {
        Member,
        Organizer,
        Owner
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
        MemberJoined,
        ParticipantLeft,
        // DrawableAdded,
        // DrawableRemoved,
        // DrawableEdited,
        ObjectAdded,
        ObjectRemoved,
        ObjectEditted,
        MessageSent
    }
}