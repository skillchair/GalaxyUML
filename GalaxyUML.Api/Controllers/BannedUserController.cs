using Microsoft.AspNetCore.Mvc;
using BannedUser = GalaxyUML.Core.Models.BannedUser;
using IBannedUserRepo = GalaxyUML.Data.Repositories.IBannedUserRepo;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/bans")]
    class BannedUserController : ControllerBase
    {
        private readonly IBannedUserRepo _banRepo;

        public BannedUserController(IBannedUserRepo banRepo)
        {
            _banRepo = banRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var ban = await _banRepo.GetByIdAsync(id);
            if (ban == null) return NotFound();
            return Ok(ban);
        }

        [HttpGet("user/{id:guid}")]
        public async Task<IActionResult> GetByUserAsync(Guid idUser)
        {
            var ban = await _banRepo.GetByUserAsync(idUser);
            if (ban == null) return NotFound();
            return Ok(ban);
        }

        [HttpGet("team/{id:guid}")]
        public async Task<IActionResult> GetByTeamAsync(Guid idTeam)
        {
            var ban = await _banRepo.GetByTeamAsync(idTeam);
            if (ban == null) return NotFound();
            return Ok(ban);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var bans = await _banRepo.GetAllAsync();
            return Ok(bans);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] BannedUser bannedUser)
        {
            try
            {
                await _banRepo.CreateAsync(bannedUser);
                return Ok(bannedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BannedUser bannedUser)
        {
            try
            {
                await _banRepo.UpdateAsync(id, bannedUser);
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
                await _banRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}