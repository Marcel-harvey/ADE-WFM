using ADE_WFM.Models;
using ADE_WFM.Data;

namespace ADE_WFM.Services.TodoService
{
    public class TodoService : ITodoService
    {
        private readonly ApplicationDbContext _context;

        public TodoService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET service

        // ADD service

        // UPDATE service

        // DELETE service



    }
}
