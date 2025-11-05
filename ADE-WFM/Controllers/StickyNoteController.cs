using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.StickyNoteDto;
using ADE_WFM.Services.StickyNoteService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StickyNoteController : ControllerBase
    {
        private readonly IStickyNoteService _stickyNoteService;
        public StickyNoteController(IStickyNoteService stickyNoteService)
        {
            _stickyNoteService = stickyNoteService;
        }

        // CREATE API's
        [HttpPost]
        public async Task<IActionResult> CreateStickyNote([FromBody] CreateStickyNoteDto dto)
        {
            var result = await _stickyNoteService.AddStickyNote(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET API's
        // Get all users sticky notes
        [HttpGet("{userId}/{stickyNoteId}")]
        public async Task<IActionResult> GetAllUserStickyNotes(string userId, int stickyNoteId)
        {
            var dto = new GetStickyNoteInfoDto
            {
                StickyNoteId = stickyNoteId,
                UserId = userId
            };

            var result = await _stickyNoteService.GetAllStickyNotes(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // UPDATE API's
        // Update selected sticky note
        [HttpPut]
        public async Task<IActionResult> UpdateStickyNote([FromBody] GetStickyNoteInfoDto dto, [FromQuery] int? sitckyNoteId = null, [FromQuery] string? userId = null)
        {
            dto.StickyNoteId = sitckyNoteId ?? dto.StickyNoteId;
            dto.UserId = userId ?? dto.UserId;
            var result = await _stickyNoteService.UpdateStickyNote(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE API's
        // Delete selected sticky note
        [HttpDelete("{userId}/{stickyNoteId}")]
        public async Task<IActionResult> DeleteStickyNote(string userId, int stickyNoteId)
        {
            var dto = new GetStickyNoteInfoDto
            {
                StickyNoteId = stickyNoteId,
                UserId = userId
            };

            var result = await _stickyNoteService.DeleteStickyNote(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
