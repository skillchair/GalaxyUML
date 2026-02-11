using GalaxyUML.Data.Entities;
using MeetingParticipant = GalaxyUML.Core.Models.MeetingParticipant;
using MeetingParticipantEntity = GalaxyUML.Data.Entities.MeetingParticipantEntity;

namespace GalaxyUML.Data.Mappers
{
    static class MeetingParticipantMapper
    {
        public static MeetingParticipant ToModel(MeetingParticipantEntity entity)
        {
            return new MeetingParticipant(
                entity.IdMeeting,
                entity.IdParticipant,
                MeetingMapper.ToModel(entity.Meeting),
                TeamMemberMapper.ToModel(entity.Participant)
            );
        }

        public static MeetingParticipantEntity ToEntity(MeetingParticipant model/*, TeamEntity teamEntity*/)
        {
            return new MeetingParticipantEntity
            {
                //Id = model.IdMeetingParticipant,
                IdMeeting = model.IdMeeting,
                Meeting = MeetingMapper.ToEntity(model.Meeting/*, teamEntity*/),
                IdParticipant = model.IdParticipant,
                Participant = TeamMemberMapper.ToEntity(model.TeamMember)
            };
        }
    }
}