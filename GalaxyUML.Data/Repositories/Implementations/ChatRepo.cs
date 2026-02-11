using Microsoft.EntityFrameworkCore;
using Chat = GalaxyUML.Core.Models.Chat;
using ChatMapper = GalaxyUML.Data.Mappers.ChatMapper;

namespace GalaxyUML.Data.Repositories.Implementations
{
    class ChatRepo : IChatRepo
    {
        AppDbContext _context;

        public ChatRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Chat chat/*, Team team*/)
        {
            // var entityT = await _context.Teams.FindAsync(team);
            // if (entityT == null)
            //     throw new Exception("Team not found.");
            var entity = ChatMapper.ToEntity(chat/*, TeamMapper.ToEntity(team)*/);
            _context.Chats.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Chats.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null)
                throw new Exception("Chat with this id doesn't exist.");

            _context.Chats.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await _context.Chats
                            .AsNoTracking()
                            .Select(c => ChatMapper.ToModel(c))
                            .ToListAsync();
        }

        public async Task<Chat?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Chats.FirstOrDefaultAsync(c => c.Id == id);
            return entity == null ? null : ChatMapper.ToModel(entity);
        }

        public async Task<Chat?> GetByMeetingAsync(Guid idMeeting)
        {
            var entity = await _context.Chats.FirstOrDefaultAsync(c => c.IdMeeting == idMeeting);
            return entity == null ? null : ChatMapper.ToModel(entity);
        }

        public async Task UpdateAsync(Guid id, Chat chat)
        {
            var entity = await _context.Chats.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null)
                throw new Exception("Chat with this id doesn't exist.");

            _context.Chats.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}