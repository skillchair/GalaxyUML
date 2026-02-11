using Microsoft.AspNetCore.Mvc;
using IMeetingRepo = GalaxyUML.Data.Repositories.IMeetingRepo;
using Meeting = GalaxyUML.Core.Models.Meeting;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/meetings")]
    public class MeetingController : ControllerBase
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

        [HttpGet("team/{idTeam:guid}")] // Ispravljeno: bilo je {id}
        public async Task<IActionResult> GetByTeamAsync(Guid idTeam)
        {
            var meetings = await _meetingRepo.GetByTeamAsync(idTeam);
            return Ok(meetings);
        }

        [HttpGet("active/team/{idTeam:guid}")] // Izmenjeno: sklonjen :bool jer pravi konflikt
        public async Task<IActionResult> GetByTeamIfActiveAsync(Guid idTeam)
        {
            var meeting = await _meetingRepo.GetByTeamIfActiveAsync(idTeam);
            if (meeting == null) return NotFound();
            return Ok(meeting);
        }

        [HttpGet("organizer/{idOrganizer:guid}")]
        public async Task<IActionResult> GetByOrganizerAsync(Guid idOrganizer)
        {
            var meetings = await _meetingRepo.GetByOrganizerAsync(idOrganizer);
            return Ok(meetings);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var meetings = await _meetingRepo.GetAllAsync();
            return Ok(meetings);
        }

        [HttpDelete("{id:guid}")] // Dodat :guid radi sigurnosti
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