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
        public async Task AddTodo(AddTodoDto dto)
        {
            var todos = await _context.Todos
                .Include(user => user.User)
                .Include(subTasks => subTasks.SubTasks)
                .ToListAsync();
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
                        UserId = t.UserId,
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
                        UserId = t.UserId,
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

                
        // UPDATE service
        // Update the title of todo
        public async Task UpdateTodoTitle(UpdateTodoTitleDto dto)
        {
            var todo = await _context.Todos.FindAsync(dto.TodoId)
                ?? throw new KeyNotFoundException($"Todo with ID {dto.TodoId} not found.");

            todo.Title = dto.NewTitle;

            await _context.SaveChangesAsync();
        }


        // Update the description of todo
        public async Task UpdateTodoDescription(UpdateTodoDescriptionDto dto)
        {
            var todo = await _context.Todos.FindAsync(dto.TodoId)
                ?? throw new KeyNotFoundException($"Todo with ID {dto.TodoId} not found.");

            todo.Description = dto.NewDescription;

            await _context.SaveChangesAsync();
        }


        // Update the due date of todo
        public async Task UpdateTodoDueDate(UpdateTodoDueDateDto dto)
        {
            var todo = await _context.Todos.FindAsync(dto.TodoId)
                ?? throw new KeyNotFoundException($"Todo with ID {dto.TodoId} not found.");

            todo.DueDate = dto.NewDueDate;

            await _context.SaveChangesAsync();
        }

        // DELETE service
        public async Task DeleteTodo(DeleteTodoDto dto)
        {
            var todo = await _context.Todos.FindAsync(dto.TodoId)
                ?? throw new KeyNotFoundException($"Todo with ID {dto.TodoId} not found.");

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }
    }
}
