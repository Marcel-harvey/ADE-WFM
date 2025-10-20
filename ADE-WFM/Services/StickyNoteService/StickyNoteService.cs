using ADE_WFM.Data;
using ADE_WFM.Models;
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

        // ADD services

        // UPDATE services

        // DELETE services
    }
}
