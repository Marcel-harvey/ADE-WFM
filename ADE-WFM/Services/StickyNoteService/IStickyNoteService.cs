using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.StickyNoteDto;

namespace ADE_WFM.Services.StickyNoteService
{
    public interface IStickyNoteService
    {
        // GET services
        Task <ServiceResult<List<GetStickyNoteResponseDto>>> GetAllStickyNotes(GetAllUserStickyNotesDto dto);

        // ADD services
        Task AddStickyNote(CreateStickyNoteDto dto);

        // UPDATE services
        Task UpdateStickyNote(UpdateStickyNoteDto dto);

        // DELETE services
        Task DeleteStickyNote(DeleteStickyNoteDto dto);
    }
}
