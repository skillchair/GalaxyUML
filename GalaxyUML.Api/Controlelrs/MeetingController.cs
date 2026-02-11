using Microsoft.AspNetCore.Mvc;
using IMeetingRepo = GalaxyUML.Data.Repositories.IMeetingRepo;
using Meeting = GalaxyUML.Core.Models.Meeting;
using Team = GalaxyUML.Core.Models.Team;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/meetings")]
    class MeetingController : ControllerBase
    {
        private readonly IMeetingRepo _meetingRepo;

        public MeetingController(IMeetingRepo meetingRepo)
        {
            _meetingRepo = meetingRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var meeting = await _meetingRepo.GetByIdAsync(id);
            if (meeting == null) return NotFound();
            return Ok(meeting);
        }

        [HttpGet("team/{id:guid}")]
        public async Task<IActionResult> GetByTeamAsync(Guid idTeam)
        {
            var meetings = await _meetingRepo.GetByTeamAsync(idTeam);
            return Ok(meetings);
        }

        [HttpGet("{isActive:bool}/team/{id:guid}")]
        public async Task<IActionResult> GetByTeamIfActiveAsync(Guid idTeam)
        {
            var meeting = await _meetingRepo.GetByTeamIfActiveAsync(idTeam);
            if (meeting == null) return NotFound();
            return Ok(meeting);
        }

        [HttpGet("organizer/{id:guid}")]
        public async Task<IActionResult> GetByOrganizerAsync(Guid idOrganizer)
        {
            var meetings = await _meetingRepo.GetByOrganizerAsync(idOrganizer);
            return Ok(meetings);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var meetings = await _meetingRepo.GetAllAsync();
            return Ok(meetings);
        }

        [HttpPost("meeting/{idMeeting:guid}/team/{idTeam:guid}")]
        public async Task<IActionResult> CreateAsync(Meeting meeting, Team team)
        {
            try
            {
                await _meetingRepo.CreateAsync(meeting, team);
                return CreatedAtAction(nameof(GetByIdAsync), new { idMeeting = meeting.IdMeeting}, meeting);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Meeting meeting)
        {
            try
            {
                await _meetingRepo.UpdateAsync(meeting);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _meetingRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}