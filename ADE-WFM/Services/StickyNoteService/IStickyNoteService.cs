using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.StickyNoteDto;

namespace ADE_WFM.Services.StickyNoteService
{
    public interface IStickyNoteService
    {
        // GET services
        Task<ServiceResult<List<GetStickyNoteResponseDto>>> GetAllStickyNotes(GetAllUserStickyNotesDto dto);

        // ADD services
        Task<ServiceResult<StickyNoteResponseDto>> AddStickyNote(CreateStickyNoteDto dto);

        // UPDATE services
        Task<ServiceResult<StickyNoteResponseDto>> UpdateStickyNote(GetStickyNoteInfoDto dto);

        // DELETE services
        Task<ServiceResult<StickyNoteResponseDto>> DeleteStickyNote(GetStickyNoteInfoDto dto);
    }
}
