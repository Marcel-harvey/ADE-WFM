using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.StickyNoteDto;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.StickyNoteService
{
    public class StickyNoteService : IStickyNoteService
    {
        private readonly ApplicationDbContext _context;

        public StickyNoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET services
        public async Task<List<StickyNote>> GetAllStickyNotes()
        {
            var stickyNotes = await _context.StickyNotes
                .Include(user => user.User)
                .ToListAsync();

            return stickyNotes;
        }


        public async Task<StickyNote> GetStickyNoteById(GetStickyNoteByIdDto dto)
        {
            var stickyNote = await _context.StickyNotes
                .Include(user => user.User)
                .FirstOrDefaultAsync(snId => snId.Id == dto.stickyNoteId)
                ?? throw new KeyNotFoundException($"Sticky Note with Id {dto.stickyNoteId} not found.");

            return stickyNote;
        }


        // ADD services
        public async Task AddStickyNote(CreateStickyNoteDto dto)
        {
            var newStickyNote = new StickyNote
            {
                Content = dto.Content,
                UserId = dto.UserId
            };

            _context.StickyNotes.Add(newStickyNote);
            await _context.SaveChangesAsync();
        }


        // UPDATE services
        public async Task UpdateStickyNote(UpdateStickyNoteDto dto)
        {
            var stickyNote = await _context.StickyNotes
                .FirstOrDefaultAsync(snId => snId.Id == dto.StickyNoteId)
                ?? throw new KeyNotFoundException($"Sticky Note with Id {dto.StickyNoteId} not found.");

            stickyNote.Content = dto.NewContent;
            await _context.SaveChangesAsync();
        }

        // DELETE services
        public async Task DeleteStickyNote(DeleteStickyNoteDto dto)
        {
            var stickyNote = await _context.StickyNotes
                .FirstOrDefaultAsync(snId => snId.Id == dto.StickyNoteId)
                ?? throw new KeyNotFoundException($"Sticky Note with Id {dto.StickyNoteId} not found.");

            _context.StickyNotes.Remove(stickyNote);
            await _context.SaveChangesAsync();
        }
    }
}
