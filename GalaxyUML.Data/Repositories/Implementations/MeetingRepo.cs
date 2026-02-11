using Microsoft.EntityFrameworkCore;
using Meeting = GalaxyUML.Core.Models.Meeting;
using MeetingMapper = GalaxyUML.Data.Mappers.MeetingMapper;
using Team = GalaxyUML.Core.Models.Team;
using TeamMapper = GalaxyUML.Data.Mappers.TeamMapper;

namespace GalaxyUML.Data.Repositories.Implementations
{
    class MeetingRepo : IMeetingRepo
    {
        private readonly AppDbContext _context;

        public MeetingRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Meeting meeting/*, Team team*/)
        {
            // var entityT = await _context.Teams.FindAsync(team);
            // if (entityT == null)
            //     throw new Exception("Team not found.");
                
            // if (await _context.Meetings.AnyAsync(m => m.Id == meeting.IdMeeting))
            //     throw new Exception("Meeting with this id already exists.");

            var entity = MeetingMapper.ToEntity(meeting/*, TeamMapper.ToEntity(team)*/);
            _context.Meetings.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Meetings.FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null)
                throw new Exception("Meeting with this id doesn't exist.");

            _context.Meetings.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Meeting>> GetByOrganizerAsync(Guid idOrganizer)
        {
            return await _context.Meetings
                            .AsNoTracking()
                            .Where(m => m.IdOrganizer == idOrganizer)
                            .Select(m => MeetingMapper.ToModel(m))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Meeting>> GetByTeamAsync(Guid idTeam)
        {
            return await _context.Meetings
                            .AsNoTracking()
                            .Where(m => m.IdTeam == idTeam)
                            .Select(m => MeetingMapper.ToModel(m))
                            .ToListAsync();
        }

        public async Task<Meeting?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Meetings.FirstOrDefaultAsync(m => m.Id == id);
            return entity == null ? null : MeetingMapper.ToModel(entity);
        }

        public async Task<Meeting?> GetByTeamIfActiveAsync(Guid idTeam)
        {
            return await _context.Meetings
                            .AsNoTracking()
                            .Where(m => m.IdTeam == idTeam && m.IsActive)
                            .Select(m => MeetingMapper.ToModel(m))
                            .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid id, Meeting meeting)
        {
            var entity = await _context.Meetings.FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null)
                throw new Exception("Meeting with this id doesn't exist");

            _context.Meetings.Update(MeetingMapper.ToEntity(meeting/*, null!*/)); // Primer, prilagodi svom mapperu
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Meeting>> GetAllAsync()
        {
            return await _context.Meetings
                            .AsNoTracking()
                            .Select(m => MeetingMapper.ToModel(m))
                            .ToListAsync();
        }
    }
}