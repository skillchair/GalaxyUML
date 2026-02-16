using GalaxyUML.Core.Models;
using GalaxyUML.Data.Repositories;
using GalaxyUML.Data.Entities;
using Microsoft.EntityFrameworkCore;
using GalaxyUML.Data;

namespace GalaxyUML.Core.Services;

public class TeamService
{
    private readonly ITeamRepo _teams;
    private readonly IUserRepo _users;
    private readonly AppDbContext _db;

    public TeamService(ITeamRepo teams, IUserRepo users, AppDbContext db)
    {
        _teams = teams;
        _users = users;
        _db = db;
    }

    public async Task<TeamSummaryDto> CreateAsync(string name, Guid ownerId)
    {
        _ = await _users.GetByIdAsync(ownerId) ?? throw new InvalidOperationException("Owner not found");
        var team = Team.Create(name, ownerId);
        await _teams.AddAsync(team);
        return new TeamSummaryDto(team.Id, team.TeamName, team.TeamCode, team.OwnerId, team.Members.Count);
    }

    public async Task JoinAsync(Guid teamId, Guid userId, string joinCode)
    {
        var team = await _db.Teams
            .FirstOrDefaultAsync(t => t.Id == teamId)
            ?? throw new InvalidOperationException("Team not found");

        await JoinInternalAsync(team, userId, joinCode);
    }

    public async Task<TeamSummaryDto> JoinByCodeAsync(Guid userId, string joinCode)
    {
        var normalizedCode = NormalizeJoinCode(joinCode);
        var team = await _db.Teams
            .FirstOrDefaultAsync(t => t.TeamCode == normalizedCode)
            ?? throw new InvalidOperationException("Team not found");

        await JoinInternalAsync(team, userId, normalizedCode);
        var membersCount = await _db.TeamMembers.CountAsync(m => m.TeamId == team.Id);
        return new TeamSummaryDto(team.Id, team.TeamName, team.TeamCode, team.OwnerId, membersCount);
    }

    public async Task<TeamSummaryDto?> FindByCodeAsync(string joinCode)
    {
        var normalizedCode = NormalizeJoinCode(joinCode);
        var team = await _db.Teams
            .Include(t => t.Members)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TeamCode == normalizedCode);

        if (team is null)
        {
            return null;
        }

        return new TeamSummaryDto(team.Id, team.TeamName, team.TeamCode, team.OwnerId, team.Members.Count);
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

    private async Task JoinInternalAsync(TeamEntity team, Guid userId, string joinCode)
    {
        _ = await _users.GetByIdAsync(userId) ?? throw new InvalidOperationException("User not found");

        if (!string.Equals(team.TeamCode, joinCode, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Bad join code");

        var isBanned = await _db.BannedUsers.AnyAsync(b => b.TeamId == team.Id && b.UserId == userId);
        if (isBanned)
            throw new InvalidOperationException("User banned");

        var alreadyMember = await _db.TeamMembers.AnyAsync(m => m.TeamId == team.Id && m.UserId == userId);
        if (alreadyMember)
            return;

        _db.TeamMembers.Add(new TeamMemberEntity
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            UserId = userId,
            Role = RoleEnum.Member,
            JoinedAt = DateTime.UtcNow
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var existsAfterConflict = await _db.TeamMembers.AnyAsync(m => m.TeamId == team.Id && m.UserId == userId);
            if (!existsAfterConflict)
                throw;
        }
    }

    private static string NormalizeJoinCode(string joinCode)
    {
        var normalized = joinCode.Trim().ToUpperInvariant();
        if (normalized.Length != 6)
            throw new InvalidOperationException("Join code must have 6 characters");
        return normalized;
    }
}

public record TeamSummaryDto(Guid Id, string TeamName, string TeamCode, Guid OwnerId, int MemberCount);
