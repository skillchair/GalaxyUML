using Microsoft.AspNetCore.Mvc;
using Chat = GalaxyUML.Core.Models.Chat;
using IChatRepo = GalaxyUML.Data.Repositories.IChatRepo;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepo _chatRepo;

        public ChatController(IChatRepo chatRepo)
        {
            _chatRepo = chatRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var chat = _chatRepo.GetByIdAsync(id);
            if (chat == null) return NotFound(chat);
            return Ok(chat);
        }

        [HttpGet("meeting/{id:guid}")]
        public async Task<IActionResult> GetByMeetingAsync(Guid idMeeting)
        {
            var chat = _chatRepo.GetByMeetingAsync(idMeeting);
            if (chat == null) return NotFound(chat);
            return Ok(chat);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var chats = _chatRepo.GetAllAsync();
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Chat chat)
        {
            try
            {
                await _chatRepo.CreateAsync(chat);
                return Ok(chat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}