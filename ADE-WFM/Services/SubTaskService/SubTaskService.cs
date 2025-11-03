using ADE_WFM.Services.SubTaskService;
using ADE_WFM.Models;
using ADE_WFM.Data;
using ADE_WFM.Models.DTOs.SubTaskDtos;
using Microsoft.EntityFrameworkCore;
using ADE_WFM.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using ADE_WFM.Models.DTOs.ProjectDtos;
using Microsoft.JSInterop.Infrastructure;

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
        // Add new sub task to a todo
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

                _logger.LogInformation("SubTask added successfully to Todo '{TodoTitle}' (ID: {TodoId})", todo.Title, todo.Id);

                return ServiceResult<SubTaskResponseDto>.Success(new SubTaskResponseDto
                    {
                        SubTaskId = subTask.Id,
                        Description = subTask.Description,
                        IsCompleted = subTask.IsCompleted,
                        TodoId = todo.Id,
                        TodoTitle = todo.Title
                    },
                    "Sub task added successfully"
                );
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
        // Get all subtasks for a specific todo
        public async Task<ServiceResult<List<SubTaskResponseDto>>> GetTodoSubTasks(GetSubTasksDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<List<SubTaskResponseDto>>.Failure("Input data is null.");

            if (dto.TodoId <= 0)
                return ServiceResult<List<SubTaskResponseDto>>.Failure("Invalid Todo ID.");

            try
            {
                var todo = await _context.Todos
                    .FirstOrDefaultAsync(t => t.Id == dto.TodoId);
                if (todo == null)
                {
                    _logger.LogInformation("Todo with ID {TodoId} not found", dto.TodoId);
                    return ServiceResult<List<SubTaskResponseDto>>.Failure($"Todo with ID {dto.TodoId} not found.");
                }

                var subtasks = await _context.SubTasks
                    .Where(st => st.TodoId == dto.TodoId)
                    .ToListAsync();
                if (!subtasks.Any())
                {
                    _logger.LogInformation("No SubTasks found for Todo ID {TodoId}", dto.TodoId);
                    return ServiceResult<List<SubTaskResponseDto>>.Failure($"No SubTasks found for Todo ID {dto.TodoId}.");
                }

                _logger.LogInformation("Retrieved {count} SubTasks for Todo ID {ToDoTitle}", subtasks.Count(), todo.Title);

                return ServiceResult<List<SubTaskResponseDto>>.Success(
                    subtasks.Select(st => new SubTaskResponseDto
                    {
                        SubTaskId = st.Id,
                        Description = st.Description,
                        IsCompleted = st.IsCompleted,
                        TodoId = dto.TodoId,
                        TodoTitle = todo.Title
                    }).ToList(),
                    "SubTasks retrieved successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving SubTasks for Todo ID {TodoId}", dto.TodoId);
                return ServiceResult<List<SubTaskResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving the subtasks.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services
        public async Task<ServiceResult<SubTaskResponseDto>> UpdateSubTask(UpdateSubTaskDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<SubTaskResponseDto>.Failure("Input data is null.");

            if (dto.TodoId <= 0)
                return ServiceResult<SubTaskResponseDto>.Failure("Invalid Todo ID.");

            if (dto.SubTaskId <= 0)
                return ServiceResult<SubTaskResponseDto>.Failure("Invalid SubTask ID.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                return ServiceResult<SubTaskResponseDto>.Failure("Description cannot be empty.");

            try
            {
                // Confirm if Todo exists
                var todo = await _context.Todos
                    .FirstOrDefaultAsync(t => t.Id == dto.TodoId);
                if (todo == null)
                {
                    _logger.LogInformation("Todo with ID {TodoId} not found", dto.TodoId);
                    return ServiceResult<SubTaskResponseDto>.Failure($"Todo with ID {dto.TodoId} not found.");
                }

                // Find the subtask belonging to this todo
                var subTask = await _context.SubTasks
                    .FirstOrDefaultAsync(st => st.Id == dto.SubTaskId && st.TodoId == dto.TodoId);
                if (subTask == null)
                {
                    _logger.LogInformation("SubTask with ID {SubTaskId} not found for Todo ID {TodoId}", dto.SubTaskId, dto.TodoId);
                    return ServiceResult<SubTaskResponseDto>.Failure($"SubTask with ID {dto.SubTaskId} not found for Todo {dto.TodoId}.");
                }


                // Update fields
                subTask.Description = dto.Description;

                await _context.SaveChangesAsync();

                _logger.LogInformation("SubTask ID {SubTaskId} updated for Todo ID {TodoId}.", subTask.Id, todo.Id);

                return ServiceResult<SubTaskResponseDto>.Success(new SubTaskResponseDto
                    {
                        SubTaskId = subTask.Id,
                        Description = subTask.Description,
                        IsCompleted = subTask.IsCompleted,
                        TodoId = todo.Id,
                        TodoTitle = todo.Title
                    },
                    "Sub task updated successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating SubTask ID {SubTaskId}", dto.SubTaskId);
                return ServiceResult<SubTaskResponseDto>.Failure(
                    "A database error occurred while updating the subtask.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating SubTask ID {SubTaskId}", dto.SubTaskId);
                return ServiceResult<SubTaskResponseDto>.Failure(
                    "An unexpected error occurred while updating the subtask.",
                    new[] { ex.Message });
            }
        }


        // Mark sub task as completed/incomplete
        public async Task<ServiceResult<SubTaskResponseDto>> MarkSubTaskCompletion(MarkSubTaskCompletionDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<SubTaskResponseDto>.Failure("Input data is null.");

            if (dto.TodoId <= 0)
                return ServiceResult<SubTaskResponseDto>.Failure("Invalid Todo ID.");

            if (dto.SubTaskId <= 0)
                return ServiceResult<SubTaskResponseDto>.Failure("Invalid SubTask ID.");

            try
            {
                var todo = await _context.Todos
                    .FirstOrDefaultAsync(t => t.Id == dto.TodoId);
                if (todo == null)
                {
                    _logger.LogInformation("Todo with ID {TodoId} not found", dto.TodoId);
                    return ServiceResult<SubTaskResponseDto>.Failure($"Todo with ID {dto.TodoId} not found.");
                }

                var subTask = await _context.SubTasks
                    .FirstOrDefaultAsync(st => st.Id == dto.SubTaskId && st.TodoId == dto.TodoId);
                if (subTask == null)
                {
                    _logger.LogInformation("SubTask with ID {SubTaskId} not found for Todo ID {TodoId}", dto.SubTaskId, dto.TodoId);
                    return ServiceResult<SubTaskResponseDto>.Failure($"SubTask with ID {dto.SubTaskId} not found for Todo {dto.TodoId}.");
                }

                subTask.IsCompleted = dto.IsCompleted;

                await _context.SaveChangesAsync();

                _logger.LogInformation("SubTask ID {SubTaskId} marked as {Status} for Todo ID {TodoId}.",
                    subTask.Id, dto.IsCompleted ? "completed" : "incomplete", todo.Id);

                return ServiceResult<SubTaskResponseDto>.Success(new SubTaskResponseDto
                    {
                        SubTaskId = subTask.Id,
                        Description = subTask.Description,
                        IsCompleted = subTask.IsCompleted,
                        TodoId = todo.Id,
                        TodoTitle = todo.Title
                    },
                    $"Sub task marked {dto.IsCompleted.ToString()} successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating SubTask ID {SubTaskId}", dto.SubTaskId);
                return ServiceResult<SubTaskResponseDto>.Failure(
                    "A database error occurred while updating the subtask.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating SubTask ID {SubTaskId}", dto.SubTaskId);
                return ServiceResult<SubTaskResponseDto>.Failure(
                    "An unexpected error occurred while updating the subtask.",
                    new[] { ex.Message });
            }
        }



        // DELETE serives
        public async Task<ServiceResult<SubTaskResponseDto>> DeleteSubTask(GetSubTasksDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<SubTaskResponseDto>.Failure("Input data is null.");

            if (dto.TodoId <= 0)
                return ServiceResult<SubTaskResponseDto>.Failure("Invalid Todo ID.");

            if (dto.SubTaskId <= 0)
                return ServiceResult<SubTaskResponseDto>.Failure("Invalid SubTask ID.");

            try
            {
                var todo = await _context.Todos
                    .FirstOrDefaultAsync(t => t.Id == dto.TodoId);
                if (todo == null)
                {
                    _logger.LogInformation("Todo with ID {TodoId} not found", dto.TodoId);
                    return ServiceResult<SubTaskResponseDto>.Failure($"Todo with ID {dto.TodoId} not found.");
                }

                var subTask = await _context.SubTasks
                    .FirstOrDefaultAsync(st => st.Id == dto.SubTaskId && st.TodoId == dto.TodoId);
                if (subTask == null)
                {
                    _logger.LogInformation("SubTask with ID {SubTaskId} not found for Todo ID {TodoId}", dto.SubTaskId, dto.TodoId);
                    return ServiceResult<SubTaskResponseDto>.Failure($"SubTask with ID {dto.SubTaskId} not found for Todo {dto.TodoId}.");
                }

                var response = new SubTaskResponseDto
                {
                    SubTaskId = subTask.Id,
                    Description = subTask.Description,
                    IsCompleted = subTask.IsCompleted,
                    TodoId = todo.Id,
                    TodoTitle = todo.Title
                };

                _context.SubTasks.Remove(subTask);
                await _context.SaveChangesAsync();

                _logger.LogInformation("SubTask ID {SubTaskId} deleted from Todo ID {TodoId}.", subTask.Id, todo.Id);

                return ServiceResult<SubTaskResponseDto>.Success(response, "Sub task deleted successfully");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting SubTask ID {SubTaskId}", dto.SubTaskId);
                return ServiceResult<SubTaskResponseDto>.Failure(
                    "A database error occurred while deleting the subtask.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting SubTask ID {SubTaskId}", dto.SubTaskId);
                return ServiceResult<SubTaskResponseDto>.Failure(
                    "An unexpected error occurred while deleting the subtask.",
                    new[] { ex.Message });
            }
        }
    }
}
