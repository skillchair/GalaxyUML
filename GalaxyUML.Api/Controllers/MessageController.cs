using Microsoft.AspNetCore.Mvc;
using Message = GalaxyUML.Core.Models.Message;
using IMessageRepo = GalaxyUML.Data.Repositories.IMessageRepo;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepo _messageRepo;

        public MessageController(IMessageRepo messageRepo)
        {
            _messageRepo = messageRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var message = await _messageRepo.GetByIdAsync(id); // Dodat await
            if (message == null) return NotFound();
            return Ok(message);
        }

        [HttpGet("chat/{idChat:guid}")] // Ispravljeno: bilo je {id}
        public async Task<IActionResult> GetByChatAsync(Guid idChat)
        {
            var messages = await _messageRepo.GetByChatAsync(idChat); // Dodat await
            return Ok(messages);
        }

        [HttpGet("chat/{idChat:guid}/sender/{idSender:guid}")] // Ispravljeno: dodata kosa crta
        public async Task<IActionResult> GetByChatSenderAsync(Guid idChat, Guid idSender)
        {
            var messages = await _messageRepo.GetByChatSenderAsync(idChat, idSender); // Dodat await
            return Ok(messages);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _messageRepo.GetAllAsync(); // Dodat await
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