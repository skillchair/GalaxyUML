using Microsoft.AspNetCore.Mvc;
using Message = GalaxyUML.Core.Models.Message;
using IMessageRepo = GalaxyUML.Data.Repositories.IMessageRepo;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/messages")]
    class MessageController : ControllerBase
    {
        private readonly IMessageRepo _messageRepo;

        public MessageController(IMessageRepo messageRepo)
        {
            _messageRepo = messageRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var message = _messageRepo.GetByIdAsync(id);
            if (message == null) return NotFound();
            return Ok(message);
        }

        [HttpGet("chat/{id}")]
        public async Task<IActionResult> GetByChatAsync(Guid idChat)
        {
            var messages = _messageRepo.GetByChatAsync(idChat);
            return Ok(messages);
        }

        [HttpGet("chat/{idChat:guid}/sender{idSender:guid}")]
        public async Task<IActionResult> GetByChatSenderAsync(Guid idChat, Guid idSender)
        {
            var messages = _messageRepo.GetByChatSenderAsync(idChat, idSender);
            return Ok(messages);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var messages = _messageRepo.GetAllAsync();
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Message message)
        {
            try
            {
                await _messageRepo.CreateAsync(message);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}