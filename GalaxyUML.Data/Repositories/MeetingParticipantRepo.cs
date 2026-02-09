using Microsoft.EntityFrameworkCore;
using MeetingParticipant = GalaxyUML.Core.Models.MeetingParticipant;
using MeetingParticipantMapper = GalaxyUML.Data.Mappers.MeetingParticipantMapper;

namespace GalaxyUML.Data.Repositories
{
    class MeetingParticipantRepo : IMeetingParticipantRepo
    {
        AppDbContext _context;

        public MeetingParticipantRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(MeetingParticipant participant)
        {
            if (await _context.Participants.AnyAsync(p => p.Id == participant.IdMeetingParticipant))
                throw new Exception("Participant with this id already exists.");

            var entity = MeetingParticipantMapper.ToEntity(participant);
            await _context.Participants.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MeetingParticipant participant)
        {
            var entity = await _context.Participants.FirstOrDefaultAsync(p => p.Id == participant.IdMeetingParticipant);
            if (entity == null)
                throw new Exception("Participants with this id doesn't exist.");

            _context.Participants.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MeetingParticipant>> GetAllAsync()
        {
            return await _context.Participants
                            .AsNoTracking()
                            .Select(p => MeetingParticipantMapper.ToModel(p))
                            .ToListAsync();
        }

        public async Task<MeetingParticipant?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Participants.FirstOrDefaultAsync(p => p.Id == id);
            return entity == null ? null : MeetingParticipantMapper.ToModel(entity);
        }

        public async Task<IEnumerable<MeetingParticipant>> GetByMeetingAsync(Guid idMeeting)
        {
            return await _context.Participants
                            .AsNoTracking()
                            .Where(p => p.IdMeeting == idMeeting)
                            .Select(p => MeetingParticipantMapper.ToModel(p))
                            .ToListAsync();
        }

        public async Task<IEnumerable<MeetingParticipant>> GetByParticipantAsync(Guid idParticipant)
        {
            return await _context.Participants
                            .AsNoTracking()
                            .Where(p => p.IdParticipant == idParticipant)
                            .Select(p => MeetingParticipantMapper.ToModel(p))
                            .ToListAsync();
        }

        public async Task UpdateAsync(MeetingParticipant participant)
        {
            var entity = await _context.Participants.FirstOrDefaultAsync(p => p.Id == participant.IdMeetingParticipant);
            if (entity == null)
                throw new Exception("Participants with this id doesn't exist.");

            _context.Participants.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}