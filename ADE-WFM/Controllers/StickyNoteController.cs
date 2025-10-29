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
        [HttpPost("User/Create")]
        public async Task<IActionResult> CreateStickyNote([FromBody] CreateStickyNoteDto dto)
        {
            var result = await _stickyNoteService.AddStickyNote(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        // GET API's
        // Get all users sticky notes
        [HttpPost("User/All")]
        public async Task<IActionResult> GetAllUserStickyNotes([FromBody] GetAllUserStickyNotesDto dto)
        {
            var result = await _stickyNoteService.GetAllStickyNotes(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        // UPDATE API's
        // Update selected sticky note
        [HttpPut("User/Update")]
        public async Task<IActionResult> UpdateStickyNote([FromBody] GetStickyNoteInfoDto dto)
        {
            var result = await _stickyNoteService.UpdateStickyNote(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        // DELETE API's
        // Delete selected sticky note
        [HttpDelete("User/Delete")]
        public async Task<IActionResult> DeleteStickyNote([FromBody] GetStickyNoteInfoDto dto)
        {
            var result = await _stickyNoteService.DeleteStickyNote(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
