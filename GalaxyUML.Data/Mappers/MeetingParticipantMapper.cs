using MeetingParticipant = GalaxyUML.Core.Models.MeetingParticipant;
using MeetingParticipantEntity = GalaxyUML.Data.Entities.MeetingParticipantEntity;

namespace GalaxyUML.Data.Mappers
{
    static class MeetingParticipantMapper
    {
        public static MeetingParticipant ToModel(MeetingParticipantEntity entity)
        {
            return new MeetingParticipant(
                MeetingMapper.ToModel(entity.Meeting),
                TeamMemberMapper.ToModel(entity.Participant)
            );
        }

        public static MeetingParticipantEntity ToEntity(MeetingParticipant model)
        {
            return new MeetingParticipantEntity
            {
                Id = model.IdMeetingParticipant,
                IdMeeting = model.Meeting.IdMeeting,
                Meeting = MeetingMapper.ToEntity(model.Meeting),
                IdParticipant = model.TeamMember.IdTeamMember,
                Participant = TeamMemberMapper.ToEntity(model.TeamMember)
            };
        }
    }
}