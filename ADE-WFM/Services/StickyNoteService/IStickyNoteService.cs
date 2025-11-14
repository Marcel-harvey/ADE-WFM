using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.StickyNoteDto;

namespace ADE_WFM.Services.StickyNoteService {
    public interface IStickyNoteService {
        // ADD services
        Task<ServiceResult<StickyNoteResponseDto>> AddStickyNote(CreateStickyNoteDto dto);

        // GET services
        Task<ServiceResult<List<StickyNoteResponseDto>>> GetAllStickyNotes(GetStickyNoteInfoDto dto);

        // UPDATE services
        Task<ServiceResult<StickyNoteResponseDto>> UpdateStickyNote(GetStickyNoteInfoDto dto);

        // DELETE services
        Task<ServiceResult<StickyNoteResponseDto>> DeleteStickyNote(GetStickyNoteInfoDto dto);
    }
}
