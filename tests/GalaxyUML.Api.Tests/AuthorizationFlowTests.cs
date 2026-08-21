// verifies that tokens, not request payloads, determine the acting user.

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using Xunit;

namespace GalaxyUML.Api.Tests;

public sealed class AuthorizationFlowTests
{
    [Fact]
    public async Task Protected_endpoint_rejects_anonymous_requests()
    {
        using var factory = new GalaxyApiFactory();
        using var client = factory.CreateHttpsClient();

        using var response = await client.PostAsJsonAsync("/api/teams", new { teamName = "private team" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Team_owner_comes_from_token_even_when_old_owner_field_is_sent()
    {
        using var factory = new GalaxyApiFactory();
        var owner = await CreateAuthenticatedUserAsync(factory, "owner");
        var otherUser = await CreateAuthenticatedUserAsync(factory, "other");

        using var response = await owner.Client.PostAsJsonAsync("/api/teams", new
        {
            teamName = "token owned team",
            ownerId = otherUser.Id
        });
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TeamResponse>();

        Assert.NotNull(created);
        Assert.Equal(owner.Id, created.OwnerId);

        var ownerTeams = await owner.Client.GetFromJsonAsync<IReadOnlyCollection<TeamResponse>>("/api/teams/me");
        var otherTeams = await otherUser.Client.GetFromJsonAsync<IReadOnlyCollection<TeamResponse>>("/api/teams/me");
        Assert.Contains(ownerTeams!, team => team.Id == created.Id);
        Assert.Empty(otherTeams!);
    }

    [Fact]
    public async Task Non_owner_cannot_delete_team_by_supplying_the_owner_identifier()
    {
        using var factory = new GalaxyApiFactory();
        var owner = await CreateAuthenticatedUserAsync(factory, "owner");
        var otherUser = await CreateAuthenticatedUserAsync(factory, "other");
        var team = await CreateTeamAsync(owner, "owner only team");

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/teams/{team.Id}")
        {
            Content = JsonContent.Create(new { userId = owner.Id })
        };
        using var response = await otherUser.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var ownerTeams = await owner.Client.GetFromJsonAsync<IReadOnlyCollection<TeamResponse>>("/api/teams/me");
        Assert.Contains(ownerTeams!, existing => existing.Id == team.Id);
    }

    [Fact]
    public async Task Meeting_and_diagram_permissions_use_the_authenticated_user()
    {
        using var factory = new GalaxyApiFactory();
        var owner = await CreateAuthenticatedUserAsync(factory, "owner");
        var member = await CreateAuthenticatedUserAsync(factory, "member");
        var team = await CreateTeamAsync(owner, "diagram team");
        await JoinTeamAsync(member, team.TeamCode);

        using var rejectedMeetingResponse = await member.Client.PostAsJsonAsync("/api/meetings", new
        {
            teamId = team.Id,
            organizerId = owner.Id
        });
        Assert.Equal(HttpStatusCode.BadRequest, rejectedMeetingResponse.StatusCode);

        using var meetingResponse = await owner.Client.PostAsJsonAsync("/api/meetings", new
        {
            teamId = team.Id,
            organizerId = member.Id
        });
        meetingResponse.EnsureSuccessStatusCode();
        var meeting = await meetingResponse.Content.ReadFromJsonAsync<MeetingResponse>();
        Assert.NotNull(meeting);

        using var joinResponse = await member.Client.PostAsJsonAsync($"/api/meetings/{meeting.MeetingId}/join", new
        {
            userId = owner.Id
        });
        Assert.Equal(HttpStatusCode.NoContent, joinResponse.StatusCode);

        using var createBoxResponse = await owner.Client.PostAsJsonAsync($"/api/diagram/{meeting.BoardId}/class-box", new
        {
            userId = member.Id,
            x1 = 10,
            y1 = 20,
            x2 = 110,
            y2 = 120,
            attributes = Array.Empty<string>(),
            methods = Array.Empty<string>()
        });
        createBoxResponse.EnsureSuccessStatusCode();
        var box = await createBoxResponse.Content.ReadFromJsonAsync<ElementResponse>();
        Assert.NotNull(box);

        using var forbiddenResize = await member.Client.PostAsJsonAsync($"/api/diagram/{box.Id}/resize", new
        {
            userId = owner.Id,
            width = 150,
            height = 150
        });
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenResize.StatusCode);

        using var allowedResize = await owner.Client.PostAsJsonAsync($"/api/diagram/{box.Id}/resize", new
        {
            width = 150,
            height = 150
        });
        Assert.Equal(HttpStatusCode.NoContent, allowedResize.StatusCode);
    }

    [Fact]
    public async Task Signalr_group_requires_active_meeting_participation()
    {
        using var factory = new GalaxyApiFactory();
        var owner = await CreateAuthenticatedUserAsync(factory, "owner");
        var member = await CreateAuthenticatedUserAsync(factory, "member");
        var team = await CreateTeamAsync(owner, "realtime team");
        await JoinTeamAsync(member, team.TeamCode);
        var meeting = await CreateMeetingAsync(owner, team.Id);

        await using var ownerConnection = CreateHubConnection(factory, owner.Token);
        await ownerConnection.StartAsync();
        await ownerConnection.InvokeAsync("JoinMeeting", meeting.MeetingId);

        await using var memberConnection = CreateHubConnection(factory, member.Token);
        await memberConnection.StartAsync();
        await Assert.ThrowsAsync<HubException>(() => memberConnection.InvokeAsync("JoinMeeting", meeting.MeetingId));

        using var joinResponse = await member.Client.PostAsJsonAsync($"/api/meetings/{meeting.MeetingId}/join", new { });
        Assert.Equal(HttpStatusCode.NoContent, joinResponse.StatusCode);
        await memberConnection.InvokeAsync("JoinMeeting", meeting.MeetingId);
    }

    [Fact]
    public async Task Current_user_profile_does_not_expose_the_password_hash()
    {
        using var factory = new GalaxyApiFactory();
        var user = await CreateAuthenticatedUserAsync(factory, "profile");

        using var response = await user.Client.GetAsync("/api/users/me");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("password", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("hash", json, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<AuthenticatedUser> CreateAuthenticatedUserAsync(
        GalaxyApiFactory factory,
        string prefix)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var username = $"{prefix}-{suffix}";
        const string password = "correct horse battery staple";
        var client = factory.CreateHttpsClient();

        using var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = prefix,
            lastName = "tester",
            username,
            email = $"{username}@example.test",
            password
        });
        registerResponse.EnsureSuccessStatusCode();
        var id = await registerResponse.Content.ReadFromJsonAsync<Guid>();

        using var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
        loginResponse.EnsureSuccessStatusCode();
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);
        return new AuthenticatedUser(id, login.Token, client);
    }

    private static async Task<TeamResponse> CreateTeamAsync(AuthenticatedUser owner, string teamName)
    {
        using var response = await owner.Client.PostAsJsonAsync("/api/teams", new { teamName });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TeamResponse>())!;
    }

    private static async Task JoinTeamAsync(AuthenticatedUser member, string teamCode)
    {
        using var response = await member.Client.PostAsJsonAsync("/api/teams/join-by-code", new { joinCode = teamCode });
        response.EnsureSuccessStatusCode();
    }

    private static async Task<MeetingResponse> CreateMeetingAsync(AuthenticatedUser owner, Guid teamId)
    {
        using var response = await owner.Client.PostAsJsonAsync("/api/meetings", new { teamId });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MeetingResponse>())!;
    }

    private static HubConnection CreateHubConnection(GalaxyApiFactory factory, string token)
    {
        return new HubConnectionBuilder()
            .WithUrl("http://localhost/diagramHub", options =>
            {
                options.Transports = HttpTransportType.LongPolling;
                options.AccessTokenProvider = () => Task.FromResult(token)!;
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
            })
            .Build();
    }

    private sealed record AuthenticatedUser(Guid Id, string Token, HttpClient Client);
    private sealed record LoginResponse(string Token);
    private sealed record TeamResponse(Guid Id, string TeamName, string TeamCode, Guid OwnerId, int MemberCount);
    private sealed record MeetingResponse(Guid MeetingId, Guid BoardId, Guid TeamId, DateTime StartedAtUtc);
    private sealed record ElementResponse(Guid Id);
}
