using GalaxyUML.Core.Models;
using GalaxyUML.Data.Repositories;

namespace GalaxyUML.Core.Services;

public class TeamService
{
    private readonly ITeamRepo _teams;
    private readonly IUserRepo _users;

    public TeamService(ITeamRepo teams, IUserRepo users)
    {
        _teams = teams;
        _users = users;
    }

    public async Task<Guid> CreateAsync(string name, Guid ownerId)
    {
        _ = await _users.GetByIdAsync(ownerId) ?? throw new InvalidOperationException("Owner not found");
        var team = Team.Create(name, ownerId);
        await _teams.AddAsync(team);
        return team.Id;
    }

    public async Task JoinAsync(Guid teamId, Guid userId, string joinCode)
    {
        var team = await _teams.GetByIdAsync(teamId) ?? throw new InvalidOperationException("Team not found");
        team.Join(userId, joinCode);
        await _teams.SaveAsync();
    }

    public async Task LeaveAsync(Guid teamId, Guid userId)
    {
        var team = await _teams.GetByIdAsync(teamId) ?? throw new InvalidOperationException("Team not found");
        team.Leave(userId);
        await _teams.SaveAsync();
    }

    public async Task ChangeRoleAsync(Guid teamId, Guid actorId, Guid targetUserId, RoleEnum role)
    {
        var team = await _teams.GetByIdAsync(teamId) ?? throw new InvalidOperationException("Team not found");
        team.ChangeRole(actorId, targetUserId, role);
        await _teams.SaveAsync();
    }

    public async Task BanAsync(Guid teamId, Guid actorId, Guid targetUserId, string? reason = null)
    {
        var team = await _teams.GetByIdAsync(teamId) ?? throw new InvalidOperationException("Team not found");
        team.Ban(actorId, targetUserId, reason);
        await _teams.SaveAsync();
    }

    public async Task DeleteAsync(Guid teamId, Guid actorId)
    {
        var team = await _teams.GetByIdAsync(teamId) ?? throw new InvalidOperationException("Team not found");
        if (team.OwnerId != actorId) throw new InvalidOperationException("Only owner can delete team");
        if (team.CurrentMeetingId != null) throw new InvalidOperationException("Meeting in progress");
        await _teams.RemoveAsync(teamId);
    }
}
