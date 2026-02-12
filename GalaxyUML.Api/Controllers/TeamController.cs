using GalaxyUML.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using ITeamRepo = GalaxyUML.Data.Repositories.ITeamRepo;
using Team = GalaxyUML.Core.Models.Team;
using TeamDto = GalaxyUML.Core.Models.DTOs.TeamDto;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepo _teamRepo;
        private readonly IUserRepo _userRepo;

        public TeamController(ITeamRepo teamRepo, IUserRepo userRepo)
        {
            _teamRepo = teamRepo;
            _userRepo = userRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var team = await _teamRepo.GetByIdAsync(id);
            if (team == null) return NotFound();
            return Ok(team);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCodeAsync(string code)
        {
            var team = _teamRepo.GetByCodeAsync(code);
            if (team == null) return NotFound();
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
        public async Task<IActionResult> CreateTeamAsync([FromBody] TeamDto dto)
        {
            // Fetch the user from DB using dto.IdOwner
            var user = await _userRepo.GetByIdAsync(dto.IdTeamOwner);
            if (user == null) return NotFound("User not found.");

            // Now construct the domain object properly
            var team = new Team(Guid.NewGuid(), dto.IdTeamOwner, dto.TeamName, user);

            await _teamRepo.CreateAsync(team);
            return Ok();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTeamAsync(Guid id, [FromBody] TeamDto dto)
        {
            // Fetch the user from DB using dto.IdOwner
            var user = await _userRepo.GetByIdAsync(dto.IdTeamOwner);
            if (user == null) return NotFound("User not found.");

            // Now construct the domain object properly
            var team = new Team(Guid.NewGuid(), dto.IdTeamOwner, dto.TeamName, user);

            await _teamRepo.UpdateAsync(id, team);
            return Ok();
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
                return NotFound(ex.Message);
            }
        }
    }
}