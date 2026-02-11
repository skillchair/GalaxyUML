using Microsoft.AspNetCore.Mvc;
using ObjectType = GalaxyUML.Core.Models.ObjectType;
using IDiagramRepo = GalaxyUML.Data.Repositories.IDiagramRepo;
using GalaxyUML.Core.Models;
//using IDiagram = GalaxyUML.Core.Models.IDiagram;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/diagrams")]
    class DiagramController : ControllerBase
    {
        private readonly IDiagramRepo _diagramRepo;

        public DiagramController(IDiagramRepo diagramRepo)
        {
            _diagramRepo = diagramRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var diagram = await _diagramRepo.GetByIdAsync(id);
            if (diagram == null) return NotFound(diagram);
            return Ok(diagram);
        }

        [HttpGet("meeting/{id:guid}")]
        public async Task<IActionResult> GetByMeetingAsync(Guid idMeeting)
        {
            var diagrams = await _diagramRepo.GetByMeetingAsync(idMeeting);
            return Ok(diagrams);
        }

        [HttpGet("meeting/{id:guid}/type/{type}")]
        public async Task<IActionResult> GetByMeetingTypeAsync(Guid idMeeting, ObjectType type)
        {
            var diagrams = await _diagramRepo.GetByMeetingTypeAsync(idMeeting, type);
            return Ok(diagrams);
        }

        [HttpGet("meeting/{idMeeting:guid}/parent/{idParent:guid}")]
        public async Task<IActionResult> GetByMeetingParentAsync(Guid idMeeting, Guid idParent)
        {
            var diagrams = await _diagramRepo.GetByMeetingParentAsync(idMeeting, idParent);
            return Ok(diagrams);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var diagrams = await _diagramRepo.GetAllAsync();
            return Ok(diagrams);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Diagram diagram)
        {
            try
            {
                await _diagramRepo.CreateAsync(diagram);
                return Ok(diagram);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] Diagram diagram)
        {
            try
            {
                await _diagramRepo.UpdateAsync(id, diagram);
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
                await _diagramRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}