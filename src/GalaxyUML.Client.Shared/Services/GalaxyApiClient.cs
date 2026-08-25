using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using GalaxyUML.Client.Shared.Models;

namespace GalaxyUML.Client.Shared.Services;

public class GalaxyApiClient
{
    private string _baseUrl = "http://localhost:5248";
    private string? _jwtToken;

    public string BaseUrl
    {
        get => _baseUrl;
        set => _baseUrl = value.Trim().TrimEnd('/');
    }

    public string? JwtToken
    {
        get => _jwtToken;
        set => _jwtToken = value;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private HttpClient CreateClient()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl, UriKind.Absolute)
        };

        if (!string.IsNullOrWhiteSpace(_jwtToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        }

        return client;
    }

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var body = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("error", out var err))
                {
                    throw new InvalidOperationException(err.GetString());
                }
            }
            catch (JsonException) { }

            throw new InvalidOperationException($"API error ({(int)response.StatusCode}): {body}");
        }

        throw new InvalidOperationException($"API error ({(int)response.StatusCode}): {response.ReasonPhrase}");
    }

    #region Auth

    public async Task RegisterAsync(string firstName, string lastName, string username, string email, string password)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName,
            lastName,
            username,
            email,
            password
        });
        await EnsureSuccess(response);
    }

    public async Task<LoginResult> LoginAsync(string username, string password)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
        await EnsureSuccess(response);

        var data = await response.Content.ReadFromJsonAsync<LoginResponseDto>(JsonOptions);
        if (data is null) throw new InvalidOperationException("Prazan odgovor sa servera.");

        _jwtToken = data.Token;
        return new LoginResult(data.Token, data.User.IdUser, data.User.Username, data.User.Email);
    }

    #endregion

    #region Teams

    public async Task<IReadOnlyCollection<TeamItem>> GetUserTeamsAsync()
    {
        using var client = CreateClient();
        using var response = await client.GetAsync("/api/teams/me");
        await EnsureSuccess(response);

        var teams = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TeamResponseDto>>(JsonOptions);
        return teams?.Select(t => new TeamItem(t.Id, t.TeamName, t.TeamCode, t.OwnerId, t.MemberCount)).ToList() ?? [];
    }

    public async Task<TeamItem> CreateTeamAsync(string teamName)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync("/api/teams", new { teamName });
        await EnsureSuccess(response);

        var t = await response.Content.ReadFromJsonAsync<TeamResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Prazan odgovor sa servera.");
        return new TeamItem(t.Id, t.TeamName, t.TeamCode, t.OwnerId, t.MemberCount);
    }

    public async Task<TeamItem> FindTeamByCodeAsync(string joinCode)
    {
        using var client = CreateClient();
        using var response = await client.GetAsync($"/api/teams/by-code/{joinCode.Trim().ToUpperInvariant()}");
        await EnsureSuccess(response);

        var t = await response.Content.ReadFromJsonAsync<TeamResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Prazan odgovor sa servera.");
        return new TeamItem(t.Id, t.TeamName, t.TeamCode, t.OwnerId, t.MemberCount);
    }

    public async Task<TeamItem> JoinTeamByCodeAsync(string joinCode)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync("/api/teams/join-by-code", new { joinCode = joinCode.Trim().ToUpperInvariant() });
        await EnsureSuccess(response);

        var t = await response.Content.ReadFromJsonAsync<TeamResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Prazan odgovor sa servera.");
        return new TeamItem(t.Id, t.TeamName, t.TeamCode, t.OwnerId, t.MemberCount);
    }

    public async Task<IReadOnlyCollection<TeamMemberItem>> GetTeamMembersAsync(Guid teamId)
    {
        using var client = CreateClient();
        using var response = await client.GetAsync($"/api/teams/{teamId}/members");
        await EnsureSuccess(response);

        var members = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TeamMemberResponseDto>>(JsonOptions);
        return members?.Select(m => new TeamMemberItem(m.UserId, m.Username, m.Email, m.FirstName, m.LastName, m.Role, m.JoinedAt)).ToList() ?? [];
    }

    public async Task ChangeRoleAsync(Guid teamId, Guid targetUserId, string role)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/teams/{teamId}/role", new { targetUserId, role });
        await EnsureSuccess(response);
    }

    public async Task BanMemberAsync(Guid teamId, Guid targetUserId, string? reason)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/teams/{teamId}/ban", new { targetUserId, reason });
        await EnsureSuccess(response);
    }

    public async Task LeaveTeamAsync(Guid teamId)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/teams/{teamId}/leave", new { });
        await EnsureSuccess(response);
    }

    public async Task DeleteTeamAsync(Guid teamId)
    {
        using var client = CreateClient();
        using var response = await client.DeleteAsync($"/api/teams/{teamId}");
        await EnsureSuccess(response);
    }

    #endregion

    #region Meetings

    public async Task<MeetingItem?> GetActiveMeetingForTeamAsync(Guid teamId)
    {
        using var client = CreateClient();
        using var response = await client.GetAsync($"/api/meetings/by-team/{teamId}/active");
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent || response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        await EnsureSuccess(response);

        var m = await response.Content.ReadFromJsonAsync<MeetingStartedResponseDto>(JsonOptions);
        return m is null ? null : new MeetingItem(m.MeetingId, m.TeamId, m.BoardId, m.StartedAtUtc);
    }

    public async Task<MeetingItem> CreateMeetingAsync(Guid teamId)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync("/api/meetings", new { teamId });
        await EnsureSuccess(response);

        var m = await response.Content.ReadFromJsonAsync<MeetingStartedResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Prazan odgovor sa servera.");
        return new MeetingItem(m.MeetingId, m.TeamId, m.BoardId, m.StartedAtUtc);
    }

    public async Task JoinMeetingAsync(Guid meetingId)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/meetings/{meetingId}/join", new { });
        await EnsureSuccess(response);
    }

    public async Task LeaveMeetingAsync(Guid meetingId)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/meetings/{meetingId}/leave", new { });
        await EnsureSuccess(response);
    }

    public async Task EndMeetingAsync(Guid meetingId)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/meetings/{meetingId}/end", new { });
        await EnsureSuccess(response);
    }

    public async Task<IReadOnlyCollection<MeetingParticipantItem>> GetParticipantsAsync(Guid meetingId)
    {
        using var client = CreateClient();
        using var response = await client.GetAsync($"/api/meetings/{meetingId}/participants");
        await EnsureSuccess(response);

        var parts = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<MeetingParticipantResponseDto>>(JsonOptions);
        return parts?.Select(p => new MeetingParticipantItem(p.UserId, p.Username, p.Role, p.CanDraw, p.JoinedAtUtc)).ToList() ?? [];
    }

    public async Task GrantDrawAsync(Guid meetingId, Guid targetUserId, bool canDraw)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/meetings/{meetingId}/grant-draw", new { targetId = targetUserId, canDraw });
        await EnsureSuccess(response);
    }

    #endregion

    #region Chat

    public async Task<IReadOnlyCollection<ChatMessageItem>> GetMessagesAsync(Guid meetingId)
    {
        using var client = CreateClient();
        using var response = await client.GetAsync($"/api/meetings/{meetingId}/messages");
        await EnsureSuccess(response);

        var msgs = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ChatMessageResponseDto>>(JsonOptions);
        return msgs?.Select(m => new ChatMessageItem(m.Id, m.SenderId, m.SenderUsername, m.Content, m.SentAtUtc)).ToList() ?? [];
    }

    public async Task SendMessageAsync(Guid meetingId, string content)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/meetings/{meetingId}/message", new { content });
        await EnsureSuccess(response);
    }

    #endregion

    #region Diagram & Board

    public async Task<BoardElementsResult> GetElementsAsync(Guid boardId)
    {
        using var client = CreateClient();
        using var response = await client.GetAsync($"/api/diagram/{boardId}/elements");
        await EnsureSuccess(response);

        var data = await response.Content.ReadFromJsonAsync<BoardElementsResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Prazan odgovor sa servera.");

        var boxes = new List<BoxItem>();
        if (data.ClassBoxes is not null)
        {
            foreach (var cb in data.ClassBoxes)
            {
                boxes.Add(new BoxItem(cb.Id, cb.X1, cb.Y1, cb.X2, cb.Y2, "ClassBox", cb.Attributes ?? [], cb.Methods ?? []));
            }
        }
        if (data.Boxes is not null)
        {
            foreach (var b in data.Boxes)
            {
                boxes.Add(new BoxItem(b.Id, b.X1, b.Y1, b.X2, b.Y2, "Box", [], []));
            }
        }

        var lines = data.Lines?.Select(l => new LineItem(l.Id, l.StartBoxId, l.EndBoxId, l.MiddleText)).ToList() ?? [];
        return new BoardElementsResult(boxes, lines);
    }

    public async Task<Guid> AddClassBoxAsync(Guid boardId, int x1, int y1, int x2, int y2, IReadOnlyCollection<string> attributes, IReadOnlyCollection<string> methods)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/diagram/{boardId}/class-box", new
        {
            x1,
            y1,
            x2,
            y2,
            attributes,
            methods
        });
        await EnsureSuccess(response);

        var data = await response.Content.ReadFromJsonAsync<ElementCreateResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Prazan odgovor sa servera.");
        return data.Id;
    }

    public async Task<Guid> AddLineAsync(Guid boardId, Guid startBoxId, Guid endBoxId, string? middleText)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/diagram/{boardId}/line", new
        {
            startBoxId,
            endBoxId,
            middleText = string.IsNullOrWhiteSpace(middleText) ? null : middleText,
            text1 = (string?)null,
            text2 = (string?)null
        });
        await EnsureSuccess(response);

        var data = await response.Content.ReadFromJsonAsync<ElementCreateResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Prazan odgovor sa servera.");
        return data.Id;
    }

    public async Task MoveElementAsync(Guid elementId, int dx, int dy)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/diagram/{elementId}/move", new { dx, dy });
        await EnsureSuccess(response);
    }

    public async Task DeleteElementAsync(Guid elementId)
    {
        using var client = CreateClient();
        using var response = await client.DeleteAsync($"/api/diagram/{elementId}");
        await EnsureSuccess(response);
    }

    public async Task ClearBoardAsync(Guid boardId)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"/api/diagram/{boardId}/clear", new { });
        await EnsureSuccess(response);
    }

    #endregion

    #region Private DTOs

    private sealed record LoginResponseDto(string Token, LoginUserResponseDto User);
    private sealed record LoginUserResponseDto(Guid IdUser, string Username, string Email);
    private sealed record TeamResponseDto(Guid Id, string TeamName, string TeamCode, Guid OwnerId, int MemberCount);
    private sealed record TeamMemberResponseDto(Guid UserId, string Username, string Email, string FirstName, string LastName, string Role, DateTime JoinedAt);
    private sealed record MeetingStartedResponseDto(Guid MeetingId, Guid BoardId, Guid TeamId, DateTime StartedAtUtc);
    private sealed record MeetingParticipantResponseDto(Guid UserId, string Username, string Role, bool CanDraw, DateTime JoinedAtUtc);
    private sealed record ChatMessageResponseDto(Guid Id, Guid SenderId, string SenderUsername, string Content, DateTime SentAtUtc);
    private sealed record ElementCreateResponseDto(Guid Id);

    private sealed record BoardElementsResponseDto(
        IReadOnlyCollection<BoxDataDto> Boxes,
        IReadOnlyCollection<ClassBoxDataDto> ClassBoxes,
        IReadOnlyCollection<TextDataDto> Texts,
        IReadOnlyCollection<LineDataDto> Lines);

    private sealed record BoxDataDto(Guid Id, int X1, int Y1, int X2, int Y2);
    private sealed record ClassBoxDataDto(Guid Id, int X1, int Y1, int X2, int Y2, IReadOnlyCollection<string>? Attributes, IReadOnlyCollection<string>? Methods);
    private sealed record TextDataDto(Guid Id, int X1, int Y1, int X2, int Y2, string Content, int FontSize, string Color, string? Format);
    private sealed record LineDataDto(Guid Id, Guid StartBoxId, Guid EndBoxId, double X1, double Y1, double X2, double Y2, string? MiddleText, string? Text1, string? Text2);

    #endregion
}
