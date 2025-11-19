using ADE_WFM.Models.DTOs.StickyNoteDto;
using ADE_WFM.Services.StickyNoteService;
using Microsoft.AspNetCore.Mvc;

namespace ADE_WFM.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class StickyNoteController : ControllerBase {
        private readonly IStickyNoteService _stickyNoteService;
        public StickyNoteController(IStickyNoteService stickyNoteService) {
            _stickyNoteService = stickyNoteService;
        }

        // CREATE API's
        [HttpPost]
        public async Task<IActionResult> CreateStickyNote([FromBody] CreateStickyNoteDto dto) {
            var result = await _stickyNoteService.AddStickyNote(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET API's
        // Get all users sticky notes
        [HttpGet]
        public async Task<IActionResult> GetAllUserStickyNotes() {
            var result = await _stickyNoteService.GetAllStickyNotes();

            return result.Succeeded ? Ok(result) : NotFound(result);
        }

        // Get sticky note by ID
        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetStickyNoteById(int noteId) {
            var result = await _stickyNoteService.GetStickyNoteById(
                new GetStickyNoteInfoDto {
                    StickyNoteId = noteId
                });

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // UPDATE API's
        // Update selected sticky note
        [HttpPut]
        public async Task<IActionResult> UpdateStickyNote([FromBody] GetStickyNoteInfoDto dto, [FromQuery] int? sitckyNoteId = null) {
            dto.StickyNoteId = sitckyNoteId ?? dto.StickyNoteId;
            var result = await _stickyNoteService.UpdateStickyNote(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE API's
        // Delete selected sticky note
        [HttpDelete("{stickyNoteId}")]
        public async Task<IActionResult> DeleteStickyNote(int stickyNoteId) {
            var dto = new GetStickyNoteInfoDto {
                StickyNoteId = stickyNoteId
            };

            var result = await _stickyNoteService.DeleteStickyNote(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
