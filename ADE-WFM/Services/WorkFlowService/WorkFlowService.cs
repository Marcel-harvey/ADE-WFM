using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace ADE_WFM.Services.WorkFlowService
{
    public class WorkFlowService : IWorkFlowService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<WorkFlowService> _logger;

        public WorkFlowService(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            ILogger<WorkFlowService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        // CREATE:
        // Add new workflow with user the created and extra list of users if selected
        public async Task<ServiceResult<WorkFlowResponseDto>> AddWorkFlow(CreateWorkFlowDto dto)
        {
            // General validation
            if (string.IsNullOrWhiteSpace(dto.WorkFlowName))
                return ServiceResult<WorkFlowResponseDto>.Failure("Work flow name is required.");

            if (string.IsNullOrEmpty(dto.CurrentUserId))
                return ServiceResult<WorkFlowResponseDto>.Failure("Current user ID is required.");

            try
            {
                var workFlow = new WorkFlow
                {
                    WorkFlowName = dto.WorkFlowName,
                    WorkFlowUsers = new List<WorkFlowUser>()
                };

                // Add creator as admin
                workFlow.WorkFlowUsers.Add(new WorkFlowUser
                {
                    UserId = dto.CurrentUserId,
                    Role = "Admin"
                });

                // Add other assigned users
                if (dto.UserIds != null && dto.UserIds.Any())
                {
                    foreach (var userId in dto.UserIds)
                    {
                        if (userId != dto.CurrentUserId)
                        {
                            workFlow.WorkFlowUsers.Add(new WorkFlowUser
                            {
                                UserId = userId,
                                Role = "Standard"
                            });
                        }
                    }
                }

                _context.WorkFlows.Add(workFlow);
                await _context.SaveChangesAsync();

                // Reload for username en response
                var createdWorkflow = await _context.WorkFlows
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .Include(wf => wf.Project)
                    .FirstOrDefaultAsync(wf => wf.Id == workFlow.Id);


                _logger.LogInformation("Workflow '{WorkFlowName}' created successfully by user ID {UserId}",
                    dto.WorkFlowName, dto.CurrentUserId);

                // Return success
                return ServiceResult<WorkFlowResponseDto>.Success(
                    new WorkFlowResponseDto
                    {
                        WorkFlowId = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                        Projects = workFlow.Project?.Select(p => new GetWorkFlowProjectsDto
                        {
                            Id = p.Id,
                            ProjectName = p.ProjectTitle
                        }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                        Users = createdWorkflow?.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                        {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? "Unknown"
                        }).ToList() ?? new List<GetWorkFlowUsersDto>()
                    },
                    "Workflow created successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating workflow '{WorkFlowName}'", dto.WorkFlowName);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "A database error occurred while creating the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating workflow '{WorkFlowName}'", dto.WorkFlowName);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while creating the workflow.",
                    new[] { ex.Message });
            }
        }


        // Add users to existing workflow
        public async Task<ServiceResult<WorkFlowResponseDto>> AddUserToWorkFlow(AddUserWorkFlowDto dto)
        {
            if (dto == null)
                return ServiceResult<WorkFlowResponseDto>.Failure("Input data is required.");

            // List of user IDs to add - check for if valid
            if (dto.UserIds == null || !dto.UserIds.Any())
                return ServiceResult<WorkFlowResponseDto>.Failure("No user IDs were provided.");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<WorkFlowResponseDto>.Failure("Invalid workflow ID provided.");

            try
            {
                // Check if the workflow exists
                var workFlow = await _context.WorkFlows
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId);
                if (workFlow == null)
                    return ServiceResult<WorkFlowResponseDto>.Failure($"Workflow with ID {dto.WorkFlowId} not found.");

                // Get existing user IDs to avoid duplicates
                var existingUserIds = workFlow.WorkFlowUsers
                    .Select(wfUser => wfUser.UserId)
                    .ToList();

                // For errors in users added
                var errors = new List<string>();

                // Loop through new users
                foreach (var userId in dto.UserIds)
                {
                    if (existingUserIds.Contains(userId))
                    {
                        errors.Add($"User {userId} is already part of this workflow.");
                        continue;
                    }

                    // Verify the user exists in Identity
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        errors.Add($"User with ID {userId} not found. Skipped.");
                        continue;
                    }

                    var wfUser = new WorkFlowUser
                    {
                        WorkFlowId = dto.WorkFlowId,
                        UserId = userId,
                        Role = "Standard"
                    };

                    _context.WorkFlowUsers.Add(wfUser);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Added new users to workflow '{WorkFlowName}' (ID: {WorkFlowId})", workFlow.WorkFlowName, workFlow.Id);

                return ServiceResult<WorkFlowResponseDto>.Success(
                    new WorkFlowResponseDto
                    {
                        WorkFlowId = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                        Projects = workFlow.Project?.Select(p => new GetWorkFlowProjectsDto
                        {
                            Id = p.Id,
                            ProjectName = p.ProjectTitle
                        }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                        Users = workFlow.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                        {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? ""
                        }).ToList() ?? new List<GetWorkFlowUsersDto>()
                    },
                    errors.Any()
                        ? "Completed with warnings."
                        : "Users added successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding users to workflow ID {WorkFlowId}", dto.WorkFlowId);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "A database error occurred while adding users to the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding users to workflow ID {WorkFlowId}", dto.WorkFlowId);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while adding users to the workflow.",
                    new[] { ex.Message });
            }
        }


        // GET
        // list of all workflows  
        public async Task<ServiceResult<List<WorkFlowResponseDto>>> GetAllWorkFlows()
        {
            try
            {
                var workFlows = await _context.WorkFlows
                    .Include(wf => wf.Project)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .ToListAsync();
                if (workFlows == null || !workFlows.Any())
                {
                    _logger.LogWarning("No workflows found in the system.");
                    return ServiceResult<List<WorkFlowResponseDto>>.Failure("No workflows found.");
                }

                _logger.LogInformation("Retrieved all workflows successfully.");

                return ServiceResult<List<WorkFlowResponseDto>>.Success(
                    workFlows.Select(wf => new WorkFlowResponseDto
                    {
                        WorkFlowId = wf.Id,
                        WorkFlowName = wf.WorkFlowName,
                        Projects = wf.Project?.Select(p => new GetWorkFlowProjectsDto
                        {
                            Id = p.Id,
                            ProjectName = p.ProjectTitle
                        }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                        Users = wf.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                        {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? ""
                        }).ToList() ?? new List<GetWorkFlowUsersDto>()
                    }).ToList(),
                    "Workflows retrieved successfully."
                );
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflows.");

                return ServiceResult<List<WorkFlowResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving workflows.",
                    new[] { ex.Message });
            }
        }


        // Get workflow by ID
        public async Task<ServiceResult<WorkFlowResponseDto>> GetWorkFlowById(GetWorkFlowByIdDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<WorkFlowResponseDto>.Failure("Input data is required.");

            if (dto.Id <= 0)
                return ServiceResult<WorkFlowResponseDto>.Failure("Invalid workflow ID provided.");

            try
            {
                var workFlow = await _context.WorkFlows
                    .Include(wf => wf.Project)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.Id);

                if (workFlow == null)
                {
                    _logger.LogWarning("Workflow with ID {WorkFlowId} not found.", dto.Id);
                    return ServiceResult<WorkFlowResponseDto>.Failure($"Workflow with ID {dto.Id} was not found.");
                }

                _logger.LogInformation("Retrieved workflow '{WorkFlowName}' (ID: {WorkFlowId}) successfully.",
                    workFlow.WorkFlowName, workFlow.Id);

                return ServiceResult<WorkFlowResponseDto>.Success(
                    new WorkFlowResponseDto
                    {
                        WorkFlowId = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                        Projects = workFlow.Project?.Select(p => new GetWorkFlowProjectsDto
                        {
                            Id = p.Id,
                            ProjectName = p.ProjectTitle
                        }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                        Users = workFlow.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                        {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? ""
                        }).ToList() ?? new List<GetWorkFlowUsersDto>()
                    },
                    "Workflow retrieved successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow with ID {WorkFlowId}.", dto.Id);

                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while retrieving the workflow.",
                    new[] { ex.Message });
            }
        }


        //UPDATE:
        // Update workflow's name
        public async Task <ServiceResult<WorkFlowResponseDto>> UpdateWorkFlowName(UpdateWorkFlowNameDto dto)
        {
            if (dto == null)
                return ServiceResult<WorkFlowResponseDto>.Failure("Input data is required.");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<WorkFlowResponseDto>.Failure("Invalid workflow ID provided.");

            if (string.IsNullOrWhiteSpace(dto.WorkFlowName))
                return ServiceResult<WorkFlowResponseDto>.Failure("New workflow name cannot be empty.");

            try
            {
                var workFlow = await _context.WorkFlows
                    .Include(wf => wf.Project)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId);
                if (workFlow == null)
                {
                    _logger.LogWarning("Workflow with ID {WorkFlowId} not found for update.", dto.WorkFlowId);
                    return ServiceResult<WorkFlowResponseDto>.Failure($"Workflow with ID {dto.WorkFlowId} was not found.");
                }

                workFlow.WorkFlowName = dto.WorkFlowName.Trim();

                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow name updated to '{NewName}' for ID {WorkFlowId}", dto.WorkFlowName, dto.WorkFlowId);

                return ServiceResult<WorkFlowResponseDto>.Success(
                    new WorkFlowResponseDto
                    {
                        WorkFlowId = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                        Projects = workFlow.Project?.Select(p => new GetWorkFlowProjectsDto
                        {
                            Id = p.Id,
                            ProjectName = p.ProjectTitle
                        }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                        Users = workFlow.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                        {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? ""
                        }).ToList() ?? new List<GetWorkFlowUsersDto>()
                    },
                    $"Workflow name updated successfully to {dto.WorkFlowName}."
                );
                
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating workflow name for ID {WorkFlowId}", dto.WorkFlowId);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "A database error occurred while updating the workflow name.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating workflow name for ID {WorkFlowId}", dto.WorkFlowId);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while updating the workflow name.",
                    new[] { ex.Message });
            }
        }


        // DELETE services
        // Delete workflow
        public async Task<ServiceResult<WorkFlowResponseDto>> DeleteWorkFlow(DeleteWorkFlowDto dto)
        {
            // Genral validation
            if (dto == null)
                return ServiceResult<WorkFlowResponseDto>.Failure("Input data is required.");

            if (dto.Id <= 0)
                return ServiceResult<WorkFlowResponseDto>.Failure("Invalid workflow ID.");

            try
            {

                var workFlow = await _context.WorkFlows
                    .Include(w => w.Comments)
                    .Include(w => w.Project)
                    .Include(w => w.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync(w => w.Id == dto.Id);

                if (workFlow == null)
                {
                    _logger.LogWarning("Workflow with ID {WorkFlowId} not found for deletion.", dto.Id);
                    return ServiceResult<WorkFlowResponseDto>.Failure($"Workflow with ID {dto.Id} was not found.");
                }

                var response = new WorkFlowResponseDto
                {
                    WorkFlowId = workFlow.Id,
                    WorkFlowName = workFlow.WorkFlowName,
                    Projects = workFlow.Project?.Select(p => new GetWorkFlowProjectsDto
                    {
                        Id = p.Id,
                        ProjectName = p.ProjectTitle
                    }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                    Users = workFlow.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                    {
                        Id = wu.UserId,
                        UserName = wu.User?.UserName ?? ""
                    }).ToList() ?? new List<GetWorkFlowUsersDto>()
                };

                _context.WorkFlows.Remove(workFlow);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow '{WorkFlowName}' (ID: {WorkFlowId}) deleted successfully.", workFlow.WorkFlowName, workFlow.Id);

                return ServiceResult<WorkFlowResponseDto>.Success(response, "Workflow deleted successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating workflow name for ID {WorkFlowId}", dto.Id);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "A database error occurred while deleting the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow with ID {WorkFlowId}", dto.Id);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while deleting the workflow.",
                    new[] { ex.Message });
            }
        }


        // Remove user from workflow
        public async Task<ServiceResult<WorkFlowResponseDto>> RemoveUserFromWorkFlow(RemoveUserFromWorkFlowDto dto)
        {
            if (dto == null)
                return ServiceResult<WorkFlowResponseDto>.Failure("Input data is required.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<WorkFlowResponseDto>.Failure("User ID is required.");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<WorkFlowResponseDto>.Failure("Invalid workflow ID provided.");

            try
            {
                var workFlowUser = await _context.WorkFlowUsers
                    .Include(wfu => wfu.User)
                    .FirstOrDefaultAsync(wfu => wfu.UserId == dto.UserId && wfu.WorkFlowId == dto.WorkFlowId);
                if (workFlowUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found in workflow ID {WorkFlowId}.", dto.UserId, dto.WorkFlowId);
                    return ServiceResult<WorkFlowResponseDto>.Failure($"User with ID {dto.UserId} not found in the specified workflow.");
                }

                var user = await _userManager
                    .FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found in Identity.", dto.UserId);
                    return ServiceResult<WorkFlowResponseDto>.Failure($"User with ID {dto.UserId} not found.");
                }

                var workFlow = await _context.WorkFlows
                    .Include(wf => wf.Project)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId);
                if (workFlow == null)
                    return ServiceResult<WorkFlowResponseDto>.Failure($"Workflow with ID {dto.WorkFlowId} not found.");

                _context.WorkFlowUsers.Remove(workFlowUser);
                await _context.SaveChangesAsync();

                return ServiceResult<WorkFlowResponseDto>.Success(
                    new WorkFlowResponseDto
                    {
                        WorkFlowId = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                        Projects = workFlow.Project?.Select(p => new GetWorkFlowProjectsDto
                        {
                            Id = p.Id,
                            ProjectName = p.ProjectTitle
                        }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                        Users = workFlow.WorkFlowUsers?.Where(wu => wu.UserId != dto.UserId).Select(wu => new GetWorkFlowUsersDto
                        {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? ""
                        }).ToList() ?? new List<GetWorkFlowUsersDto>()
                    },
                    "User removed from workflow successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while removing user from the workflow with ID user ID = {userId}", dto.UserId);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "A database error occurred while removing the user from the workflow .",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from the workflow with ID user ID = {userId}", dto.UserId);
                return ServiceResult<WorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while removing the user from the workflow.",
                    new[] { ex.Message });
            }
        }
    }
}
