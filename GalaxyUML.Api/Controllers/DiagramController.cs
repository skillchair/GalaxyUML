using Microsoft.AspNetCore.Mvc;
using ObjectType = GalaxyUML.Core.Models.ObjectType;
using IDiagramRepo = GalaxyUML.Data.Repositories.IDiagramRepo;
using Diagram = GalaxyUML.Core.Models.Diagram;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/diagrams")]
    public class DiagramController : ControllerBase
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
            if (diagram == null) return NotFound();
            return Ok(diagram);
        }

        [HttpGet("meeting/{idMeeting:guid}")]
        public async Task<IActionResult> GetByMeetingAsync(Guid idMeeting)
        {
            var diagrams = await _diagramRepo.GetByMeetingAsync(idMeeting);
            return Ok(diagrams);
        }

        [HttpGet("meeting/{idMeeting:guid}/type/{type}")] // Dodato /type/ radi preglednosti
        public async Task<IActionResult> GetByMeetingTypeAsync(Guid idMeeting, ObjectType type)
        {
            var diagrams = await _diagramRepo.GetByMeetingTypeAsync(idMeeting, type);
            return Ok(diagrams);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var diagrams = await _diagramRepo.GetAllAsync();
            return Ok(diagrams);
        }
    }
}