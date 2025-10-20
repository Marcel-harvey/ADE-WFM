using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.StickyNoteDto;

namespace ADE_WFM.Services.StickyNoteService
{
    public interface IStickyNoteService
    {
        // GET services
        Task <List<StickyNote>> GetAllStickyNotes();
        Task<StickyNote> GetStickyNoteById(GetStickyNoteByIdDto dto);

        // ADD services
        Task AddStickyNote(CreateStickyNoteDto dto);

        // UPDATE services
        Task UpdateStickyNote(UpdateStickyNoteDto dto);

        // DELETE services
    }
}
