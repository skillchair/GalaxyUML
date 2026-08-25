namespace GalaxyUML.Client.Shared.Models;

public record LoginResult(string Token, Guid UserId, string Username, string Email);

public record TeamItem(Guid Id, string TeamName, string TeamCode, Guid OwnerId, int MemberCount)
{
    public string Display => $"{TeamName} (Kod: {TeamCode})";
    public string MemberCountText => $"Članova: {MemberCount}";
}

public record TeamMemberItem(Guid UserId, string Username, string Email, string FirstName, string LastName, string Role, DateTime JoinedAt)
{
    public string FullName => $"{FirstName} {LastName}";
}

public record MeetingItem(Guid Id, Guid TeamId, Guid BoardId, DateTime StartedAtUtc)
{
    public string Display => $"Sastanak {Id.ToString()[..8]}... (Board: {BoardId.ToString()[..8]}...)";
    public string TimeText => $"Početak: {StartedAtUtc.ToLocalTime():HH:mm:ss}";
}

public record MeetingParticipantItem(Guid UserId, string Username, string Role, bool CanDraw, DateTime JoinedAtUtc)
{
    public bool CanDraw { get; set; } = CanDraw;
    public string DrawBadgeText => CanDraw ? "Crtanje: Dozvoljeno" : "Samo pregled";
}

public record ChatMessageItem(Guid Id, Guid SenderId, string SenderUsername, string Content, DateTime SentAtUtc)
{
    public string TimeText => SentAtUtc.ToLocalTime().ToString("HH:mm:ss");
}

public record BoxItem(Guid Id, int X1, int Y1, int X2, int Y2, string ClassName, IReadOnlyCollection<string> Attributes, IReadOnlyCollection<string> Methods)
{
    public int X1 { get; set; } = X1;
    public int Y1 { get; set; } = Y1;
    public int X2 { get; set; } = X2;
    public int Y2 { get; set; } = Y2;
    public string Display => $"{ClassName} ({X1},{Y1})";
}

public record LineItem(Guid Id, Guid StartBoxId, Guid EndBoxId, string? MiddleText);

public record BoardElementsResult(
    IReadOnlyCollection<BoxItem> Boxes,
    IReadOnlyCollection<LineItem> Lines);
