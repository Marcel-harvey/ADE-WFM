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
        public async Task<ServiceResult<CreateWorkFlowResponseDto>> AddWorkFlow(CreateWorkFlowDto dto)
        {
            try
            {
                // Basic input validation
                if (string.IsNullOrWhiteSpace(dto.WorkFlowName))
                    return ServiceResult<CreateWorkFlowResponseDto>.Failure("Work flow name is required.");

                if (string.IsNullOrEmpty(dto.CurrentUserId))
                    return ServiceResult<CreateWorkFlowResponseDto>.Failure("Current user ID is required.");

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

                // Save workflow to database
                _context.WorkFlows.Add(workFlow);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow '{WorkFlowName}' created successfully by user ID {UserId}",
                    dto.WorkFlowName, dto.CurrentUserId);

                // Return success
                return ServiceResult<CreateWorkFlowResponseDto>.Success(
                    new CreateWorkFlowResponseDto
                    {
                        Id = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                        CreatedByUserId = dto.CurrentUserId,
                        AssignedUserIds = workFlow.WorkFlowUsers.Select(u => u.UserId).ToList(),
                        CreatedAt = DateTime.UtcNow,
                    },
                    "Workflow created successfully."
                );
            }
            // Database exceptions
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating workflow '{WorkFlowName}'", dto.WorkFlowName);
                return ServiceResult<CreateWorkFlowResponseDto>.Failure(
                    "A database error occurred while creating the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating workflow '{WorkFlowName}'", dto.WorkFlowName);
                return ServiceResult<CreateWorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while creating the workflow.",
                    new[] { ex.Message });
            }
        }




        // Add users to existing workflow
        public async Task<ServiceResult<AddUserWorkFlowResponseDto>> AddUserToWorkFlow(AddUserWorkFlowDto model)
        {
            try
            {
                // Validate input
                if (model.UserIds == null || !model.UserIds.Any())
                    return ServiceResult<AddUserWorkFlowResponseDto>.Failure("No user IDs were provided.");

                // Check if the workflow exists
                var workFlow = await _context.WorkFlows
                    .Include(wf => wf.WorkFlowUsers)
                    .FirstOrDefaultAsync(wf => wf.Id == model.WorkFlowId);

                if (workFlow == null)
                    return ServiceResult<AddUserWorkFlowResponseDto>.Failure($"Workflow with ID {model.WorkFlowId} not found.");

                // Get existing user IDs to avoid duplicates
                var existingUserIds = workFlow.WorkFlowUsers
                    .Select(wfUser => wfUser.UserId)
                    .ToList();

                var addedUsers = new List<WorkFlowUserDto>();
                // For errors in users added
                var errors = new List<string>();

                // Loop through new users
                foreach (var userId in model.UserIds)
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
                        WorkFlowId = model.WorkFlowId,
                        UserId = userId,
                        Role = "Standard"
                    };

                    _context.WorkFlowUsers.Add(wfUser);

                    addedUsers.Add(new WorkFlowUserDto
                    {
                        Name = user.UserName ?? "Unknown",
                        Role = wfUser.Role
                    });
                }

                await _context.SaveChangesAsync();

                // Log the operation
                _logger.LogInformation(
                    "Added {Count} users to workflow '{WorkFlowName}' (ID: {WorkFlowId})",
                    addedUsers.Count,
                    workFlow.WorkFlowName,
                    workFlow.Id
                );

                // Return response
                return ServiceResult<AddUserWorkFlowResponseDto>.Success(
                    new AddUserWorkFlowResponseDto
                    {
                        WorkFlowId = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                        Users = addedUsers,
                    },
                    errors.Any()
                        ? "Completed with warnings."
                        : "Users added successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding users to workflow ID {WorkFlowId}", model.WorkFlowId);
                return ServiceResult<AddUserWorkFlowResponseDto>.Failure(
                    "A database error occurred while adding users to the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding users to workflow ID {WorkFlowId}", model.WorkFlowId);
                return ServiceResult<AddUserWorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while adding users to the workflow.",
                    new[] { ex.Message });
            }
        }


        // GET
        // list of all workflows  
        public async Task<ServiceResult<List<GetAllWorkFlowsDtoResponse>>> GetAllWorkFlows()
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
                    return ServiceResult<List<GetAllWorkFlowsDtoResponse>>.Failure("No workflows found.");
                }

                var response = workFlows.Select(wf => new GetAllWorkFlowsDtoResponse
                {
                    Id = wf.Id,
                    Name = wf.WorkFlowName,
                    Projects = wf.Project?.Select(p => new GetWorkFlowProjectsDto
                    {
                        Id = p.Id,
                        ProjectName = p.ProjectTitle
                    }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                    Users = wf.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                    {
                        Id = wu.UserId,
                        UserName = wu.User.UserName ?? ""
                    }).ToList() ?? new List<GetWorkFlowUsersDto>()
                }).ToList();

                _logger.LogInformation("Retrieved {Count} workflows successfully.", response.Count);

                return ServiceResult<List<GetAllWorkFlowsDtoResponse>>.Success(response, "Workflows retrieved successfully.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflows.");

                return ServiceResult<List<GetAllWorkFlowsDtoResponse>>.Failure(
                    "An unexpected error occurred while retrieving workflows.",
                    new[] { ex.Message });
            }
        }


        // Get workflow by ID
        public async Task<ServiceResult<GetAllWorkFlowsDtoResponse>> GetWorkFlowById(GetWorkFlowByIdDto dto)
        {
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
                    return ServiceResult<GetAllWorkFlowsDtoResponse>.Failure($"Workflow with ID {dto.Id} was not found.");
                }
                var response = new GetAllWorkFlowsDtoResponse
                {
                    Id = workFlow.Id,
                    Name = workFlow.WorkFlowName,
                    Projects = workFlow.Project?.Select(p => new GetWorkFlowProjectsDto
                    {
                        Id = p.Id,
                        ProjectName = p.ProjectTitle
                    }).ToList() ?? new List<GetWorkFlowProjectsDto>(),
                    Users = workFlow.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                    {
                        Id = wu.UserId,
                        UserName = wu.User.UserName ?? ""
                    }).ToList() ?? new List<GetWorkFlowUsersDto>()
                };

                _logger.LogInformation("Retrieved workflow '{WorkFlowName}' (ID: {WorkFlowId}) successfully.",
                    workFlow.WorkFlowName, workFlow.Id);

                return ServiceResult<GetAllWorkFlowsDtoResponse>.Success(response, "Workflow retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow with ID {WorkFlowId}.", dto.Id);

                return ServiceResult<GetAllWorkFlowsDtoResponse>.Failure(
                    "An unexpected error occurred while retrieving the workflow.",
                    new[] { ex.Message });
            }
        }


        //UPDATE:
        // Update workflow's name
        public async Task <ServiceResult<UpdateWorkFlowNameResponseDto>> UpdateWorkFlowName(UpdateWorkFlowNameDto dto)
        {
            try
            {
                var workFlow = await _context.WorkFlows
                    .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId);

                if (workFlow == null)
                {
                    _logger.LogWarning("Workflow with ID {WorkFlowId} not found for update.", dto.WorkFlowId);
                    return ServiceResult<UpdateWorkFlowNameResponseDto>.Failure($"Workflow with ID {dto.WorkFlowId} was not found.");
                }

                var oldName = workFlow.WorkFlowName;

                if (string.IsNullOrWhiteSpace(dto.WorkFlowName))
                    return ServiceResult<UpdateWorkFlowNameResponseDto>.Failure("New workflow name cannot be empty.");

                workFlow.WorkFlowName = dto.WorkFlowName.Trim();

                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow name updated from '{OldName}' to '{NewName}' for ID {WorkFlowId}",
                    oldName, dto.WorkFlowName, dto.WorkFlowId);

                return ServiceResult<UpdateWorkFlowNameResponseDto>.Success(
                    new UpdateWorkFlowNameResponseDto
                    {
                        OldName = oldName,
                        NewName = dto.WorkFlowName,
                    },
                    $"Workflow name updated successfully to {dto.WorkFlowName}."
                );
                
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating workflow name for ID {WorkFlowId}", dto.WorkFlowId);
                return ServiceResult<UpdateWorkFlowNameResponseDto>.Failure(
                    "A database error occurred while updating the workflow name.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating workflow name for ID {WorkFlowId}", dto.WorkFlowId);
                return ServiceResult<UpdateWorkFlowNameResponseDto>.Failure(
                    "An unexpected error occurred while updating the workflow name.",
                    new[] { ex.Message });
            }
        }


        // DELETE services
        // Delete workflow
        public async Task<ServiceResult<DeleteWorkFlowResponseDto>> DeleteWorkFlow(DeleteWorkFlowDto dto)
        {
            try
            {
                if (dto.Id <= 0)
                    return ServiceResult<DeleteWorkFlowResponseDto>.Failure("Invalid workflow ID.");

                var workFlow = await _context.WorkFlows
                    .Include(w => w.Comments)
                    .FirstOrDefaultAsync(w => w.Id == dto.Id);

                if (workFlow == null)
                {
                    _logger.LogWarning("Workflow with ID {WorkFlowId} not found for deletion.", dto.Id);
                    return ServiceResult<DeleteWorkFlowResponseDto>.Failure($"Workflow with ID {dto.Id} was not found.");
                }

                _context.WorkFlows.Remove(workFlow);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow '{WorkFlowName}' (ID: {WorkFlowId}) deleted successfully.",
                    workFlow.WorkFlowName, workFlow.Id);

                return ServiceResult<DeleteWorkFlowResponseDto>.Success(
                    new DeleteWorkFlowResponseDto
                    {
                        Name = workFlow.WorkFlowName,
                    },
                    "Workflow deleted successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating workflow name for ID {WorkFlowId}", dto.Id);
                return ServiceResult<DeleteWorkFlowResponseDto>.Failure(
                    "A database error occurred while updating the workflow name.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow with ID {WorkFlowId}", dto.Id);
                return ServiceResult<DeleteWorkFlowResponseDto>.Failure(
                    "An unexpected error occurred while deleting the workflow.",
                    new[] { ex.Message });
            }
        }


        // Remove user from workflow
        public async Task<ResponseRemoveUserFromWorkFlowDto> RemoveUserFromWorkFlow(RemoveUserFromWorkFlowDto dto)
        {
            var workFlowUser = await _context.WorkFlowUsers
                .FirstOrDefaultAsync(wfu => wfu.UserId == dto.UserId && wfu.WorkFlowId == dto.WorkFlowId)
                ?? throw new KeyNotFoundException($"User with ID {dto.UserId} not found in any workflow.");

            var userName = await _userManager
                .FindByIdAsync(dto.UserId);

            _context.WorkFlowUsers.Remove(workFlowUser);
            await _context.SaveChangesAsync();

            return new ResponseRemoveUserFromWorkFlowDto
            {
                Name = userName?.UserName ?? "Unknown",
                Message = $"User '{userName?.UserName ?? "Unknown"}' removed from workflow successfully."
            };
        }





    }
}
