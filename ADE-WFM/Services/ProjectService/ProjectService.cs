using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
        // Create a new project
        public async Task<ServiceResult<CreateProjectResponseDto>> CreateProject(CreateProjectDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<CreateProjectResponseDto>.Failure("CreateProjectDto cannot be null");

            if (string.IsNullOrWhiteSpace(dto.ProjectTitle))
                return ServiceResult<CreateProjectResponseDto>.Failure("Project title cannot be empty");

            if (string.IsNullOrWhiteSpace(dto.CurrentUserId))
                return ServiceResult<CreateProjectResponseDto>.Failure("Current user ID cannot be empty");

            if (string.IsNullOrWhiteSpace(dto.WorkFlowId.ToString()))
                return ServiceResult<CreateProjectResponseDto>.Failure("Workflow ID cannot be empty");

            // Get users in work flow - can not add users outside of work flow
            var workFlow = await _context.WorkFlows
                .Include(u => u.WorkFlowUsers)
                    .ThenInclude(wfu => wfu.User)
                .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId);

            if (workFlow == null)
                return ServiceResult<CreateProjectResponseDto>.Failure($"Workflow with ID {dto.WorkFlowId} does not exist");

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

                // Lists for adding added and skipped users when iterating through provided user IDs
                var addedUsers = new List<ProjectUsersInfoDto>();
                var skippedUsers = new List<ProjectUsersInfoDto>();

                // Add creator of project
                project.ProjectUsers.Add(new ProjectUser { UserId = dto.CurrentUserId });

                var creator = await _context.Users
                    .FindAsync(dto.CurrentUserId);

                addedUsers.Add(new ProjectUsersInfoDto
                {
                    UserId = dto.CurrentUserId,
                    UserName = creator?.UserName ?? "Unknown"
                });

                // Prepare lookup of all users in workflow
                var workflowUserIds = workFlow.WorkFlowUsers.Select(wfu => wfu.UserId).ToHashSet();

                // Add valid users and collect skipped ones
                foreach (var userId in dto.UserIds)
                {
                    if (userId == dto.CurrentUserId)
                        continue;

                    var user = await _context.Users.FindAsync(userId);
                    var userName = user?.UserName ?? "Unknown";

                    if (workflowUserIds.Contains(userId))
                    {
                        project.ProjectUsers.Add(new ProjectUser { UserId = userId });
                        addedUsers.Add(new ProjectUsersInfoDto
                        {
                            UserId = userId,
                            UserName = userName
                        });
                    }
                    else
                    {
                        skippedUsers.Add(new ProjectUsersInfoDto
                        {
                            UserId = userId,
                            UserName = userName
                        });
                    }
                }

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Project {ProjectTitle} created successfully by {UserId}. Added {AddedCount} users, skipped {SkippedCount}",
                    project.ProjectTitle, dto.CurrentUserId, addedUsers.Count, skippedUsers.Count
                );

                return ServiceResult<CreateProjectResponseDto>.Success(new CreateProjectResponseDto
                {
                    WorkFlowName = workFlow.WorkFlowName,
                    Title = project.ProjectTitle,
                    Description = project.ProjectDescription ?? null,
                    DateCreated = project.DateCreated.ToDateTime(TimeOnly.MinValue),
                    DueDate = project.DueDate.ToDateTime(TimeOnly.MinValue),
                    AddedUsers = addedUsers,
                    SkippedUsers = skippedUsers
                },
                $"Created project {project.ProjectTitle} successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating project '{projectName}'", dto.ProjectTitle);
                return ServiceResult<CreateProjectResponseDto>.Failure(
                    "A database error occurred while creating the project.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating project '{projectName}'", dto.ProjectTitle);
                return ServiceResult<CreateProjectResponseDto>.Failure(
                    "An unexpected error occurred while creating the project.",
                    new[] { ex.Message });
            }
        }


        // Add new user to project
        public async Task<ServiceResult<ProjectUsersInfoDto>> AddUserToProject(AddUserToProjectDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<ProjectUsersInfoDto>.Failure("AddUserToProjectDto cannot be null");

            if (dto.ProjectId <= 0)
                return ServiceResult<ProjectUsersInfoDto>.Failure("Invalid Project ID provided");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<ProjectUsersInfoDto>.Failure("User ID cannot be empty");

            var project = await _context.Projects
                .Include(p => p.ProjectUsers)
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

            if (project == null)
                return ServiceResult<ProjectUsersInfoDto>.Failure($"Project with ID: {dto.ProjectId} was not found");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
                return ServiceResult<ProjectUsersInfoDto>.Failure($"User with ID: {dto.UserId} was not found");

            // Get users in workflow - cannot add users outside of workflow
            var workFlow = await _context.WorkFlows
                .Include(wf => wf.WorkFlowUsers)
                .FirstOrDefaultAsync(wf => wf.Id == project.WorkFlowId);

            if (workFlow == null)
                return ServiceResult<ProjectUsersInfoDto>.Failure($"Workflow with ID {project.WorkFlowId} does not exist");

            try
            {
                // Ensure user is part of the project's workflow
                var workflowUserIds = workFlow.WorkFlowUsers.Select(wfu => wfu.UserId).ToHashSet();

                if (!workflowUserIds.Contains(dto.UserId))
                {
                    return ServiceResult<ProjectUsersInfoDto>.Failure($"User '{user.UserName}' cannot be added because they are not part of the workflow '{workFlow.WorkFlowName}'.");
                }

                // Prevent duplicate users
                if (project.ProjectUsers.Any(pu => pu.UserId == dto.UserId))
                {
                    return ServiceResult<ProjectUsersInfoDto>.Failure(
                        $"User '{user.UserName}' is already part of this project."
                    );
                }

                // Add new user to project
                var projectUser = new ProjectUser
                {
                    ProjectId = dto.ProjectId,
                    UserId = dto.UserId
                };

                await _context.ProjectUsers.AddAsync(projectUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User '{UserName}' added to project '{ProjectTitle}' (ProjectId: {ProjectId})",
                    user.UserName, project.ProjectTitle, project.Id);

                return ServiceResult<ProjectUsersInfoDto>.Success(new ProjectUsersInfoDto
                {
                    ProjectId = dto.ProjectId,
                    UserId = dto.UserId,
                    UserName = user?.UserName ?? "Unknown"
                },
                $"Added user {user?.UserName ?? "Unknown"} to project successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding user '{UserName}' to project '{ProjectTitle}'",
                    user.UserName, project.ProjectTitle);
                return ServiceResult<ProjectUsersInfoDto>.Failure(
                    "A database error occurred while adding the user.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding user '{UserName}' to project '{ProjectTitle}'",
                    user.UserName, project.ProjectTitle);
                return ServiceResult<ProjectUsersInfoDto>.Failure(
                    "An unexpected error occurred while adding the user.",
                    new[] { ex.Message });
            }
        }


        // GET services
        // Get all projects
        public async Task<ServiceResult<List<GetProjectResponseDto>>> GetAllProjects()
        {
            try
            {
                var projects = await _context.Projects
                    .Include(p => p.WorkFlows)
                    .Include(pu => pu.ProjectUsers)
                        .ThenInclude(u => u.User)
                    .OrderByDescending(p => p.DateCreated)
                    .ToListAsync();

                if (!projects.Any())
                {
                    _logger.LogWarning("No projects found in the database.");
                    return ServiceResult<List<GetProjectResponseDto>>.Failure("No projects found");
                }

                return ServiceResult<List<GetProjectResponseDto>>.Success(
                    projects.Select(p => new GetProjectResponseDto
                    {
                        WorkFlowName = p.WorkFlows.WorkFlowName,
                        ProjectTitle = p.ProjectTitle,
                        Description = p.ProjectDescription,
                        DueDate = p.DueDate,
                        DateCreated = p.DateCreated,
                        Users = p.ProjectUsers.Select(u => new ProjectUsersInfoDto
                        {
                            ProjectId = p.Id,
                            UserId = u.UserId,
                            UserName = u.User.UserName ?? string.Empty,
                        }).ToList()
                    }).ToList(),
                    $"Retrieved {projects.Count} projects successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects.");

                return ServiceResult<List<GetProjectResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving projects.",
                    new[] { ex.Message });
            }
        }


        // Get project by id
        public async Task<ServiceResult<GetProjectResponseDto>> GetProjectById(GetProjectByIdDto dto)
        {
            if (dto == null)
                return ServiceResult<GetProjectResponseDto>.Failure("GetProjectByIdDto cannot be null");

            if (dto.Id <= 0)
                return ServiceResult<GetProjectResponseDto>.Failure("Invalid project ID provided");

            try
            {
                var project = await _context.Projects
                    .Include(p => p.ProjectUsers)
                        .ThenInclude(pu => pu.User)
                    .FirstOrDefaultAsync(p => p.Id == dto.Id);

                if (project == null)
                {
                    _logger.LogWarning("Project with ID {ProjectId} not found.", dto.Id);
                    return ServiceResult<GetProjectResponseDto>.Failure("Project not found");
                }

                var response = new GetProjectResponseDto
                {
                    ProjectTitle = project.ProjectTitle,
                    Description = project.ProjectDescription,
                    DueDate = project.DueDate,
                    DateCreated = project.DateCreated,
                    Users = project.ProjectUsers.Select(u => new ProjectUsersInfoDto
                    {
                        ProjectId = project.Id,
                        UserId = u.UserId,
                        UserName = u.User?.UserName ?? string.Empty
                    }).ToList()
                };

                _logger.LogInformation("Successfully retrieved project ID {ProjectId}.", dto.Id);
                return ServiceResult<GetProjectResponseDto>.Success(
                    response,
                    $"Retrieved project {project.ProjectTitle} successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project ID {ProjectId}", dto.Id);
                return ServiceResult<GetProjectResponseDto>.Failure(
                    "An unexpected error occurred while retrieving the project.",
                    new[] { ex.Message });
            }
        }



        // Get all users involved in project
        public async Task<ServiceResult<GetProjectUsersResponseDto>> GetUsersInProject(GetProjectUsersDto dto)
        {
            if (dto == null)
                return ServiceResult<GetProjectUsersResponseDto>.Failure("GetProjectUsersDto cannot be null");

            if (dto.Id <= 0)
                return ServiceResult<GetProjectUsersResponseDto>.Failure($"Invalid project ID provided: {dto.Id}");

            try
            {
                var projectUsers = await _context.ProjectUsers
                    .Where(pu => pu.ProjectId == dto.Id)
                    .Include(u => u.User)
                    .ToListAsync();

                if (!projectUsers.Any())
                {
                    _logger.LogWarning("No users found for project ID {ProjectId}", dto.Id);
                    return ServiceResult<GetProjectUsersResponseDto>.Failure("No users found for the specified project");
                }

                var response = new GetProjectUsersResponseDto
                {
                    Users = projectUsers.Select(u => new ProjectUsersInfoDto
                    {
                        UserId = u.UserId,
                        UserName = u.User?.UserName ?? string.Empty
                    }).ToList()
                };

                return ServiceResult<GetProjectUsersResponseDto>.Success(
                    response,
                    $"Retrieved {response.Users.Count} users for project ID {dto.Id} successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users for project ID {ProjectId}", dto.Id);
                return ServiceResult<GetProjectUsersResponseDto>.Failure(
                    "An unexpected error occurred while retrieving users in the project.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services
        // Update project info
        public async Task<ServiceResult<UpdateProjectInfoResponseDto>> UpdateProjectInfo(UpdateProjectInfoDto dto)
        {
            if (dto == null)
                return ServiceResult<UpdateProjectInfoResponseDto>.Failure("UpdateProjectInfoDto cannot be null");

            if (dto.ProjectId <= 0)
                return ServiceResult<UpdateProjectInfoResponseDto>.Failure("Invalid Project ID provided");


            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
            if (project == null)
                return ServiceResult<UpdateProjectInfoResponseDto>.Failure($"Project with ID: {dto.ProjectId} was not found");

            try
            {
                // Update fields if provided
                if (!string.IsNullOrWhiteSpace(dto.Title))
                    project.ProjectTitle = dto.Title;

                if (dto.Description != null)
                    project.ProjectDescription = dto.Description;

                if (dto.DueDate.HasValue)
                    project.DueDate = dto.DueDate.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Project ID {ProjectId} updated successfully", dto.ProjectId);

                return ServiceResult<UpdateProjectInfoResponseDto>.Success(
                    new UpdateProjectInfoResponseDto
                    {
                        ProjectId = project.Id,
                        Title = project.ProjectTitle,
                        Description = project.ProjectDescription,
                        DueDate = project.DueDate
                    },
                    $"Updated project successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating project ID {ProjectId}", dto.ProjectId);
                return ServiceResult<UpdateProjectInfoResponseDto>.Failure(
                    "A database error occurred while updating the project.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating project ID {ProjectId}", dto.ProjectId);
                return ServiceResult<UpdateProjectInfoResponseDto>.Failure(
                    "An unexpected error occurred while updating the project.",
                    new[] { ex.Message });
            }
        }


        // DELETE services
        // Delete project
        public async Task<ServiceResult<DeleteProjectResponseDto>> DeleteProject(DeleteProjectDto dto)
        {
            if (dto == null)
                return ServiceResult<DeleteProjectResponseDto>.Failure("DeleteProjectDto cannot be null");

            if (dto.ProjectId <= 0)
                return ServiceResult<DeleteProjectResponseDto>.Failure("Invalid Project ID provided");

            var project = await _context.Projects
                .FindAsync(dto.ProjectId);
            if (project == null)
                return ServiceResult<DeleteProjectResponseDto>.Failure($"Project with ID: {dto.ProjectId} was not found");

            try
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Project ID {ProjectId} deleted successfully", dto.ProjectId);

                return ServiceResult<DeleteProjectResponseDto>.Success(
                    new DeleteProjectResponseDto
                    {
                        ProjectTitle = project.ProjectTitle
                    }, $"Deleted project {project.ProjectTitle} successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting project ID {ProjectId}", dto.ProjectId);
                return ServiceResult<DeleteProjectResponseDto>.Failure(
                    "A database error occurred while deleting the project.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting project ID {ProjectId}", dto.ProjectId);
                return ServiceResult<DeleteProjectResponseDto>.Failure(
                    "An unexpected error occurred while deleting the project.",
                    new[] { ex.Message });
            }
        }


        // Remove user from project
        public async Task<ServiceResult<ProjectUsersInfoDto>> RemoveUserFromProject(RemoveUserFromProjectDto dto)
        {
            if (dto == null)
                return ServiceResult<ProjectUsersInfoDto>.Failure("RemoveUserFromProjectDto cannot be null");

            if (dto.ProjectId <= 0)
                return ServiceResult<ProjectUsersInfoDto>.Failure("Invalid Project ID provided");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<ProjectUsersInfoDto>.Failure("User ID cannot be empty");

            var projectUser = await _context.ProjectUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == dto.ProjectId && pu.UserId == dto.UserId);
            if (projectUser == null)
                return ServiceResult<ProjectUsersInfoDto>.Failure($"User with ID: {dto.UserId} is not part of project ID: {dto.ProjectId}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null)
                return ServiceResult<ProjectUsersInfoDto>.Failure($"User with ID: {dto.UserId} was not found");

            try
            {
                // Check if user is last user in project
                var projectUsersCount = await _context.ProjectUsers.CountAsync(pu => pu.ProjectId == dto.ProjectId);
                if (projectUsersCount <= 1)
                    return ServiceResult<ProjectUsersInfoDto>.Failure("Cannot remove the last user from the project");


                _context.ProjectUsers.Remove(projectUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User ID {UserId} removed from project ID {ProjectId} successfully", dto.UserId, dto.ProjectId);
                return ServiceResult<ProjectUsersInfoDto>.Success(
                    new ProjectUsersInfoDto
                    {
                        ProjectId = dto.ProjectId,
                        UserId = user.Id,
                        UserName = user.UserName ?? "Unknown"
                    },
                    $"Removed user {user.UserName ?? "Unknown"} from project successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting user ID {userId} from project ID {projectId}", dto.UserId, dto.ProjectId);
                return ServiceResult<ProjectUsersInfoDto>.Failure(
                    "A database error occurred while deleting the the user from the project.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting user ID {userId} from project ID {projectId}", dto.UserId, dto.ProjectId);
                return ServiceResult<ProjectUsersInfoDto>.Failure(
                    "An unexpected error occurred while deleting the the user from the project.",
                    new[] { ex.Message });
            }
        }
    }
}
