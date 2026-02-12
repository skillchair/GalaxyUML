using Microsoft.AspNetCore.Mvc;
using IUserRepo = GalaxyUML.Data.Repositories.IUserRepo;
using User = GalaxyUML.Core.Models.User;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsernameAsync(string username)
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmailAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] User user)
        {
            try
            {
                await _userRepo.CreateAsync(user);
                //return CreatedAtAction(nameof(GetByIdAsync), new { id = user.IdUser }, user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")] // Dodajemo id u rutu radi identifikacije
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] User user)
        {
            try
            {
                await _userRepo.UpdateAsync(id, user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _userRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}