using Microsoft.EntityFrameworkCore;
using Chat = GalaxyUML.Core.Models.Chat;
using Message = GalaxyUML.Core.Models.Message;
using MessageMapper = GalaxyUML.Data.Mappers.MessageMapper;

namespace GalaxyUML.Data.Repositories
{
    class MessageRepo : IMessageRepo
    {
        AppDbContext _context;

        public MessageRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Message message, Chat chat)
        {
            if (await _context.Messages.AnyAsync(m => m.Id == message.IdMessage))
                throw new Exception("Meeting with this id already exists.");
            
            var entity = MessageMapper.ToEntity(message, chat);
            await _context.Messages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetByChatAsync(Guid idChat)
        {
            return await _context.Messages
                            .AsNoTracking()
                            .Where(m => m.IdChat == idChat)
                            .Select(m => MessageMapper.ToModel(m))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetByTeamParticipantAsync(Guid idMeetingParticipant)
        {
            return await _context.Messages
                            .AsNoTracking()
                            .Where(m => m.IdMeetingParticipant == idMeetingParticipant)
                            .Select(m => MessageMapper.ToModel(m))
                            .ToListAsync();
        }

        public async Task<Message?> GetByIdAsync(Guid id)
        {
            return await _context.Messages
                            .AsNoTracking()
                            .Where(m => m.Id == id)
                            .Select(m => MessageMapper.ToModel(m))
                            .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await _context.Messages
                            .AsNoTracking()
                            .Select(m => MessageMapper.ToModel(m))
                            .ToListAsync();
        }
    }
}