using TeamEntity = GalaxyUML.Data.Entities.TeamEntity;
using Meeting = GalaxyUML.Core.Models.Meeting;
using MeetingEntity = GalaxyUML.Data.Entities.MeetingEntity;
using MeetingParticipantEntity = GalaxyUML.Data.Entities.MeetingParticipantEntity;

namespace GalaxyUML.Data.Mappers
{
    static class MeetingMapper
    {
        public static Meeting ToModel(MeetingEntity entity)
        {
            return new Meeting(
                entity.IdTeam,
                //entity.Id,
                entity.IdOrganizer,
                entity.IdChat,
                entity.IdBoard,
                MeetingParticipantMapper.ToModel(entity.Organizer),
                //MeetingParticipantMapper.ToModel(entity.Organizer).TeamMember,
                DiagramMapper.ToModel(entity.Board),
                ChatMapper.ToModel(entity.Chat)
            );
        }

        public static MeetingEntity ToEntity(Meeting model/*, TeamEntity teamEntity*/)
        {
            //List<MeetingParticipantEntity> participantEntities = new List<MeetingParticipantEntity>();
            //foreach (var p in model.Participants)
                //participantEntities.Add(MeetingParticipantMapper.ToEntity(p/*, teamEntity*/));

            return new MeetingEntity
            {
                //Id = model.IdMeeting,
                StartingTime = model.StartingTime,
                EndingTime = model.EndingTime,
                IdOrganizer = model.IdOrganizer,
                //Organizer = MeetingParticipantMapper.ToEntity(model.Organizer/*, teamEntity*/),
                IdTeam = model.IdTeam,
                // IdTeam = teamEntity.Id,
                //Team = teamEntity,
                //Participants = participantEntities,
                IdChat = model.IdChat,
                //Chat = ChatMapper.ToEntity(model.Chat/*, teamEntity*/),
                IdBoard = model.IdBoard,
                //Board = DiagramMapper.ToEntity(model.Board/*, null/*, teamEntity*/),  // board nema parent-a!
                IsActive = model.IsActive
            };
        }
    }
}