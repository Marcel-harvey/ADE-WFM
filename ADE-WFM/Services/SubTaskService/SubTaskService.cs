using ADE_WFM.Services.SubTaskService;
using ADE_WFM.Models;
using ADE_WFM.Data;

namespace ADE_WFM.Services.SubTaskService
{
    public class SubTaskService : ISubTaskService
    {
        private readonly ApplicationDbContext _context;
        public SubTaskService(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET serives

        // ADD services

        // UPDATE services

        // DELETE serives
    }
}
