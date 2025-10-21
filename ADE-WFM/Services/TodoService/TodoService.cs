using ADE_WFM.Models;
using ADE_WFM.Data;
using Microsoft.EntityFrameworkCore;
using ADE_WFM.Models.DTOs.TodoDtos;

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
        // Get all todos
        public async Task<List<Todo>> GetAllTodos()
        {
            var todos = await _context.Todos
                .Include(user => user.User)
                .Include(subTasks => subTasks.SubTasks)
                .ToListAsync();

            return todos;
        }


        // Get todo by id
        public async Task<Todo> GetTodoById(GetTodoByIdDto dto)
        {
            var todo = await _context.Todos
                .Include(t => t.User)
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == dto.TodoId)
                ?? throw new KeyNotFoundException($"Todo with ID {dto.TodoId} not found.");
            
            return todo;
        }


        // Get all todos for a specific user
        public async Task<List<Todo>> GetAllUserTodos(GetAllUserTodoDto dto)
        {
            var todos = await _context.Todos
                .Where(t => t.UserId == dto.UserId)
                .Include(t => t.User)
                .Include(t => t.SubTasks)
                .Include(t => t.Project)
                .ToListAsync();

            return todos;
        }


        // ADD service

        // UPDATE service

        // DELETE service



    }
}
