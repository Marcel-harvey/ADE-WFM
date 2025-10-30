using ADE_WFM.Services.SubTaskService;
using ADE_WFM.Models;
using ADE_WFM.Data;
using ADE_WFM.Models.DTOs.SubTaskDtos;
using Microsoft.EntityFrameworkCore;
using ADE_WFM.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using ADE_WFM.Models.DTOs.ProjectDtos;

namespace ADE_WFM.Services.SubTaskService
{
    public class SubTaskService : ISubTaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SubTaskService> _logger;
        public SubTaskService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SubTaskService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        // ADD services
        public async Task<ServiceResult<SubTaskResponseDto>> AddSubTasksToTodo(AddSubTasksToTodoDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<SubTaskResponseDto>.Failure("Input data is null.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                return ServiceResult<SubTaskResponseDto>.Failure("Description cannot be empty.");

            if (dto.TodoId <= 0)
                return ServiceResult<SubTaskResponseDto>.Failure("Invalid Todo ID.");

            try
            {
                // Confirm if the Todo exists
                var todo = await _context.Todos
                    .FirstOrDefaultAsync(t => t.Id == dto.TodoId);

                if (todo == null)
                    return ServiceResult<SubTaskResponseDto>.Failure($"Todo with ID {dto.TodoId} not found.");

                var subTask = new SubTask
                {
                    Description = dto.Description,
                    IsCompleted = false,
                    TodoId = dto.TodoId
                };

                _context.SubTasks.Add(subTask);
                await _context.SaveChangesAsync();

                _logger.LogInformation("SubTask '{Description}' added successfully to Todo '{TodoTitle}' (ID: {TodoId})",
                    subTask.Description, todo.Title, todo.Id);

                return ServiceResult<SubTaskResponseDto>.Success(new SubTaskResponseDto
                {
                    Id = subTask.Id,
                    Description = subTask.Description,
                    IsCompleted = subTask.IsCompleted,
                    TodoTitle = todo.Title
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding SubTask to Todo ID {TodoId}", dto.TodoId);
                return ServiceResult<SubTaskResponseDto>.Failure(
                    "A database error occurred while adding the subtask.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding SubTask to Todo ID {TodoId}", dto.TodoId);
                return ServiceResult<SubTaskResponseDto>.Failure(
                    "An unexpected error occurred while adding the subtask.",
                    new[] { ex.Message });
            }
        }



        // GET serives


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
