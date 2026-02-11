using Microsoft.AspNetCore.Mvc;
using TeamMember = GalaxyUML.Core.Models.TeamMember;
using Team = GalaxyUML.Core.Models.Team;
using User = GalaxyUML.Core.Models.User;
using ITeamMemberRepo = GalaxyUML.Data.Repositories.ITeamMemberRepo;
using RoleEnum = GalaxyUML.Core.Models.RoleEnum;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/members")]
    public class TeamMemberController : ControllerBase
    {
        private readonly ITeamMemberRepo _memberRepo;

        public TeamMemberController(ITeamMemberRepo memberRepo)
        {
            _memberRepo = memberRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetbyIdAsync(Guid id)
        {
            var member = await _memberRepo.GetByIdAsync(id);
            if (member == null) return NotFound();
            return Ok(member);
        }

        [HttpGet("team/{id}")]
        public async Task<IActionResult> GetByTeamAsync(Guid idTeam)
        {
            var members = await _memberRepo.GetByTeamAsync(idTeam);
            return Ok(members);
        }

        [HttpGet("team/{id:guid}member/{role:int}")]
        public async Task<IActionResult> GetByTeamRoleAsync(Guid idTeam, RoleEnum role)
        {
            var members = await _memberRepo.GetByTeamRoleAsync(idTeam, role);
            return Ok(members);
        }

        [HttpGet("user/{id:guid}")]
        public async Task<IActionResult> GetByUserAsync(Guid idUser)
        {
            var members = await _memberRepo.GetByUserAsync(idUser);
            return Ok(members);
        }

        [HttpGet("user/{id:guid}/role")]
        public async Task<IActionResult> GetByUserRoleAsync(Guid idUser, RoleEnum role)
        {
            var members = await _memberRepo.GetByUserRoleAsync(idUser, role);
            return Ok(members);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var members = await _memberRepo.GetAllAsync();
            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TeamMember teamMember)
        {
            try
            {
                await _memberRepo.CreateAsync(teamMember);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(TeamMember teamMember)
        {
            try
            {
                await _memberRepo.UpdateAsync(teamMember);
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
                await _memberRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }
    }
}