using System.Reflection;
using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class MeetingMapper
{
    static void SetPrivate<TObj, TVal>(TObj obj, string fieldName, TVal value)
    {
        var f = typeof(TObj).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        f?.SetValue(obj, value);
    }

    public static Meeting ToDomain(MeetingEntity e)
    {
        var meeting = Meeting.Create(e.TeamId, e.OrganizedById);
        SetPrivate(meeting, "<Id>k__BackingField", e.Id);

        // Board
        if (e.Board is DiagramEntity de)
        {
            var board = DiagramMapper.FromDiagram(de, new Diagram()); // temp parent
            SetPrivate(meeting, "<Board>k__BackingField", board);
        }

        // Chat + messages
        var chat = ChatMapper.ToDomain(e.Chat);
        SetPrivate(meeting, "<_chat>k__BackingField", chat);

        // participants
        foreach (var p in e.Participants.Where(p => p.TeamMemberId != e.OrganizedById))
            meeting.Join(p.TeamMemberId);
        foreach (var p in meeting.Participants)
        {
            var src = e.Participants.First(x => x.TeamMemberId == p.UserId);
            if (src.CanDraw) p.SetDraw(true);
        }

        return meeting;
    }

    public static MeetingEntity ToEntity(Meeting d)
    {
        var board = DiagramMapper.ToEntity(d.Board);
        var chat = ChatMapper.ToEntity(d.Id, d.Chat);

        var entity = new MeetingEntity
        {
            Id = d.Id,
            TeamId = d.TeamId,
            OrganizedById = d.OrganizedBy,
            BoardId = board.Id,
            Board = board,
            ChatId = chat.Id,
            Chat = chat,
            IsActive = true,
            StartingTime = DateTime.UtcNow
        };

        entity.Participants = d.Participants.Select(p => MeetingParticipantMapper.ToEntity(d.Id, p)).ToList();
        return entity;
    }
}
