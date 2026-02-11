using Microsoft.EntityFrameworkCore;
using Chat = GalaxyUML.Core.Models.Chat;
using Message = GalaxyUML.Core.Models.Message;
using Team = GalaxyUML.Core.Models.Team;
using MessageMapper = GalaxyUML.Data.Mappers.MessageMapper;
using ChatMapper = GalaxyUML.Data.Mappers.ChatMapper;
using GalaxyUML.Data.Mappers;

namespace GalaxyUML.Data.Repositories.Implementations
{
    class MessageRepo : IMessageRepo
    {
        AppDbContext _context;

        public MessageRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Message message/*, Chat chat, Team team*/)
        {
            // if (await _context.Chats.FindAsync(chat) != null)
            //     throw new Exception("Chat not found.");
            // if (await _context.Messages.AnyAsync(m => m.Id == message.IdMessage))
            //     throw new Exception("Message with this id already exists.");

            // var teamEntity = TeamMapper.ToEntity(team);
            // var chatEntity = ChatMapper.ToEntity(chat, teamEntity);
            var entity = MessageMapper.ToEntity(message/*, chatEntity, teamEntity*/);
            _context.Messages.Add(entity);
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

        public async Task<IEnumerable<Message>> GetByChatSenderAsync(Guid idChat, Guid idSender)
        {
            return await _context.Messages
                            .AsNoTracking()
                            .Where(m => m.IdSender == idSender && m.IdChat == idChat)
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