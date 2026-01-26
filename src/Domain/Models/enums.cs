namespace GalaxyUML.Models
{
    public enum RoleEnum
    {
        Member,
        Organizer,
        Owner
    }

    public enum TeamResult
    {
        Success,
        InvalidTeamCode,
        Error
    }

    public enum MemberResult
    {
        Success,
        AlreadyAMember,
        NotAMember,
        Error
    }

    public enum MeetingResult
    {
        Success,
        AlreadyInMeet,
        AlreadyOrganized,
        Error
    }

    public enum DrawableType
    {
        Text,
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
}