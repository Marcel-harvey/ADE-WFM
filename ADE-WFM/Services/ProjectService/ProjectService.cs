using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;
using ADE_WFM.Services.TenantService;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.ProjectService {
    public class ProjectService : IProjectService {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProjectService> _logger;
        private readonly TenantContext _tenantContext;

        public ProjectService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ProjectService> logger,
            TenantContext tenantContext) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _tenantContext = tenantContext;
        }


        // ADD services
        // Create a new project
        public async Task<ServiceResult<ProjectResponseDto>> CreateProject(CreateProjectDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ProjectResponseDto>.Failure("No information provided");

            if (string.IsNullOrWhiteSpace(dto.ProjectTitle))
                return ServiceResult<ProjectResponseDto>.Failure("Project title cannot be empty");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<ProjectResponseDto>.Failure("Workflow ID cannot be empty");

            try {
                // Get users in work flow - can not add users outside of work flow
                var workFlow = await _context.WorkFlows
                    .Include(u => u.WorkFlowUsers)
                        .ThenInclude(wfu => wfu.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId && wf.TenantId == _tenantContext.TenantId);
                if (workFlow == null)
                    return ServiceResult<ProjectResponseDto>.Failure($"Workflow with ID {dto.WorkFlowId} does not exist");

                // Models.Project use for some weird error
                var project = new Models.Project {
                    ProjectTitle = dto.ProjectTitle,
                    ProjectDescription = dto.ProjectDescription,
                    DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                    DueDate = dto.DueDate,
                    WorkFlowId = dto.WorkFlowId,
                    ProjectUsers = new List<ProjectUser>(),
                    TenantId = _tenantContext.TenantId
                };

                // Lists for adding added and skipped users when iterating through provided user IDs
                var addedUsers = new List<ProjectUsersInfoDto>();
                var skippedUsers = new List<ProjectUsersInfoDto>();

                // Add creator of project
                project.ProjectUsers.Add(new ProjectUser { UserId = _tenantContext.UserId });

                // Get the user entity for username
                var creator = await _context.Users
                    .FindAsync(_tenantContext.UserId);

                addedUsers.Add(new ProjectUsersInfoDto {
                    UserId = _tenantContext.UserId,
                    UserName = creator?.UserName ?? "Unknown"
                });

                // Prepare lookup of all users in workflow
                var workflowUserIds = workFlow.WorkFlowUsers
                    .Select(wfu => wfu.UserId)
                    .ToHashSet();

                // Add valid users and collect skipped ones
                foreach (var userId in dto.UserIds) {
                    if (userId == _tenantContext.UserId)
                        continue;

                    var user = await _context.Users.FindAsync(userId);
                    var userName = user?.UserName ?? "Unknown";

                    if (workflowUserIds.Contains(userId)) {
                        project.ProjectUsers.Add(new ProjectUser { UserId = userId });
                        addedUsers.Add(new ProjectUsersInfoDto {
                            UserId = userId,
                            UserName = userName
                        });
                    }
                    else {
                        skippedUsers.Add(new ProjectUsersInfoDto {
                            UserId = userId,
                            UserName = userName
                        });
                    }
                }

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Project '{ProjectTitle}' created successfully by {UserId}. Added {AddedCount} users, skipped {SkippedCount}",
                    project.ProjectTitle, _tenantContext.UserId, addedUsers.Count, skippedUsers.Count
                );

                return ServiceResult<ProjectResponseDto>.Success(
                    new ProjectResponseDto {
                        ProjectId = project.Id,
                        ProjectTitle = project.ProjectTitle,
                        ProjectDescription = project.ProjectDescription ?? null,
                        DateCreated = project.DateCreated,
                        DueDate = project.DueDate,
                        WorkFlowId = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                        Users = addedUsers,
                        SkippedUsers = skippedUsers
                    },
                    $"Created project '{project.ProjectTitle}' successfully"
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while creating project '{projectName}'", dto.ProjectTitle);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "A database error occurred while creating the project.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while creating project '{projectName}'", dto.ProjectTitle);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "An unexpected error occurred while creating the project.",
                    new[] { ex.Message });
            }
        }


        // Add new user to project
        public async Task<ServiceResult<ProjectResponseDto>> AddUserToProject(AddUserToProjectDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ProjectResponseDto>.Failure("No information provided");

            if (dto.ProjectId <= 0)
                return ServiceResult<ProjectResponseDto>.Failure("Invalid Project ID provided");

            if (string.IsNullOrWhiteSpace(dto.AddUserId))
                return ServiceResult<ProjectResponseDto>.Failure("User ID cannot be empty");

            try {
                var project = await _context.Projects
                .Include(p => p.WorkFlows)
                .Include(pu => pu.ProjectUsers)
                    .ThenInclude(u => u.User)
                .Include(pc => pc.Comment)
                .Include(pt => pt.PorjectTodos!)
                    .ThenInclude(pt => pt.SubTasks)
                    .OrderByDescending(p => p.DateCreated)
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.TenantId == _tenantContext.TenantId);
                if (project == null)
                    return ServiceResult<ProjectResponseDto>.Failure($"Project with ID: {dto.ProjectId} was not found");

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.AddUserId);
                if (user == null)
                    return ServiceResult<ProjectResponseDto>.Failure($"User with ID: {dto.AddUserId} was not found");

                // Get users in workflow - cannot add users outside of workflow
                var workFlow = await _context.WorkFlows
                    .Include(wf => wf.WorkFlowUsers)
                    .FirstOrDefaultAsync(wf => wf.Id == project.WorkFlowId && wf.TenantId == _tenantContext.TenantId);
                if (workFlow == null)
                    return ServiceResult<ProjectResponseDto>.Failure($"Workflow with ID {project.WorkFlowId} does not exist");

                // Ensure user is part of the project's workflow
                var workflowUserIds = workFlow.WorkFlowUsers
                    .Select(wfu => wfu.UserId)
                    .ToHashSet();

                if (!workflowUserIds.Contains(dto.AddUserId)) {
                    return ServiceResult<ProjectResponseDto>.Failure(
                        $"User '{user.UserName}' cannot be added because they are not part of the workflow '{workFlow.WorkFlowName}'.");
                }

                // Prevent duplicate users
                if (project.ProjectUsers.Any(pu => pu.UserId == dto.AddUserId)) {
                    return ServiceResult<ProjectResponseDto>.Failure(
                        $"User '{user.UserName}' is already part of this project."
                    );
                }

                // Add new user to project
                var projectUser = new ProjectUser {
                    ProjectId = dto.ProjectId,
                    UserId = dto.AddUserId
                };

                await _context.ProjectUsers.AddAsync(projectUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User '{UserName}' added to project '{ProjectTitle}' (ProjectId: {ProjectId})",
                    user.UserName, project.ProjectTitle, project.Id);

                return ServiceResult<ProjectResponseDto>.Success(
                    new ProjectResponseDto {
                        ProjectId = project.Id,
                        ProjectTitle = project.ProjectTitle,
                        ProjectDescription = project.ProjectDescription,
                        DueDate = project.DueDate,
                        DateCreated = project.DateCreated,
                        WorkFlowId = project.WorkFlows.Id,
                        WorkFlowName = project.WorkFlows.WorkFlowName,
                        Users = project.ProjectUsers.Select(u => new ProjectUsersInfoDto {
                            UserId = u.UserId,
                            UserName = u.User.UserName ?? string.Empty,
                        }).ToList(),
                        Comments = project.Comment?.Select(c => new ProjectCommentsInfoDto {
                            CommentId = c.Id,
                            CommentContent = c.CommentContent,
                            DateCreated = c.DateCreated,
                            UserId = c.UserId,
                            UserName = c.User.UserName ?? string.Empty,
                            IsViewed = c.IsViewed
                        }).ToList(),
                        Todos = project.PorjectTodos?.Select(t => new ProjectTodosInfoDto {
                            TodoId = t.Id,
                            TodoTitle = t.Title,
                            TodoIsComplete = t.IsComplete,
                            DueDate = t.DueDate,
                            UserName = t.User?.UserName ?? "Unknown",
                            ProjectTodoSubTasks = t.SubTasks?.Select(st => new ProjectTodoSubTasksInfoDto {
                                SubTaskId = st.Id,
                                SubTaskDescription = st.Description,
                                SubTaskIsCompleted = st.IsCompleted,
                            }).ToList() ?? new List<ProjectTodoSubTasksInfoDto>(),
                        }).ToList()
                    },
                    $"Added user '{user?.UserName ?? "Unknown"}' to project successfully"
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while adding new user to selected project");
                return ServiceResult<ProjectResponseDto>.Failure(
                    "A database error occurred while adding the user.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while adding users to selected project");
                return ServiceResult<ProjectResponseDto>.Failure(
                    "An unexpected error occurred while adding the user.",
                    new[] { ex.Message });
            }
        }


        // GET services
        // Get all projects
        public async Task<ServiceResult<List<ProjectResponseDto>>> GetAllProjects() {
            try {
                var projects = await _context.Projects
                    .Include(p => p.WorkFlows)
                    .Include(pu => pu.ProjectUsers)
                        .ThenInclude(u => u.User)
                    .Include(pc => pc.Comment)
                    .Include(pt => pt.PorjectTodos!)
                        .ThenInclude(pt => pt.SubTasks)
                    .OrderByDescending(p => p.DateCreated)
                    .Where(p => p.TenantId == _tenantContext.TenantId)
                    .ToListAsync();

                if (!projects.Any()) {
                    _logger.LogWarning($"No projects found for tenant {_tenantContext.TenantName}.");
                    return ServiceResult<List<ProjectResponseDto>>.Failure("No projects found");
                }

                return ServiceResult<List<ProjectResponseDto>>.Success(
                    projects.Select(p =>
                        new ProjectResponseDto {
                            ProjectId = p.Id,
                            ProjectTitle = p.ProjectTitle,
                            ProjectDescription = p.ProjectDescription,
                            DueDate = p.DueDate,
                            DateCreated = p.DateCreated,
                            WorkFlowId = p.WorkFlows.Id,
                            WorkFlowName = p.WorkFlows.WorkFlowName,
                            Users = p.ProjectUsers.Select(u => new ProjectUsersInfoDto {
                                UserId = u.UserId,
                                UserName = u.User.UserName ?? string.Empty,
                            }).ToList(),
                            Comments = p.Comment?.Select(c => new ProjectCommentsInfoDto {
                                CommentId = c.Id,
                                CommentContent = c.CommentContent,
                                DateCreated = c.DateCreated,
                                UserId = c.UserId,
                                UserName = c.User.UserName ?? string.Empty,
                                IsViewed = c.IsViewed
                            }).ToList(),
                            Todos = p.PorjectTodos?.Select(t => new ProjectTodosInfoDto {
                                TodoId = t.Id,
                                TodoTitle = t.Title,
                                TodoIsComplete = t.IsComplete,
                                DueDate = t.DueDate,
                                UserName = t.User?.UserName ?? "Unknown",
                                ProjectTodoSubTasks = t.SubTasks?.Select(st => new ProjectTodoSubTasksInfoDto {
                                    SubTaskId = st.Id,
                                    SubTaskDescription = st.Description,
                                    SubTaskIsCompleted = st.IsCompleted,
                                }).ToList() ?? new List<ProjectTodoSubTasksInfoDto>(),
                            }).ToList()
                        }).ToList(),
                    $"Retrieved {projects.Count} projects successfully"
                );
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving projects.");

                return ServiceResult<List<ProjectResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving projects.",
                    new[] { ex.Message });
            }
        }


        // Get project by id
        public async Task<ServiceResult<ProjectResponseDto>> GetProjectById(GetProjectDto dto) {
            if (dto == null)
                return ServiceResult<ProjectResponseDto>.Failure("No information provided");

            if (dto.ProjectId <= 0)
                return ServiceResult<ProjectResponseDto>.Failure("Invalid project ID provided");

            try {
                var project = await _context.Projects
                    .Include(p => p.WorkFlows)
                    .Include(pu => pu.ProjectUsers)
                        .ThenInclude(u => u.User)
                    .Include(pc => pc.Comment)
                    .Include(pt => pt.PorjectTodos!)
                        .ThenInclude(pt => pt.SubTasks)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.TenantId == _tenantContext.TenantId);

                if (project == null) {
                    _logger.LogWarning("Project with ID {ProjectId} not found.", dto.ProjectId);
                    return ServiceResult<ProjectResponseDto>.Failure("Project not found");
                }

                _logger.LogInformation("Successfully retrieved project ID {ProjectId}.", dto.ProjectId);

                return ServiceResult<ProjectResponseDto>.Success(
                    new ProjectResponseDto {
                        ProjectId = project.Id,
                        ProjectTitle = project.ProjectTitle,
                        ProjectDescription = project.ProjectDescription,
                        DueDate = project.DueDate,
                        DateCreated = project.DateCreated,
                        WorkFlowId = project.WorkFlows.Id,
                        WorkFlowName = project.WorkFlows.WorkFlowName,
                        Users = project.ProjectUsers.Select(u => new ProjectUsersInfoDto {
                            UserId = u.UserId,
                            UserName = u.User.UserName ?? string.Empty,
                        }).ToList(),
                        Comments = project.Comment?.Select(c => new ProjectCommentsInfoDto {
                            CommentId = c.Id,
                            CommentContent = c.CommentContent,
                            DateCreated = c.DateCreated,
                            UserId = c.UserId,
                            UserName = c.User.UserName ?? string.Empty,
                            IsViewed = c.IsViewed
                        }).ToList(),
                        Todos = project.PorjectTodos?.Select(t => new ProjectTodosInfoDto {
                            TodoId = t.Id,
                            TodoTitle = t.Title,
                            TodoIsComplete = t.IsComplete,
                            DueDate = t.DueDate,
                            UserName = t.User?.UserName ?? "Unknown",
                            ProjectTodoSubTasks = t.SubTasks?.Select(st => new ProjectTodoSubTasksInfoDto {
                                SubTaskId = st.Id,
                                SubTaskDescription = st.Description,
                                SubTaskIsCompleted = st.IsCompleted,
                            }).ToList() ?? new List<ProjectTodoSubTasksInfoDto>(),
                        }).ToList()
                    },
                    $"Retrieved project ID {dto.ProjectId} successfully"
                );
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving project ID {ProjectId}", dto.ProjectId);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "An unexpected error occurred while retrieving the project.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services
        // Update project info
        public async Task<ServiceResult<ProjectResponseDto>> UpdateProjectInfo(UpdateProjectInfoDto dto) {
            if (dto == null)
                return ServiceResult<ProjectResponseDto>.Failure("UpdateProjectInfoDto cannot be null");

            if (dto.ProjectId <= 0)
                return ServiceResult<ProjectResponseDto>.Failure("Invalid Project ID provided");

            try {
                var project = await _context.Projects
                    .Include(p => p.WorkFlows)
                    .Include(pu => pu.ProjectUsers)
                        .ThenInclude(u => u.User)
                    .Include(pc => pc.Comment)
                    .Include(pt => pt.PorjectTodos!)
                        .ThenInclude(pt => pt.SubTasks)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.TenantId == _tenantContext.TenantId);
                if (project == null)
                    return ServiceResult<ProjectResponseDto>.Failure($"Project with ID: {dto.ProjectId} was not found");

                // Update fields if provided
                if (!string.IsNullOrWhiteSpace(dto.Title))
                    project.ProjectTitle = dto.Title;

                if (dto.Description != null)
                    project.ProjectDescription = dto.Description;

                if (dto.DueDate.HasValue)
                    project.DueDate = dto.DueDate.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Project ID {ProjectId} updated successfully", dto.ProjectId);

                return ServiceResult<ProjectResponseDto>.Success(
                    new ProjectResponseDto {
                        ProjectId = project.Id,
                        ProjectTitle = project.ProjectTitle,
                        ProjectDescription = project.ProjectDescription,
                        DueDate = project.DueDate,
                        DateCreated = project.DateCreated,
                        WorkFlowId = project.WorkFlows.Id,
                        WorkFlowName = project.WorkFlows.WorkFlowName,
                        Users = project.ProjectUsers.Select(u => new ProjectUsersInfoDto {
                            UserId = u.UserId,
                            UserName = u.User.UserName ?? string.Empty,
                        }).ToList(),
                        Comments = project.Comment?.Select(c => new ProjectCommentsInfoDto {
                            CommentId = c.Id,
                            CommentContent = c.CommentContent,
                            DateCreated = c.DateCreated,
                            UserId = c.UserId,
                            UserName = c.User.UserName ?? string.Empty,
                            IsViewed = c.IsViewed
                        }).ToList(),
                        Todos = project.PorjectTodos?.Select(t => new ProjectTodosInfoDto {
                            TodoId = t.Id,
                            TodoTitle = t.Title,
                            TodoIsComplete = t.IsComplete,
                            DueDate = t.DueDate,
                            UserName = t.User?.UserName ?? "Unknown",
                            ProjectTodoSubTasks = t.SubTasks?.Select(st => new ProjectTodoSubTasksInfoDto {
                                SubTaskId = st.Id,
                                SubTaskDescription = st.Description,
                                SubTaskIsCompleted = st.IsCompleted,
                            }).ToList() ?? new List<ProjectTodoSubTasksInfoDto>(),
                        }).ToList()
                    },
                    $"Updated project successfully"
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while updating project ID {ProjectId}", dto.ProjectId);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "A database error occurred while updating the project.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while updating project ID {ProjectId}", dto.ProjectId);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "An unexpected error occurred while updating the project.",
                    new[] { ex.Message });
            }
        }


        // DELETE services
        // Delete project
        public async Task<ServiceResult<ProjectResponseDto>> DeleteProject(GetProjectDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ProjectResponseDto>.Failure("No information provided");

            if (dto.ProjectId <= 0)
                return ServiceResult<ProjectResponseDto>.Failure("Invalid Project ID provided");

            try {
                var project = await _context.Projects
                    .Include(p => p.WorkFlows)
                    .Include(pu => pu.ProjectUsers)
                        .ThenInclude(u => u.User)
                    .Include(pc => pc.Comment)
                    .Include(pt => pt.PorjectTodos!)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.TenantId == _tenantContext.TenantId);
                if (project == null)
                    return ServiceResult<ProjectResponseDto>.Failure($"Project with ID: '{dto.ProjectId}' was not found");

                var response = new ProjectResponseDto {
                    ProjectId = project.Id,
                    ProjectTitle = project.ProjectTitle,
                    ProjectDescription = project.ProjectDescription,
                    DueDate = project.DueDate,
                    DateCreated = project.DateCreated,
                    WorkFlowId = project.WorkFlows.Id,
                    WorkFlowName = project.WorkFlows.WorkFlowName,
                    Users = project.ProjectUsers.Select(u => new ProjectUsersInfoDto {
                        UserId = u.UserId,
                        UserName = u.User.UserName ?? string.Empty,
                    }).ToList(),
                    Comments = project.Comment?.Select(c => new ProjectCommentsInfoDto {
                        CommentId = c.Id,
                        CommentContent = c.CommentContent,
                        DateCreated = c.DateCreated,
                        UserId = c.UserId,
                        UserName = c.User.UserName ?? string.Empty,
                        IsViewed = c.IsViewed
                    }).ToList(),
                    Todos = project.PorjectTodos?.Select(t => new ProjectTodosInfoDto {
                        TodoId = t.Id,
                        TodoTitle = t.Title,
                        TodoIsComplete = t.IsComplete,
                        DueDate = t.DueDate,
                        UserName = t.User?.UserName ?? "Unknown",
                        ProjectTodoSubTasks = t.SubTasks?.Select(st => new ProjectTodoSubTasksInfoDto {
                            SubTaskId = st.Id,
                            SubTaskDescription = st.Description,
                            SubTaskIsCompleted = st.IsCompleted,
                        }).ToList() ?? new List<ProjectTodoSubTasksInfoDto>(),
                    }).ToList()
                };

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Project ID '{ProjectId}' deleted successfully", dto.ProjectId);

                return ServiceResult<ProjectResponseDto>.Success(response, $"Deleted project '{project.ProjectTitle}' successfully");
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while deleting project ID '{ProjectId}'", dto.ProjectId);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "A database error occurred while deleting the project.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while deleting project ID '{ProjectId}'", dto.ProjectId);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "An unexpected error occurred while deleting the project.",
                    new[] { ex.Message });
            }
        }


        // Remove user from project
        public async Task<ServiceResult<ProjectResponseDto>> RemoveUserFromProject(GetProjectDto dto) {
            if (dto == null)
                return ServiceResult<ProjectResponseDto>.Failure("RemoveUserFromProjectDto cannot be null");

            if (dto.ProjectId <= 0)
                return ServiceResult<ProjectResponseDto>.Failure("Invalid Project ID provided");

            // UserId = user to be deleted
            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<ProjectResponseDto>.Failure("User ID cannot be empty");

            try {
                // Entity used to remove the user
                var projectUser = await _context.ProjectUsers
                    .FirstOrDefaultAsync(pu => pu.ProjectId == dto.ProjectId && pu.UserId == dto.UserId);
                if (projectUser == null)
                    return ServiceResult<ProjectResponseDto>.Failure($"User with ID: {dto.UserId} is not part of project ID: {dto.ProjectId}");

                var project = await _context.Projects
                    .Include(p => p.WorkFlows)
                    .Include(pu => pu.ProjectUsers)
                        .ThenInclude(u => u.User)
                    .Include(pc => pc.Comment)
                    .Include(pt => pt.PorjectTodos!)
                        .ThenInclude(pt => pt.SubTasks)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.TenantId == _tenantContext.TenantId);
                if (project == null)
                    return ServiceResult<ProjectResponseDto>.Failure($"Project with ID: {dto.ProjectId} was not found");
                // Check if user is last user in project
                var projectUsersCount = await _context.ProjectUsers
                    .CountAsync(pu => pu.ProjectId == dto.ProjectId);
                if (projectUsersCount <= 1)
                    return ServiceResult<ProjectResponseDto>.Failure("Cannot remove the last user from the project");

                _context.ProjectUsers.Remove(projectUser);
                await _context.SaveChangesAsync();

                var response = new ProjectResponseDto {
                    ProjectId = project.Id,
                    ProjectTitle = project.ProjectTitle,
                    ProjectDescription = project.ProjectDescription,
                    DueDate = project.DueDate,
                    DateCreated = project.DateCreated,
                    WorkFlowId = project.WorkFlows.Id,
                    WorkFlowName = project.WorkFlows.WorkFlowName,
                    Users = project.ProjectUsers.Select(u => new ProjectUsersInfoDto {
                        UserId = u.UserId,
                        UserName = u.User.UserName ?? string.Empty,
                    }).ToList(),
                    Comments = project.Comment?.Select(c => new ProjectCommentsInfoDto {
                        CommentId = c.Id,
                        CommentContent = c.CommentContent,
                        DateCreated = c.DateCreated,
                        UserId = c.UserId,
                        UserName = c.User.UserName ?? string.Empty,
                        IsViewed = c.IsViewed
                    }).ToList(),
                    Todos = project.PorjectTodos?.Select(t => new ProjectTodosInfoDto {
                        TodoId = t.Id,
                        TodoTitle = t.Title,
                        TodoIsComplete = t.IsComplete,
                        DueDate = t.DueDate,
                        UserName = t.User?.UserName ?? "Unknown",
                        ProjectTodoSubTasks = t.SubTasks?.Select(st => new ProjectTodoSubTasksInfoDto {
                            SubTaskId = st.Id,
                            SubTaskDescription = st.Description,
                            SubTaskIsCompleted = st.IsCompleted,
                        }).ToList() ?? new List<ProjectTodoSubTasksInfoDto>(),
                    }).ToList()
                };

                _logger.LogInformation("User ID '{UserId}' removed from project ID '{ProjectId}' successfully", dto.UserId, dto.ProjectId);

                return ServiceResult<ProjectResponseDto>.Success(response, $"Removed user '{projectUser.User.UserName ?? "Unknown"}' from project successfully");
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while deleting user ID '{userId}' from project ID '{projectId}'", dto.UserId, dto.ProjectId);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "A database error occurred while deleting the the user from the project.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while deleting user ID '{userId}' from project ID '{projectId}'", dto.UserId, dto.ProjectId);
                return ServiceResult<ProjectResponseDto>.Failure(
                    "An unexpected error occurred while deleting the the user from the project.",
                    new[] { ex.Message });
            }
        }
    }
}
