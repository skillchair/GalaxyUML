namespace GalaxyUML.Core.Models.DTOs;

public record CreateUserDto(string FirstName, string LastName, string Username, string Email, string Password);
public record UserDto(Guid Id, string FirstName, string LastName, string Username, string Email);

public record CreateTeamDto(string TeamName, Guid OwnerId);
public record TeamDto(Guid Id, string TeamName, string TeamCode, Guid OwnerId,
    IReadOnlyCollection<TeamMemberDto> Members, IReadOnlyCollection<BannedUserDto> Bans);
public record TeamMemberDto(Guid UserId, string Role, DateTime JoinedAt);
public record BannedUserDto(Guid UserId, DateTime BannedAt, string? Reason);

public record JoinTeamDto(Guid UserId, string JoinCode);
public record ChangeRoleDto(Guid ActorId, Guid TargetUserId, string Role);

public record CreateMeetingDto(Guid TeamId, Guid OrganizerId);
public record MeetingDto(Guid Id, Guid TeamId, Guid OrganizedBy, DiagramDto Board,
    IReadOnlyCollection<MeetingParticipantDto> Participants, ChatDto Chat);
public record MeetingParticipantDto(Guid UserId, bool CanDraw, DateTime JoinedAt);
public record GrantDrawDto(Guid ActorId, Guid TargetId, bool CanDraw);
public record LeaveMeetingDto(Guid UserId);
public record JoinMeetingDto(Guid UserId);

public record ChatDto(IReadOnlyCollection<MessageDto> Messages);
public record MessageDto(Guid Id, Guid SenderId, string Content, DateTime SentAt);

// Diagram DTOs (polymorphic)
public abstract record DiagramElementDto(Guid Id, string ObjectType,
    int X1, int Y1, int X2, int Y2, IReadOnlyCollection<DiagramElementDto> Children);
public record DiagramDto(Guid Id, int X1, int Y1, int X2, int Y2,
    IReadOnlyCollection<DiagramElementDto> Children)
    : DiagramElementDto(Id, "Diagram", X1, Y1, X2, Y2, Children);
public record TextDto(Guid Id, int X1, int Y1, int X2, int Y2, string Content, int FontSize, string Color, string? Format,
    IReadOnlyCollection<DiagramElementDto> Children)
    : DiagramElementDto(Id, "Text", X1, Y1, X2, Y2, Children);
public record BoxDto(Guid Id, int X1, int Y1, int X2, int Y2,
    IReadOnlyCollection<LineDto> Lines, IReadOnlyCollection<DiagramElementDto> Children)
    : DiagramElementDto(Id, "Box", X1, Y1, X2, Y2, Children);
public record ClassBoxDto(Guid Id, int X1, int Y1, int X2, int Y2,
    IReadOnlyCollection<string> Attributes, IReadOnlyCollection<string> Methods,
    IReadOnlyCollection<LineDto> Lines, IReadOnlyCollection<DiagramElementDto> Children)
    : DiagramElementDto(Id, "ClassBox", X1, Y1, X2, Y2, Children);
public record LineDto(Guid Id, int X1, int Y1, int X2, int Y2, Guid BoxId,
    string? MiddleText, string? Text1, string? Text2, IReadOnlyCollection<DiagramElementDto> Children)
    : DiagramElementDto(Id, "Line", X1, Y1, X2, Y2, Children);
