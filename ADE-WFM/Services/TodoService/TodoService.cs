using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.TodoDtos;
using ADE_WFM.Services.TenantService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.TodoService {
    public class TodoService : ITodoService {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TodoService> _logger;
        private readonly TenantContext _tenantContext;

        public TodoService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<TodoService> logger,
            TenantContext tenantContext) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _tenantContext = tenantContext;
        }


        // ADD service
        public async Task<ServiceResult<ToDoResponseDto>> AddTodo(AddTodoDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ToDoResponseDto>.Failure("No information provided.");

            if (dto.ProjectId <= 0)
                return ServiceResult<ToDoResponseDto>.Failure("Project id required.");

            if (string.IsNullOrWhiteSpace(dto.Task))
                return ServiceResult<ToDoResponseDto>.Failure("Title is required.");

            try {
                var todo = new Todo {
                    Task = dto.Task,
                    IsComplete = false,
                    DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                    DueDate = dto.DueDate,
                    UserId = dto.UserId ?? null,
                    ProjectId = dto.ProjectId,
                    TenantId = _tenantContext.TenantId
                };

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId);
                if (user == null)
                    _logger.LogInformation("No username found");

                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Todo with ID {TodoId} created successfully for user ID {UserId}.", todo.Id, dto.UserId);

                return ServiceResult<ToDoResponseDto>.Success(
                    new ToDoResponseDto {
                        todoId = todo.Id,
                        IsComplete = todo.IsComplete,
                        Task = dto.Task,
                        UserName = user?.UserName ?? "Unknown",
                        DateCreated = todo.DateCreated,
                        DueDate = dto.DueDate,
                        ProjectId = dto.ProjectId
                    },
                    "Todo created successfully."
                    );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while adding todo {TodoTitle}", dto.Task);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "A database error occurred while adding the todo.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while adding todo {TodoTitle}", dto.Task);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "An unexpected error occurred while adding the todo.",
                    new[] { ex.Message });
            }
        }


        // GET service
        // Get all todos for a user
        public async Task<ServiceResult<List<ToDoResponseDto>>> GetAllUserTodos() {
            try {
                var todos = await _context.Todos
                    .Where(t => t.UserId == _tenantContext.UserId)
                    .Include(t => t.User)
                    .Include(t => t.SubTasks)
                    .ToListAsync();

                if (!todos.Any()) {
                    _logger.LogInformation("No todo's found for user {UserId}.", _tenantContext.UserId);
                    return ServiceResult<List<ToDoResponseDto>>.Failure($"No todo's found for user {_tenantContext.UserName}.");
                }

                return ServiceResult<List<ToDoResponseDto>>.Success(
                    todos.Select(t => new ToDoResponseDto {
                        Id = t.Id,
                        IsComplete = t.IsComplete,
                        UserName = t.User?.UserName ?? "Unknown",
                        ProjectId = t.ProjectId,
                        SubTasks = t.SubTasks?
                            .Select(st => new TodoSubTasksResponseDto {
                                Id = st.Id,
                                Description = st.Description,
                                IsCompleted = st.IsCompleted
                            }).ToList() ?? new List<TodoSubTasksResponseDto>()
                    }).ToList(),
                    "User todo's retrieved successfully."
                );

            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving todo's for user {UserId}.", _tenantContext.UserId);

                return ServiceResult<List<ToDoResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving todo's.",
                    new[] { ex.Message });
            }
        }


        // Get all todos for a project
        public async Task<ServiceResult<List<ToDoResponseDto>>> GetAllProjectTodos(GetToDoDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<List<ToDoResponseDto>>.Failure("No information provided.");

            if (dto.ProjectId <= 0)
                return ServiceResult<List<ToDoResponseDto>>.Failure("Valid Project id required.");

            try {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.TenantId == _tenantContext.TenantId);
                if (project == null) {
                    _logger.LogWarning("Project with ID {ProjectId} does not exist.", dto.ProjectId);
                    return ServiceResult<List<ToDoResponseDto>>.Failure("Project does not exist.");
                }

                var todos = await _context.Todos
                    .Where(t => t.ProjectId == dto.ProjectId && t.TenantId == _tenantContext.TenantId)
                    .Include(t => t.User)
                    .Include(t => t.SubTasks)
                    .ToListAsync();

                if (!todos.Any()) {
                    _logger.LogInformation("No todo's found for project {ProjectId}.", dto.ProjectId);
                    return ServiceResult<List<ToDoResponseDto>>.Failure("No todo's found for the given project.");
                }

                return ServiceResult<List<ToDoResponseDto>>.Success(
                    todos.Select(t => new ToDoResponseDto {
                        Id = t.Id,
                        IsComplete = t.IsComplete,
                        UserName = t.User?.UserName ?? "Unknown",
                        ProjectId = t.ProjectId,
                        SubTasks = t.SubTasks?
                            .Select(st => new TodoSubTasksResponseDto {
                                Id = st.Id,
                                Description = st.Description,
                                IsCompleted = st.IsCompleted
                            }).ToList() ?? new List<TodoSubTasksResponseDto>()
                    }).ToList(),
                    "Project todo's retrieved successfully."
                );
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving todo's for project {ProjectId}.", dto.ProjectId);
                return ServiceResult<List<ToDoResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving todo's.",
                    new[] { ex.Message });
            }
        }


        // UPDATE service
        // Update todo by id
        public async Task<ServiceResult<ToDoResponseDto>> UpdateTodo(UpdateTodoDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ToDoResponseDto>.Failure("No information provided.");

            if (dto.TodoId <= 0)
                return ServiceResult<ToDoResponseDto>.Failure("Valid ID required.");

            try {
                var todo = await _context.Todos
                    .Include(t => t.User)
                    .Include(t => t.SubTasks)
                    .FirstOrDefaultAsync(t => t.Id == dto.TodoId && t.TenantId == _tenantContext.TenantId);
                if (todo == null) {
                    _logger.LogInformation("Todo with ID {TodoId} not found.", dto.TodoId);
                    return ServiceResult<ToDoResponseDto>.Failure("Todo not found.");
                }

                if (!string.IsNullOrWhiteSpace(dto.UserName)) {
                    var user = await _userManager
                        .FindByNameAsync(dto.UserName.Trim());
                    if (user == null)
                        return ServiceResult<ToDoResponseDto>.Failure("User not found");

                    todo.UserId = user.Id;
                }

                if (!string.IsNullOrWhiteSpace(dto.Task))
                    todo.Task = dto.Task.Trim();

                if (dto.DueDate != default(DateOnly))
                    todo.DueDate = dto.DueDate;

                if (dto.IsComplete.HasValue)
                    todo.IsComplete = dto.IsComplete.Value;

                _context.Todos.Update(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Todo with ID {TodoId} updated successfully by user {UserName}.", dto.TodoId, todo.User?.UserName);

                return ServiceResult<ToDoResponseDto>.Success(
                    new ToDoResponseDto {
                        Id = todo.Id,
                        IsComplete = todo.IsComplete,
                        UserName = todo.User?.UserName ?? "Unknown",
                        ProjectId = todo.ProjectId,
                        SubTasks = todo.SubTasks?
                            .Select(st => new TodoSubTasksResponseDto {
                                Id = st.Id,
                                Description = st.Description,
                                IsCompleted = st.IsCompleted
                            }).ToList() ?? new List<TodoSubTasksResponseDto>()
                    },
                    "Todo updated successfully."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while updating todo for {UserId}", dto.UserName);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "A database error occurred while updating the todo.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while updating todo {UserId}", dto.UserName);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "An unexpected error occurred while updating the todo.",
                    new[] { ex.Message });
            }
        }


        // Mark todo as complete
        public async Task<ServiceResult<ToDoResponseDto>> MarkTodoCompletion(MarkTodoCompletionDto dto) {
            if (dto == null)
                return ServiceResult<ToDoResponseDto>.Failure("No information provided.");
            if (dto.ToDoId <= 0)
                return ServiceResult<ToDoResponseDto>.Failure("Valid ID required.");

            try {
                var todo = await _context.Todos
                    .Include(t => t.User)
                    .Include(t => t.SubTasks)
                    .FirstOrDefaultAsync(t => t.Id == dto.ToDoId && t.TenantId == _tenantContext.TenantId);

                if (todo == null) {
                    _logger.LogInformation("Todo with ID {TodoId} not found.", dto.ToDoId);
                    return ServiceResult<ToDoResponseDto>.Failure("Todo not found.");
                }

                todo.IsComplete = dto.IsComplete;
                await _context.SaveChangesAsync();

                var status = dto.IsComplete ? "complete" : "incomplete";
                _logger.LogInformation("Todo with ID {TodoId} marked as {Status} by user {UserName}.", dto.ToDoId, status, todo.User?.UserName ?? "Unknown");

                return ServiceResult<ToDoResponseDto>.Success(
                    new ToDoResponseDto {
                        Id = todo.Id,
                        IsComplete = todo.IsComplete,
                        UserName = todo.User?.UserName ?? "Unknown",
                        ProjectId = todo.ProjectId,
                        SubTasks = todo.SubTasks?
                            .Select(st => new TodoSubTasksResponseDto {
                                Id = st.Id,
                                Description = st.Description,
                                IsCompleted = st.IsCompleted
                            }).ToList() ?? new List<TodoSubTasksResponseDto>()
                    },
                    $"Todo marked as {status} successfully."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while updating completion status for todo ID {TodoId}", dto.ToDoId);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "A database error occurred while updating the todo completion status.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while updating completion status for todo ID {TodoId}", dto.ToDoId);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "An unexpected error occurred while updating the todo completion status.",
                    new[] { ex.Message });
            }
        }


        // DELETE service
        public async Task<ServiceResult<ToDoResponseDto>> DeleteTodo(GetToDoDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ToDoResponseDto>.Failure("No information provided.");
            if (dto.TodoId <= 0)
                return ServiceResult<ToDoResponseDto>.Failure("Valid ID required.");

            try {
                var todo = await _context.Todos
                    .Include(t => t.User)
                    .Include(t => t.SubTasks)
                    .FirstOrDefaultAsync(t => t.Id == dto.TodoId && t.TenantId == _tenantContext.TenantId);
                if (todo == null) {
                    _logger.LogInformation("Todo with ID {TodoId} not found.", dto.TodoId);
                    return ServiceResult<ToDoResponseDto>.Failure("Todo not found.");
                }

                // Prepare response before deletion
                var response = new ToDoResponseDto {
                    Id = todo.Id,
                    IsComplete = todo.IsComplete,
                    UserName = todo.User?.UserName ?? "Unknown",
                    ProjectId = todo.ProjectId,
                    SubTasks = todo.SubTasks?
                        .Select(st => new TodoSubTasksResponseDto {
                            Id = st.Id,
                            Description = st.Description,
                            IsCompleted = st.IsCompleted
                        }).ToList() ?? new List<TodoSubTasksResponseDto>()
                };

                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Todo with ID {TodoId} deleted successfully by user {UserName}.",
                    dto.TodoId, todo.User?.UserName ?? "Unknown");

                return ServiceResult<ToDoResponseDto>.Success(response, "Todo deleted successfully.");
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while deleting todo ID {TodoId}", dto.TodoId);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "A database error occurred while deleting the todo.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while deleting todo ID {TodoId}", dto.TodoId);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "An unexpected error occurred while deleting the todo.",
                    new[] { ex.Message });
            }
        }
    }
}
