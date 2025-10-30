using ADE_WFM.Models;
using ADE_WFM.Data;
using Microsoft.EntityFrameworkCore;
using ADE_WFM.Models.DTOs.TodoDtos;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;
using Microsoft.AspNetCore.Identity;

namespace ADE_WFM.Services.TodoService
{
    public class TodoService : ITodoService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TodoService> _logger;

        public TodoService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<TodoService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        // ADD service
        public async Task<ServiceResult<ToDoResponseDto>> AddTodo(AddTodoDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<ToDoResponseDto>.Failure("Input data is null.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<ToDoResponseDto>.Failure("User id required.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                return ServiceResult<ToDoResponseDto>.Failure("Title is required.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                return ServiceResult<ToDoResponseDto>.Failure("Description is required.");

            var user = await _userManager
                .FindByIdAsync(dto.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} does not exist.", dto.UserId);
                return ServiceResult<ToDoResponseDto>.Failure("User does not exist.");
            }

            try
            {
                var todo = new Todo
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    DateCreated = DateTime.UtcNow,
                    IsComplete = false,
                    UserId = dto.UserId,
                    ProjectId = dto.ProjectId,
                };

                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Todo with ID {TodoId} created successfully for user ID {UserId}.", todo.Id, dto.UserId);

                return ServiceResult<ToDoResponseDto>.Success(
                    new ToDoResponseDto
                    {
                        Id = todo.Id,
                        IsComplete = todo.IsComplete,
                        Title = todo.Title,
                        Description = todo.Description,
                        DateCreated = todo.DateCreated,
                        DueDate = todo.DueDate,
                        UserName = user.UserName ?? "Unknown",
                        ProjectId = todo.ProjectId
                    },
                    "Todo created successfully."
                    );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding todo {TodoTitle}", dto.Title);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "A database error occurred while adding the todo.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding todo {TodoTitle}", dto.Title);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "An unexpected error occurred while adding the todo.",
                    new[] { ex.Message });
            }
        }


        // GET service
        // Get all todos for a user
        public async Task<ServiceResult<List<ToDoResponseDto>>> GetAllUserTodos(GetToDoDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<List<ToDoResponseDto>>.Failure("Input data is null.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<List<ToDoResponseDto>>.Failure("User id required.");

            try
            {
                var todos = await _context.Todos
                    .Where(t => t.UserId == dto.UserId)
                    .Include(t => t.User)
                    .Include(t => t.SubTasks)
                    .ToListAsync();

                if (!todos.Any())
                {
                    _logger.LogInformation("No todo's found for user {UserId}.", dto.UserId);
                    return ServiceResult<List<ToDoResponseDto>>.Failure("No todo's found for the given user.");
                }

                return ServiceResult<List<ToDoResponseDto>>.Success(
                    todos.Select(t => new ToDoResponseDto
                    {
                        Id = t.Id,
                        IsComplete = t.IsComplete,
                        Title = t.Title,
                        Description = t.Description,
                        DateCreated = t.DateCreated,
                        DueDate = t.DueDate,
                        UserName = t.User.UserName ?? "Unknown",
                        ProjectId = t.ProjectId
                    }).ToList(),
                    "User todo's retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todo's for user {UserId}.", dto.UserId);

                return ServiceResult<List<ToDoResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving todo's.",
                    new[] { ex.Message });
            }
        }


        // Get all todos for a project
        public async Task<ServiceResult<List<ToDoResponseDto>>> GetAllProjectTodos(GetToDoDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<List<ToDoResponseDto>>.Failure("Input data is null.");

            if (dto.ProjectId <= 0)
                return ServiceResult<List<ToDoResponseDto>>.Failure("Valid Project id required.");

            var project = await _context.Projects
                .FindAsync(dto.ProjectId);
            if (project == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist.", dto.ProjectId);
                return ServiceResult<List<ToDoResponseDto>>.Failure("Project does not exist.");
            }                

            try
            {
                var todos = await _context.Todos
                    .Where(t => t.ProjectId == dto.ProjectId)
                    .Include(t => t.User)
                    .Include(t => t.SubTasks)
                    .ToListAsync();

                if (!todos.Any())
                {
                    _logger.LogInformation("No todo's found for project {ProjectId}.", dto.ProjectId);
                    return ServiceResult<List<ToDoResponseDto>>.Failure("No todo's found for the given project.");
                }

                return ServiceResult<List<ToDoResponseDto>>.Success(
                    todos.Select(t => new ToDoResponseDto
                    {
                        Id = t.Id,
                        IsComplete = t.IsComplete,
                        Title = t.Title,
                        Description = t.Description,
                        DateCreated = t.DateCreated,
                        DueDate = t.DueDate,
                        UserName = t.User.UserName ?? "Unknown",
                        ProjectId = t.ProjectId
                    }).ToList(),
                    "Project todo's retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todo's for project {ProjectId}.", dto.ProjectId);

                return ServiceResult<List<ToDoResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving todo's.",
                    new[] { ex.Message });
            }
        }


        // UPDATE service
        // Update todo by id
        public async Task<ServiceResult<ToDoResponseDto>> UpdateTodo(UpdateTodoDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<ToDoResponseDto>.Failure("Input data is null.");

            if (dto.TodoId <= 0)
                return ServiceResult<ToDoResponseDto>.Failure("Valid Todo id required.");

            var todo = await _context.Todos
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == dto.TodoId);
            if ( todo == null)
            {
                _logger.LogInformation( "Todo with ID {TodoId} not found.", dto.TodoId);
                return ServiceResult<ToDoResponseDto>.Failure("Todo not found.");
            }

            try
            {

                if (!string.IsNullOrWhiteSpace(dto.Title))
                    todo.Title = dto.Title.Trim();

                if (!string.IsNullOrWhiteSpace(dto.Description))
                    todo.Description = dto.Description.Trim();

                if (dto.DueDate != default(DateTime))
                    todo.DueDate = dto.DueDate;

                _context.Todos.Update(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Todo with ID {TodoId} updated successfully by user {UserName}.", dto.TodoId, todo.User.UserName);

                return ServiceResult<ToDoResponseDto>.Success(
                    new ToDoResponseDto
                    {
                        Id = todo.Id,
                        IsComplete = todo.IsComplete,
                        Title = todo.Title,
                        Description = todo.Description,
                        DateCreated = todo.DateCreated,
                        DueDate = todo.DueDate,
                        UserName = todo.User.UserName ?? "Unknown",
                        ProjectId = todo.ProjectId
                    },
                    "Todo updated successfully."
                    );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating todo {TodoTitle}", dto.Title);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "A database error occurred while updating the todo.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating todo {TodoTitle}", dto.Title);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "An unexpected error occurred while updating the todo.",
                    new[] { ex.Message });
            }
        }


        // DELETE service
        public async Task<ServiceResult<ToDoResponseDto>> DeleteTodo(GetToDoDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<ToDoResponseDto>.Failure("Input data is null.");

            if (dto.ToDoId <= 0)
                return ServiceResult<ToDoResponseDto>.Failure("Valid Todo id required.");

            try
            {
                var todo = await _context.Todos
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == dto.ToDoId);

                if (todo == null)
                {
                    _logger.LogInformation("Todo with ID {TodoId} not found.", dto.ToDoId);
                    return ServiceResult<ToDoResponseDto>.Failure("Todo not found.");
                }

                var response = new ToDoResponseDto
                {
                    Id = todo.Id,
                    IsComplete = todo.IsComplete,
                    Title = todo.Title,
                    Description = todo.Description,
                    DateCreated = todo.DateCreated,
                    DueDate = todo.DueDate,
                    UserName = todo.User?.UserName ?? "Unknown",
                    ProjectId = todo.ProjectId
                };

                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Todo with ID {TodoId} deleted successfully by user {UserName}.",
                    dto.ToDoId, todo.User?.UserName ?? "Unknown");

                return ServiceResult<ToDoResponseDto>.Success(response, "Todo deleted successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting todo ID {TodoId}", dto.ToDoId);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "A database error occurred while deleting the todo.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting todo ID {TodoId}", dto.ToDoId);
                return ServiceResult<ToDoResponseDto>.Failure(
                    "An unexpected error occurred while deleting the todo.",
                    new[] { ex.Message });
            }
        }

    }
}
