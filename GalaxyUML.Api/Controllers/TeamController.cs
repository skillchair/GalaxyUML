using Microsoft.AspNetCore.Mvc;
using ITeamRepo = GalaxyUML.Data.Repositories.ITeamRepo;
using Team = GalaxyUML.Core.Models.Team;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/teams")]
    class TeamController : ControllerBase
    {
        private readonly ITeamRepo _teamRepo;
        
        public TeamController(ITeamRepo teamRepo)
        {
            _teamRepo = teamRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var team = await _teamRepo.GetByIdAsync(id);
            if (team == null) return NotFound();
            return Ok(team);
        }

        [HttpGet("{code:string}")]
        public async Task<IActionResult> GetByCodeAsync(string code)
        {
            var team = _teamRepo.GetByCodeAsync(code);
            if (team == null)  return NotFound();
            return Ok(team);
        }

        [HttpGet("owner/{idOwner:guid}")]
        public async Task<IActionResult> GetByOwnerAsync(Guid idOwner)
        {
            var teams = _teamRepo.GetByOwnerAsync(idOwner);
            return Ok(teams);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var teams = _teamRepo.GetAllAsync();
            return Ok(teams);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Team team)
        {
            try
            {
                await _teamRepo.CreateAsync(team);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = team.IdTeam}, team);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Team team)
        {
            try
            {
                await _teamRepo.UpdateAsync(team);
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
                await _teamRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}