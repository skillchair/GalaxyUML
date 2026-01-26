namespace GalaxyUML.Models
{
    public enum RoleEnum
    {
        Member,
        Organizer,
        Owner
    }

    public enum GetTeamResult
    {
        Success,
        InvalidTeamCode,
        Error
    }

    public enum GetMemberResult
    {
        Success,
        NotAMember,
        Error
    }

    public enum GetParticipantResult
    {
        Success,
        NotAParticipant,
        Error
    }

    public enum GetMeetingResult
    {
        Success,
        NotAMeeting,
        Error
    }

    public enum GetBoardResult
    {
        Success,
        NotABoard,
        Error
    }
    
    public enum GivePermissionResult
    {
        Success,
        PermissionAlreadyGiven,
        NotAParticipant,
        Error
    }

    public enum TakePermissionResult
    {
        Success,
        PermissionAlreadyTaken,
        NotAParticipant,
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