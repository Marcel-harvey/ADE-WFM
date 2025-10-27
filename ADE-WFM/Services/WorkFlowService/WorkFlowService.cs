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
        public async Task<List<ResponseGetWorkFlowsDto>> GetAllWorkFlows()
        {
            var workflows = await _context.WorkFlows
                .Include(wf => wf.Project)
                .Include(wf => wf.WorkFlowUsers)
                .ThenInclude(wu => wu.User)
                .ToListAsync();

            return workflows.Select(wf => new ResponseGetWorkFlowsDto
            {
                Id = wf.Id,
                Name = wf.WorkFlowName,
                Projects = wf.Project?.Select(p => new GetWorkFlowProjectsDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectTitle
                }).ToList(),
                Users = wf.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                {
                    Id = wu.UserId,
                    UserName = wu.User.UserName ?? ""
                }).ToList()
            }).ToList();
        }


        // Get workflow by ID
        public async Task<ResponseGetWorkFlowsDto> GetWorkFlowById(GetWorkFlowByIdDto dto)
        {
            var workFlow = await _context.WorkFlows
                .Include(wf => wf.Project)
                .Include(wf => wf.WorkFlowUsers)
                    .ThenInclude(wu => wu.User)
                .FirstOrDefaultAsync(wf => wf.Id == dto.Id)
                ?? throw new KeyNotFoundException($"Work flow with ID: {dto.Id} was not found");

            return new ResponseGetWorkFlowsDto
            {
                Id = workFlow.Id,
                Name = workFlow.WorkFlowName,
                Projects = workFlow.Project?.Select(p => new GetWorkFlowProjectsDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectTitle
                }).ToList(),
                Users = workFlow.WorkFlowUsers?.Select(wu => new GetWorkFlowUsersDto
                {
                    Id = wu.UserId,
                    UserName = wu.User.UserName ?? ""
                }).ToList()
            }; 
        }


        //UPDATE:
        // Update workflow's name
        public async Task <ResponseUpdateWorkFlowNameDto> UpdateWorkFlowName(UpdateWorkFlowNameDto dto)
        {
            var workFlow = await _context.WorkFlows
                .FirstOrDefaultAsync(wfId => wfId.Id == dto.WorkFlowId)
                ?? throw new KeyNotFoundException($"Workflow with ID {dto.WorkFlowId} was not found.");

            var oldName = workFlow.WorkFlowName;

            workFlow.WorkFlowName = dto.WorkFlowName;

            await _context.SaveChangesAsync();

            return new ResponseUpdateWorkFlowNameDto
            {
                OldName = oldName,
                NewName = dto.WorkFlowName,
                Message = $"Workflow name updated to '{dto.WorkFlowName}'."
            };
        }        


        // DELETE services
        // Delete workflow
        public async Task <ResponseDeleteWorkFlowDto> DeleteWorkFlow(DeleteWorkFlowDto dto)
        {
            var workFlow = await _context.WorkFlows
                .Include(w => w.Comments)
                .FirstOrDefaultAsync(w => w.Id == dto.Id)
                ?? throw new KeyNotFoundException($"Workflow with ID {dto.Id} was not found.");

            var workFlowName = workFlow.WorkFlowName;

            _context.WorkFlows.Remove(workFlow);
            await _context.SaveChangesAsync();

            return new ResponseDeleteWorkFlowDto
            {
                Name = workFlowName,
                Message = $"Work flow '{workFlowName}' deleted successfully."
            };
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
