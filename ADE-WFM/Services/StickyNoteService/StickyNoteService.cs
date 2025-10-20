using ADE_WFM.Data;

namespace ADE_WFM.Services.StickyNoteService
{
    public class StickyNoteService : IStickyNoteService
    {
        private readonly ApplicationDbContext _context;

        public StickyNoteService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
