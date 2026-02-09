using Microsoft.EntityFrameworkCore;
using Meeting = GalaxyUML.Core.Models.Meeting;
using MeetingMapper = GalaxyUML.Data.Mappers.MeetingMapper;
using Team = GalaxyUML.Core.Models.Team;
using TeamMapper = GalaxyUML.Data.Mappers.TeamMapper;

namespace GalaxyUML.Data.Repositories
{
    class MeetingRepo : IMeetingRepo
    {
        private readonly AppDbContext _context;

        public MeetingRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Meeting meeting, Team team)
        {
            if (await _context.Meetings.AnyAsync(m => m.Id == meeting.IdMeeting))
                throw new Exception("Meeting with this id already exists.");
            
            var entityT = await _context.Teams.FirstOrDefaultAsync(t => t.Id == team.IdTeam);
            if (entityT == null)
                throw new Exception("Team with this id doesn't exist.");

            var entity = MeetingMapper.ToEntity(meeting, TeamMapper.ToEntity(team));
            await _context.Meetings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Meeting meeting)
        {
            var entity = await _context.Meetings.FirstOrDefaultAsync(m => m.Id == meeting.IdMeeting);
            if (entity == null)
                throw new Exception("Meeting with this id doesn't exist.");
            
            _context.Meetings.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Meeting>> GetAllByOrganizerAsync(Guid idOrganizer)
        {
            return await _context.Meetings
                            .AsNoTracking()
                            .Where(m => m.IdOrganizer == idOrganizer)
                            .Select(m => MeetingMapper.ToModel(m))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Meeting>> GetAllByTeamAsync(Guid idTeam)
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

        public async Task UpdateAsync(Meeting meeting)
        {
            var entity = await _context.Meetings.FirstOrDefaultAsync(m => m.Id == meeting.IdMeeting);
            if (entity == null)
                throw new Exception("Meeting with this id doesn't exist");

            _context.Meetings.Update(MeetingMapper.ToEntity(meeting, null!)); // Primer, prilagodi svom mapperu
            await _context.SaveChangesAsync();
        }
    }
}