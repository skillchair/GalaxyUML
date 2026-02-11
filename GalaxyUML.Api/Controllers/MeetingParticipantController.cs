using Microsoft.AspNetCore.Mvc;
using MeetingParticipant = GalaxyUML.Core.Models.MeetingParticipant;
using Meeting = GalaxyUML.Core.Models.Meeting;
using TeamMember = GalaxyUML.Core.Models.TeamMember;
using IMeetingParticipant = GalaxyUML.Data.Repositories.IMeetingParticipantRepo;
using IMeetingParticipantRepo = GalaxyUML.Data.Repositories.IMeetingParticipantRepo;
using TeamEntity = GalaxyUML.Data.Entities.TeamEntity;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/participants")]
    public class MeetingParticipantController : ControllerBase
    {
        private readonly IMeetingParticipantRepo _participantRepo;

        public MeetingParticipantController(IMeetingParticipantRepo participantRepo)
        {
            _participantRepo = participantRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var participant = await _participantRepo.GetByIdAsync(id);
            if (participant == null) return NotFound();
            return Ok(participant);
        }

        [HttpGet("meeting/{idMeeting:guid}")]
        public async Task<IActionResult> GetByMeetingAsync(Guid idMeeting)
        {
            var participants = await _participantRepo.GetByMeetingAsync(idMeeting);
            return Ok(participants);
        }

        [HttpGet("participant/{idParticipant:guid}")]
        public async Task<IActionResult> GetByParticipantAsync(Guid idParticipant)
        {
            var participants = await _participantRepo.GetByParticipantAsync(idParticipant);
            return Ok(participants);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var participants = await _participantRepo.GetAllAsync();
            return Ok(participants);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] MeetingParticipant meetingParticipant/*, TeamEntity teamEntity*/)
        {
            try
            {
                await _participantRepo.CreateAsync(meetingParticipant/*, teamEntity*/);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] MeetingParticipant meetingParticipant/*, TeamEntity teamEntity*/)
        {
            try
            {
                await _participantRepo.UpdateAsync(id, meetingParticipant/*, teamEntity*/);
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
                await _participantRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}