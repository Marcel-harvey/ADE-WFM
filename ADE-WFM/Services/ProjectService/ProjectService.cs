using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace ADE_WFM.Services.ProjectService
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ProjectService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        // ADD services
        public async Task<ServiceResult<CreateProjectResponseDto>> CreateProject(CreateProjectDto dto)
        {
            if (dto == null)
                return ServiceResult<CreateProjectResponseDto>.Failure("CreateProjectDto cannot be null");

            if (string.IsNullOrWhiteSpace(dto.ProjectTitle))
                return ServiceResult<CreateProjectResponseDto>.Failure("Project title cannot be empty");

            if (string.IsNullOrWhiteSpace(dto.CurrentUserId))
                return ServiceResult<CreateProjectResponseDto>.Failure("Current user ID cannot be empty");

            try
            {
                var project = new Project
                {
                    ProjectTitle = dto.ProjectTitle,
                    ProjectDescription = dto.ProjectDescription,
                    DueDate = dto.DueDate,
                    WorkFlowId = dto.WorkFlowId,
                    DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                    ProjectUsers = new List<ProjectUser>(),
               };

                // Add user that created project
                project.ProjectUsers.Add(new ProjectUser
                {
                    UserId = dto.CurrentUserId,
                });

                // Add list of selected user after checking if already exists
                foreach (var userId in dto.UserIds)
                {
                    if (userId != dto.CurrentUserId)
                    {
                        project.ProjectUsers.Add(new ProjectUser
                        {
                            UserId = userId,
                        });
                    }
                }

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Project {ProjectTitle} was created successfully by {UserId} ", project.ProjectTitle, dto.CurrentUserId);

                return ServiceResult<CreateProjectResponseDto>.Success(
                    new CreateProjectResponseDto
                    {
                        Title = project.ProjectTitle,
                        Description = project.ProjectDescription ?? null,
                        DateCreated = project.DateCreated.ToDateTime(TimeOnly.MinValue),
                        DueDate = project.DueDate.ToDateTime(TimeOnly.MinValue),
                        AssignedUserIds = project.ProjectUsers.Select(pu => pu.UserId).ToList()
                    }
                );
            }
            // Database exceptions
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating workflow '{WorkFlowName}'", dto.ProjectTitle);
                return ServiceResult<CreateProjectResponseDto>.Failure(
                    "A database error occurred while creating the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating workflow '{WorkFlowName}'", dto.ProjectTitle);
                return ServiceResult<CreateProjectResponseDto>.Failure(
                    "An unexpected error occurred while creating the workflow.",
                    new[] { ex.Message });
            }
        }


        // GET services

        // Get project by id
        public async Task<ServiceResult<List<GetProjectByIdResponseDto>>> GetProjectById(GetProjectByIdDto dto)
        {
            if (dto == null)
            {
                return ServiceResult<List<GetProjectByIdResponseDto>>.Failure("GetAllProjectsDto cannot be null");
            }

            if (dto.Id < 0)
            {
                return ServiceResult<List<GetProjectByIdResponseDto>>.Failure("Invalid ID provided");
            }

            try
            {
                var projects = await _context.Projects
                    .Include(pu => pu.ProjectUsers)
                        .ThenInclude(u => u.User)
                    .ToListAsync();

                if (projects == null ||!projects.Any())
                {
                    _logger.LogWarning("No projects found in the database.");
                    return ServiceResult<List<GetProjectByIdResponseDto>>.Failure("No projects found");
                }

                _logger.LogInformation("Successfully retrieved all projects.");

                return ServiceResult<List<GetProjectByIdResponseDto>>.Success(
                    projects.Select(p => new GetProjectByIdResponseDto
                    {
                        Id = p.Id,
                        Title = p.ProjectTitle,
                        Description = p.ProjectDescription,
                        DueDate = p.DueDate,
                        DateCreated = p.DateCreated,
                        Users = p.ProjectUsers.Select(u => new GetProjectUsersDto
                        {
                            Id = u.UserId,
                            UserName = u.User.UserName ?? "",
                        }).ToList() ?? new List<GetProjectUsersDto>()
                    }).ToList()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflows.");

                return ServiceResult<List<GetProjectByIdResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving workflows.",
                    new[] { ex.Message });
            }
        }


        // Get all users involved in project
        public async Task<List<ApplicationUser>> GetUsersInProject(int projectId)
        {
            var projectUsers = await _context.ProjectUsers
                .Where(project => project.ProjectId == projectId)
                .Select(user => user.User)
                .ToListAsync();

            return projectUsers;
        }

        // UPDATE services

        
        // DELETE services


        // UPDATE API services
        // Update project title
        public async Task<Project> UpdateProjectTitle(UpdateProjectTitleDto model)
        {
            var project = await _context.Projects
                .FindAsync(model.projectId)
                ?? throw new KeyNotFoundException($"Project with ID: {model.projectId} was not found");

            // Check if the project title is not empty
            if (string.IsNullOrWhiteSpace(model.newProjectTitle))
            {
                throw new ArgumentException("Project title cannot be empty", nameof(model.newProjectTitle));
            }

            project.ProjectTitle = model.newProjectTitle;
            await _context.SaveChangesAsync();

            return project;
        }


        // Update project description
        public async Task<Project> UpdateProjectDescription(UpdateProjectDescriptionDto model)
        {
            var project = await _context.Projects
                .FindAsync(model.projectId)
                ?? throw new KeyNotFoundException($"Project with ID: {model.projectId} was not found");

            // Check if the new project description is not null
            if (model.newProjectDescription == null)
            {
                throw new ArgumentNullException(nameof(model.newProjectDescription), "Project description cannot be null");
            }

            project.ProjectDescription = model.newProjectDescription;
            await _context.SaveChangesAsync();

            return project;
        }


        // Update project due date
        public async Task<Project> UpdateProjectDueDate(UpdateProjectDueDateDto model)
        {
            var project = await _context.Projects
                .FindAsync(model.ProjectId)
                ?? throw new KeyNotFoundException($"Project with ID: {model.ProjectId} was not found");

            // Check if the new due date is not in the past
            if (model.NewDueDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException("Due date cannot be in the past", nameof(model.NewDueDate));
            }

            project.DueDate = model.NewDueDate;
            await _context.SaveChangesAsync();

            return project;
        }


        // ADD API services
        // Add new user to project
        public async Task<ApplicationUser> AddUserToProject(AddUserProjectDto model)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectUsers)
                .FirstOrDefaultAsync(p => p.Id == model.ProjectId)
                ?? throw new KeyNotFoundException($"Project with ID: {model.ProjectId} was not found");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == model.UserId)
                ?? throw new KeyNotFoundException($"User with ID: {model.UserId} was not found");

            // Check if the user is already in the project
            if (project.ProjectUsers.Any(pu => pu.UserId == model.UserId))
            {
                throw new InvalidOperationException($"User with ID: {model.UserId} is already in the project");
            }

            var projectUser = new ProjectUser
            {
                ProjectId = model.ProjectId,
                UserId = model.UserId
            };

            await _context.ProjectUsers.AddAsync(projectUser);
            await _context.SaveChangesAsync();

            return user;
        }


        // DELETE API services
        // Delete project
        public async Task DeleteProject(DeleteProjectDto dto)
        {
            // Check if the dto is null
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "DeleteProjectDto cannot be null");
            }

            // Find the project to be deleted
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId)
                ?? throw new KeyNotFoundException($"Project with ID: {dto.ProjectId} was not found");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        // Remove user from project
        public async Task RemoveUserFromProject(RemoveUserFromProjectDto dto)
        {
            // Check if the dto is null
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "RemoveUserFromProjectDto cannot be null");
            }

            // Find the project user association
            var projectUser = await _context.ProjectUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == dto.ProjectId && pu.UserId == dto.UserId)
                ?? throw new KeyNotFoundException($"User with ID: {dto.UserId} is not associated with Project ID: {dto.ProjectId}");

            // Find the user to be removed
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == dto.UserId)
                ?? throw new KeyNotFoundException($"User with ID: {dto.UserId} was not found");

            _context.ProjectUsers.Remove(projectUser);
            await _context.SaveChangesAsync();                  
        }
    }
}
