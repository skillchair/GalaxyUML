using GalaxyUML.Core.Models;
using GalaxyUML.Data.Repositories;
using GalaxyUML.Data;
using GalaxyUML.Data.Entities;
using Microsoft.EntityFrameworkCore;
using GalaxyUML.Data.Mappers;

namespace GalaxyUML.Core.Services;

public class MeetingService
{
    private readonly IMeetingRepo _meetings;
    private readonly AppDbContext _db;

    public MeetingService(IMeetingRepo meetings, AppDbContext db)
    {
        _meetings = meetings;
        _db = db;
    }

    public async Task<MeetingStartedDto> CreateAsync(Guid teamId, Guid organizerId)
    {
        var team = await _db.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == teamId)
            ?? throw new InvalidOperationException("Team not found");

        if (team.OwnerId != organizerId)
            throw new InvalidOperationException("Only team owner can start meeting");

        if (team.CurrentMeetingId is not null)
            throw new InvalidOperationException("Meeting already active");

        var organizerMember = team.Members.FirstOrDefault(m => m.UserId == organizerId)
            ?? throw new InvalidOperationException("Organizer is not a team member");

        var meeting = Meeting.Create(teamId, organizerId);
        team.CurrentMeetingId = meeting.Id;

        await _meetings.AddAsync(meeting);

        var participantExists = await _db.MeetingParticipants
            .AnyAsync(p => p.MeetingId == meeting.Id && p.TeamMemberId == organizerMember.Id);

        if (!participantExists)
        {
            _db.MeetingParticipants.Add(new MeetingParticipantEntity
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                TeamMemberId = organizerMember.Id,
                CanDraw = true,
                JoinedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        return new MeetingStartedDto(meeting.Id, meeting.Board.Id, teamId, DateTime.UtcNow);
    }

    public async Task JoinAsync(Guid meetingId, Guid userId)
    {
        _ = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new InvalidOperationException("User not found");
        var meeting = await _db.Meetings.FirstOrDefaultAsync(m => m.Id == meetingId) ?? throw new InvalidOperationException("Meeting not found");
        if (!meeting.IsActive) throw new InvalidOperationException("Meeting is not active");

        var teamMember = await _db.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == meeting.TeamId && tm.UserId == userId)
            ?? throw new InvalidOperationException("User must be a team member to join meeting");

        var exists = await _db.MeetingParticipants.AnyAsync(p => p.MeetingId == meetingId && p.TeamMemberId == teamMember.Id);
        if (exists)
            return;

        _db.MeetingParticipants.Add(new MeetingParticipantEntity
        {
            Id = Guid.NewGuid(),
            MeetingId = meetingId,
            TeamMemberId = teamMember.Id,
            CanDraw = false,
            JoinedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task LeaveAsync(Guid meetingId, Guid userId)
    {
        var meeting = await _db.Meetings.FirstOrDefaultAsync(m => m.Id == meetingId) ?? throw new InvalidOperationException("Meeting not found");
        var teamMember = await _db.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == meeting.TeamId && tm.UserId == userId)
            ?? throw new InvalidOperationException("User is not a team member");

        var participant = await _db.MeetingParticipants
            .FirstOrDefaultAsync(p => p.MeetingId == meetingId && p.TeamMemberId == teamMember.Id);

        if (participant is null)
            return;

        var hadDraw = participant.CanDraw;
        _db.MeetingParticipants.Remove(participant);

        if (hadDraw)
        {
            var organizer = await _db.MeetingParticipants
                .FirstOrDefaultAsync(p => p.MeetingId == meetingId && p.TeamMemberId == meeting.OrganizedById);
            if (organizer is not null)
            {
                organizer.CanDraw = true;
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task GrantDrawAsync(Guid meetingId, Guid actorId, Guid targetId, bool canDraw)
    {
        var meeting = await _db.Meetings
            .Include(m => m.Team)
            .FirstOrDefaultAsync(m => m.Id == meetingId)
            ?? throw new InvalidOperationException("Meeting not found");

        if (meeting.OrganizedById != actorId && meeting.Team.OwnerId != actorId)
            throw new InvalidOperationException("Only organizer or team owner can grant drawing rights");

        var participant = await _db.MeetingParticipants
            .Include(p => p.TeamMember)
            .FirstOrDefaultAsync(p => p.MeetingId == meetingId && p.TeamMember.UserId == targetId)
            ?? throw new InvalidOperationException("Participant not found");

        participant.CanDraw = canDraw;
        await _db.SaveChangesAsync();
    }

    public async Task<ChatMessageDto> AddMessageAsync(Guid meetingId, Guid senderId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("Message content cannot be empty");

        var meeting = await _db.Meetings
            .Include(m => m.Chat)
            .FirstOrDefaultAsync(m => m.Id == meetingId)
            ?? throw new InvalidOperationException("Meeting not found");

        if (!meeting.IsActive)
            throw new InvalidOperationException("Meeting is not active");

        var senderParticipant = await _db.MeetingParticipants
            .Include(p => p.TeamMember.User)
            .FirstOrDefaultAsync(p => p.MeetingId == meetingId && p.TeamMember.UserId == senderId)
            ?? throw new InvalidOperationException("Only meeting participants can send messages");

        var messageEntity = new MessageEntity
        {
            Id = Guid.NewGuid(),
            ChatId = meeting.ChatId,
            SenderId = senderId,
            Content = content.Trim(),
            SentAt = DateTime.UtcNow
        };

        _db.Messages.Add(messageEntity);
        await _db.SaveChangesAsync();

        return new ChatMessageDto(
            messageEntity.Id,
            senderId,
            senderParticipant.TeamMember.User.Username,
            messageEntity.Content,
            messageEntity.SentAt);
    }

    public async Task<IReadOnlyCollection<ChatMessageDto>> GetMessagesAsync(Guid meetingId, Guid userId)
    {
        var meeting = await _db.Meetings
            .Include(m => m.Chat)
            .FirstOrDefaultAsync(m => m.Id == meetingId)
            ?? throw new InvalidOperationException("Meeting not found");

        var isParticipant = await _db.MeetingParticipants
            .AnyAsync(p => p.MeetingId == meetingId && p.TeamMember.UserId == userId);

        if (!isParticipant)
            throw new InvalidOperationException("Only meeting participants can view chat messages");

        return await _db.Messages
            .Where(m => m.ChatId == meeting.ChatId)
            .Include(m => m.Sender)
            .OrderBy(m => m.SentAt)
            .Select(m => new ChatMessageDto(
                m.Id,
                m.SenderId,
                m.Sender.Username,
                m.Content,
                m.SentAt))
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid meetingId, Guid actorUserId)
    {
        var meeting = await _db.Meetings
            .Include(m => m.Team)
            .FirstOrDefaultAsync(m => m.Id == meetingId)
            ?? throw new InvalidOperationException("Meeting not found");

        if (meeting.Team.OwnerId != actorUserId)
            throw new InvalidOperationException("Only team owner can delete meeting");

        if (meeting.Team.CurrentMeetingId == meetingId)
        {
            meeting.Team.CurrentMeetingId = null;
        }

        await _meetings.RemoveAsync(meetingId);
    }

    public async Task EndAsync(Guid meetingId, Guid actorUserId)
    {
        var meeting = await _db.Meetings.FirstOrDefaultAsync(m => m.Id == meetingId)
            ?? throw new InvalidOperationException("Meeting not found");

        var team = await _db.Teams.FirstOrDefaultAsync(t => t.Id == meeting.TeamId)
            ?? throw new InvalidOperationException("Team not found");

        if (team.OwnerId != actorUserId)
            throw new InvalidOperationException("Only team owner can end meeting");

        if (!meeting.IsActive)
            return;

        meeting.IsActive = false;
        meeting.EndingTime = DateTime.UtcNow;

        if (team.CurrentMeetingId == meetingId)
        {
            team.CurrentMeetingId = null;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<MeetingParticipantSummaryDto>> GetParticipantsAsync(Guid meetingId)
    {
        var meetingExists = await _db.Meetings.AnyAsync(m => m.Id == meetingId);
        if (!meetingExists)
            throw new InvalidOperationException("Meeting not found");

        return await _db.MeetingParticipants
            .Where(p => p.MeetingId == meetingId)
            .Include(p => p.TeamMember)
            .ThenInclude(tm => tm.User)
            .OrderBy(p => p.JoinedAt)
            .Select(p => new MeetingParticipantSummaryDto(
                p.TeamMember.UserId,
                p.TeamMember.User.Username,
                p.TeamMember.Role.ToString(),
                p.CanDraw,
                p.JoinedAt))
            .ToListAsync();
    }

    public async Task<MeetingStartedDto?> GetByTeamIfActiveAsync(Guid teamId)
    {
        var entity = await _db.Meetings
            .Include(m => m.Board)
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.IsActive);

        if (entity is null)
            return null;

        return new MeetingStartedDto(
            entity.Id,
            entity.Board.Id,
            entity.TeamId,
            entity.StartingTime
        );
    }
}

public record MeetingStartedDto(Guid MeetingId, Guid BoardId, Guid TeamId, DateTime StartedAtUtc);
public record MeetingParticipantSummaryDto(Guid UserId, string Username, string Role, bool CanDraw, DateTime JoinedAtUtc);
public record ChatMessageDto(Guid Id, Guid SenderId, string SenderUsername, string Content, DateTime SentAtUtc);
