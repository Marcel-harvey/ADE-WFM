using ADE_WFM.Services.SubTaskService;
using ADE_WFM.Models;
using ADE_WFM.Data;
using ADE_WFM.Models.DTOs.SubTaskDtos;
using Microsoft.EntityFrameworkCore;

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
        public async Task AddSubTasksToTodo(AddSubTasksToTodoDto dto)
        {
            var todo = await _context.Todos
                .Include(st => st.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == dto.TodoId)
                ?? throw new KeyNotFoundException($"Todo with Id {dto.TodoId} not found.");

            // Make sure SubTasks collection is initialized
            if (todo.SubTasks == null)
                todo.SubTasks = new List<SubTask>();

            foreach (var subtask in dto.SubTasks)
            {
                var subTask = new SubTask
                {
                    Description = subtask.Description,
                    IsCompleted = false,
                    TodoId = dto.TodoId
                };

                todo.SubTasks?.Add(subTask);
            }

            await _context.SaveChangesAsync();
        }


        // UPDATE services
        public async Task UpdateSubTask(UpdateSubTaskDto dto)
        {
            var subTask = await _context.SubTasks
                .FirstOrDefaultAsync(st => st.Id == dto.SubTaskId && st.TodoId == dto.TodoId)
                ?? throw new KeyNotFoundException($"SubTask with ID {dto.SubTaskId} not found for Todo {dto.TodoId}.");

            subTask.Description = dto.Description ?? subTask.Description;
            subTask.IsCompleted = dto.IsCompleted ?? subTask.IsCompleted;

            await _context.SaveChangesAsync();
        }

        // DELETE serives
        public async Task DeleteSubTask(DeleteSubTaskDto dto)
        {
            var subTask = await _context.SubTasks
                .FirstOrDefaultAsync(st => st.Id == dto.SubTaskId && st.TodoId == dto.TodoId)
                ?? throw new KeyNotFoundException($"SubTask with ID {dto.SubTaskId} not found for Todo {dto.TodoId}.");

            _context.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();
        }
    }
}
