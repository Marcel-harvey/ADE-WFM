using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProgramDtos;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;
using ADE_WFM.Services.TenantService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.WorkFlowService {
    public class ProgramService : IProgramService {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProgramService> _logger;
        private readonly TenantContext _tenantContext;

        public ProgramService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ProgramService> logger,
            TenantContext tenantContext) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _tenantContext = tenantContext;
        }


        // CREATE:
        // Add new workflow with user the created and extra list of users if selected
        // TODO: Check that username is list is not required as author will be set and users can be added later - DTO
        public async Task<ServiceResult<ProgramResponseDto>> AddProgram(CreateProgramDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ProgramResponseDto>.Failure("No information provided");

            if (string.IsNullOrWhiteSpace(dto.WorkFlowName))
                return ServiceResult<ProgramResponseDto>.Failure("Work flow name is required.");

            try {
                var workFlow = new BusinessProgram {
                    ProgramName = dto.WorkFlowName,
                    Author = _tenantContext.UserName,
                    Description = dto.Description,
                    DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                    DueDate = dto.DueDate,
                    WorkFlowUsers = new List<WorkFlowUser>(),
                    TenantId = _tenantContext.TenantId
                };

                _logger.LogInformation(dto.Description);

                // Add creator as admin
                workFlow.WorkFlowUsers.Add(new WorkFlowUser {
                    UserId = _tenantContext.UserId,
                    Role = "Admin"
                });

                // Add other assigned users
                if (dto.UserIds != null && dto.UserIds.Any()) {
                    foreach (var userId in dto.UserIds) {
                        if (userId != _tenantContext.UserId) {
                            workFlow.WorkFlowUsers.Add(new WorkFlowUser {
                                UserId = userId,
                                Role = "Standard"
                            });
                        }
                    }
                }

                _context.Programs.Add(workFlow);
                await _context.SaveChangesAsync();

                // Reload workflow for response
                var createdWorkflow = await _context.Programs
                    .Where(wf => wf.TenantId == _tenantContext.TenantId)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .Include(wf => wf.Project)
                    .FirstOrDefaultAsync(wf => wf.Id == workFlow.Id);

                _logger.LogInformation("Workflow '{ProgramName}' created successfully by user {UserId}",
                    dto.WorkFlowName, _tenantContext.UserId);

                // Return success
                return ServiceResult<ProgramResponseDto>.Success(
                    new ProgramResponseDto {
                        ProgramId = workFlow.Id,
                        ProgramName = workFlow.ProgramName,
                        CreatedUser = workFlow.Author,
                        DateCreated = workFlow.DateCreated,
                        Projects = createdWorkflow?.Project?.Select(p => new GetProgramProjectsDto {
                            ProjectId = p.Id,
                            ProjectTitle = p.ProjectTitle
                        }).ToList() ?? new List<GetProgramProjectsDto>(),
                        Users = createdWorkflow?.WorkFlowUsers?.Select(wu => new GetProgramUsersDto {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? "Unknown"
                        }).ToList() ?? new List<GetProgramUsersDto>()
                    },
                    "Workflow created successfully."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while creating workflow '{ProgramName}'", dto.WorkFlowName);
                return ServiceResult<ProgramResponseDto>.Failure(
                    "A database error occurred while creating the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while creating workflow '{ProgramName}'", dto.WorkFlowName);
                return ServiceResult<ProgramResponseDto>.Failure(
                    "An unexpected error occurred while creating the workflow.",
                    new[] { ex.Message });
            }
        }


        // Add users to existing workflow
        /*
         * Adds a list of users to the program
         * Confirms if the user first exists before adding to avoid clashes
         * Users many to many relationship WorFlowUsers (ProgramUsers) as a medium
         */
        public async Task<ServiceResult<List<UserDetailsDto>>> AddUserToProgram(AddUserProgramDto dto) {
            if (dto == null)
                return ServiceResult<List<UserDetailsDto>>.Failure("Input data is required.");

            if (dto.UserIds == null || !dto.UserIds.Any())
                return ServiceResult<List<UserDetailsDto>>.Failure("No user IDs were provided.");

            if (dto.programId <= 0)
                return ServiceResult<List<UserDetailsDto>>.Failure("Invalid workflow ID provided.");

            try {
                // Check if the workflow exists for current tenant
                var program = await _context.Programs
                    .Where(wf => wf.TenantId == _tenantContext.TenantId)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .Include(wf => wf.Project)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.programId);
                if (program == null)
                    return ServiceResult<List<UserDetailsDto>>.Failure($"Workflow with ID {dto.programId} not found.");

                // Get existing user IDs to avoid duplicates
                var existingUserIds = program.WorkFlowUsers
                    .Select(wfUser => wfUser.UserId)
                    .ToList();

                foreach (var userId in dto.UserIds) {

                    // Verify the user exists in Identity
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null) continue;

                    var wfUser = new WorkFlowUser {
                        WorkFlowId = dto.programId,
                        UserId = userId,
                        Role = "Standard"
                    };
                    _context.WorkFlowUsers.Add(wfUser);
                }

                await _context.SaveChangesAsync();

                var userList = await _context.WorkFlowUsers
                    .Where(u => u.WorkFlowId == dto.programId)
                    .Include(u => u.User)
                    .ToListAsync();

                _logger.LogInformation(
                    "Added new users to workflow '{ProgramName}' (ID: {WorkFlowId})",
                    program.ProgramName, program.Id
                );

                return ServiceResult<List<UserDetailsDto>>.Success(
                    userList.Select(u => new UserDetailsDto {
                        UserId = u.UserId,
                        UserEmail = u.User.Email ?? "Unknown",
                        UserName = u.User.UserName ?? "Unknown"
                    }).ToList()
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while adding users to workflow ID {WorkFlowId}", dto.programId);
                return ServiceResult<List<UserDetailsDto>>.Failure(
                    "A database error occurred while adding users to the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while adding users to workflow ID {WorkFlowId}", dto.programId);
                return ServiceResult<List<UserDetailsDto>>.Failure(
                    "An unexpected error occurred while adding users to the workflow.",
                    new[] { ex.Message });
            }
        }


        // GET
        // list of all workflows  
        public async Task<ServiceResult<List<ProgramResponseDto>>> GetAllPrograms() {
            try {
                var workFlows = await _context.Programs
                    .OrderByDescending(wf => wf.DateCreated)
                    .Where(wf => wf.TenantId == _tenantContext.TenantId)
                    .Include(wf => wf.Project)
                    .Include(wf => wf.Comments)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .ToListAsync();

                if (workFlows == null || !workFlows.Any()) {
                    _logger.LogWarning("No workflows found in the system for tenant ID {TenantId}.", _tenantContext.TenantId);
                    return ServiceResult<List<ProgramResponseDto>>.Failure("No workflows found.");
                }

                _logger.LogInformation("Retrieved all workflows successfully for tenant ID {TenantId}.", _tenantContext.TenantId);

                return ServiceResult<List<ProgramResponseDto>>.Success(
                    workFlows.Select(wf => new ProgramResponseDto {
                        ProgramId = wf.Id,
                        ProgramName = wf.ProgramName,
                        Description = wf.Description,
                        CreatedUser = wf.Author,
                        DateCreated = wf.DateCreated,
                        DueDate = wf.DueDate,
                        Projects = wf.Project?.Select(p => new GetProgramProjectsDto {
                            ProjectId = p.Id,
                            ProjectTitle = p.ProjectTitle
                        }).ToList() ?? new List<GetProgramProjectsDto>(),
                        Users = wf.WorkFlowUsers?.Select(wu => new GetProgramUsersDto {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? ""
                        }).ToList() ?? new List<GetProgramUsersDto>(),
                        Comments = wf.Comments?.Select(c => new GetProgramCommentsDto {
                            Id = c.Id,
                            Content = c.CommentContent
                        }).ToList()
                    }).ToList(),
                    "Workflows retrieved successfully."
                );
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving workflows for tenant ID {TenantId}.", _tenantContext.TenantId);
                return ServiceResult<List<ProgramResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving workflows.",
                    new[] { ex.Message });
            }
        }


        // Get workflow by ID
        public async Task<ServiceResult<ProgramResponseDto>> GetProgramById(GetProgramInfoDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ProgramResponseDto>.Failure("Input data is required.");
            if (dto.ProgramId <= 0)
                return ServiceResult<ProgramResponseDto>.Failure("Invalid workflow ID provided.");

            try {
                var workFlow = await _context.Programs
                    .Where(wf => wf.Id == dto.ProgramId
                                 && wf.TenantId == _tenantContext.TenantId)
                    .Include(wf => wf.Project)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync();

                if (workFlow == null) {
                    _logger.LogWarning("Workflow with ID {WorkFlowId} not found for tenant ID {TenantId}.",
                        dto.ProgramId, _tenantContext.TenantId);
                    return ServiceResult<ProgramResponseDto>.Failure(
                        $"Workflow with ID {dto.ProgramId} was not found.");
                }

                _logger.LogInformation("Retrieved workflow '{ProgramName}' (ID: {WorkFlowId}) successfully for tenant ID {TenantId}.",
                    workFlow.ProgramName, workFlow.Id, _tenantContext.TenantId);

                return ServiceResult<ProgramResponseDto>.Success(
                    new ProgramResponseDto {
                        ProgramId = workFlow.Id,
                        ProgramName = workFlow.ProgramName,
                        Description = workFlow.Description,
                        CreatedUser = workFlow.Author ?? "No creator user name added",
                        DateCreated = workFlow.DateCreated,
                        DueDate = workFlow.DueDate,
                        Projects = workFlow.Project?.Select(p => new GetProgramProjectsDto {
                            ProjectId = p.Id,
                            ProjectTitle = p.ProjectTitle
                        }).ToList() ?? new List<GetProgramProjectsDto>(),
                        Users = workFlow.WorkFlowUsers?.Select(wu => new GetProgramUsersDto {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? ""
                        }).ToList() ?? new List<GetProgramUsersDto>()
                    },
                    "Workflow retrieved successfully."
                );
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving workflow with ID {WorkFlowId} for tenant ID {TenantId}.",
                    dto.ProgramId, _tenantContext.TenantId);
                return ServiceResult<ProgramResponseDto>.Failure(
                    "An unexpected error occurred while retrieving the workflow.",
                    new[] { ex.Message });
            }
        }


        // Get Program details
        /*
         * This is the main entry point API for the front end
         * Contains all the information related to the Program with all id fields so that if
         * the front end want to update something i can do it with the id and point it to the right service updating only what is needed
         * instead of goint through everything to update something small
         */
        public async Task<ServiceResult<ProgramDetailsResponseDto>> GetProgramDetails(GetProgramInfoDto dto) {
            if (dto == null)
                return ServiceResult<ProgramDetailsResponseDto>.Failure("No information provided");

            if (dto.ProgramId <= 0)
                return ServiceResult<ProgramDetailsResponseDto>.Failure("Valid ID required");

            try {
                // Checks and pulls all the information from the Program
                var programs = await _context.Programs
                    .Include(p => p.Project!)
                        .ThenInclude(p => p.ProjectUsers)
                            .ThenInclude(u => u.User)
                    .Include(p => p.Project!)
                        .ThenInclude(t => t.PorjectTodos)
                    .Include(p => p.Comments!)
                        .ThenInclude(cu => cu.User)
                    .Include(pu => pu.WorkFlowUsers!)
                        .ThenInclude(u => u.User)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProgramId && p.TenantId == _tenantContext.TenantId);

                if (programs == null) {
                    _logger.LogInformation("No program found with ID: {programId}", dto.ProgramId);
                    return ServiceResult<ProgramDetailsResponseDto>.Failure($"No program found with ID: {dto.ProgramId}");
                }

                return ServiceResult<ProgramDetailsResponseDto>.Success(
                    new ProgramDetailsResponseDto {
                        ProgramId = programs.Id,
                        ProgramName = programs.ProgramName,
                        Description = programs.Description,
                        ProgramAuthor = programs.Author,
                        DateCreated = programs.DateCreated,
                        DueDate = programs.DueDate,
                        Projects = programs.Project?.Select(p => new ProgramProjectDetailsDto {
                            ProjectId = p.Id,
                            ProjectTitle = p.ProjectTitle,
                            ProjectDescription = p.ProjectDescription ?? "No Description",
                            DateCreated = p.DateCreated,
                            DueDate = p.DueDate,
                            Users = p.ProjectUsers.Select(u => new UserDetailsDto {
                                UserId = u.UserId,
                                UserName = u.User.UserName ?? "Unknown",
                                UserEmail = u.User.Email ?? "Unknown"
                            }).ToList(),
                            Todos = p.PorjectTodos?.Select(t => new ProgramTodoDetailsDto {
                                TodoId = t.Id,
                                isComplete = t.IsComplete,
                                Task = t.Task,
                                UserName = t.User?.UserName,
                                DateCreated = t.DateCreated,
                                DueDate = t.DueDate,
                            }).ToList()
                        }).ToList(),
                        Comments = programs.Comments?.Select(c => new ProgramCommentDetailsDto {
                            CommentId = c.Id,
                            Content = c.CommentContent ?? "No content",
                            UserName = c.User.UserName ?? "Unknown",
                            DateCreated = c.DateCreated
                        }).ToList(),
                        Users = programs.WorkFlowUsers.Select(pu => new UserDetailsDto {
                            UserId = pu.UserId,
                            UserName = pu.User.UserName ?? "Unknown",
                            UserEmail = pu.User.Email ?? "Unknown"
                        }).ToList(),
                    },
                    "Retrieved Program details successfully for requested program"
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error retrieving selected Program");
                return ServiceResult<ProgramDetailsResponseDto>.Failure(
                    "A Database error retrieving selected Program.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while retrieving selected Program");
                return ServiceResult<ProgramDetailsResponseDto>.Failure(
                    "An unexpected while retrieving selected Program.",
                    new[] { ex.Message });
            }
        }


        //UPDATE:
        // Update program
        /*
         * Updates all the program information
         * Does check on updates so it only updates sent fields not the entire entry
         */
        public async Task<ServiceResult<ProgramResponseDto>> UpdateProgram(UpdateProgramNameDto dto) {
            if (dto == null)
                return ServiceResult<ProgramResponseDto>.Failure("No information provided.");
            if (dto.ProgramId <= 0)
                return ServiceResult<ProgramResponseDto>.Failure("Invalid program ID provided.");

            try {
                var program = await _context.Programs
                    .Where(wf => wf.Id == dto.ProgramId && wf.TenantId == _tenantContext.TenantId)
                    .Include(wf => wf.Project)
                    .Include(wf => wf.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync();

                if (program == null) {
                    _logger.LogWarning("Workflow with ID {WorkFlowId} not found for update.", dto.ProgramId);
                    return ServiceResult<ProgramResponseDto>.Failure($"Workflow with ID {dto.ProgramId} was not found.");
                }

                if (!string.IsNullOrWhiteSpace(dto.ProgramName)) {
                    program.ProgramName = dto.ProgramName.Trim();
                    _logger.LogInformation("Atempting to change Program Name to {programName}", dto.ProgramName);
                }

                if (!string.IsNullOrWhiteSpace(dto.Description)) {
                    program.Description = dto.Description.Trim();
                    _logger.LogInformation("Attempting to change Program description to {description}", dto.Description);
                }

                if (dto.DueDate.HasValue) {
                    program.DueDate = dto.DueDate.Value;
                    _logger.LogInformation("Atempting to change Program due date to {dueDate}", dto.DueDate);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Program updated successfullly");

                return ServiceResult<ProgramResponseDto>.Success(
                    new ProgramResponseDto {
                        ProgramId = program.Id,
                        ProgramName = program.ProgramName,
                        Projects = program.Project?.Select(p => new GetProgramProjectsDto {
                            ProjectId = p.Id,
                            ProjectTitle = p.ProjectTitle
                        }).ToList() ?? new List<GetProgramProjectsDto>(),
                        Users = program.WorkFlowUsers?.Select(wu => new GetProgramUsersDto {
                            Id = wu.UserId,
                            UserName = wu.User?.UserName ?? ""
                        }).ToList() ?? new List<GetProgramUsersDto>()
                    },
                    $"Workflow name updated successfully to {dto.ProgramName}."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while updating workflow name for ID {WorkFlowId}", dto.ProgramId);
                return ServiceResult<ProgramResponseDto>.Failure(
                    "A database error occurred while updating the workflow name.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while updating workflow name for ID {WorkFlowId}", dto.ProgramId);
                return ServiceResult<ProgramResponseDto>.Failure(
                    "An unexpected error occurred while updating the workflow name.",
                    new[] { ex.Message });
            }
        }


        // DELETE services
        // Delete workflow
        public async Task<ServiceResult<ProgramResponseDto>> DeleteProgram(GetProgramInfoDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<ProgramResponseDto>.Failure("Input data is required.");
            if (dto.ProgramId <= 0)
                return ServiceResult<ProgramResponseDto>.Failure("Invalid workflow ID.");

            try {
                // Include tenant filtering
                var workFlow = await _context.Programs
                    .Where(w => w.Id == dto.ProgramId && w.TenantId == _tenantContext.TenantId)
                    .Include(w => w.Comments)
                    .Include(w => w.Project)
                    .Include(w => w.WorkFlowUsers)
                        .ThenInclude(wu => wu.User)
                    .FirstOrDefaultAsync();

                if (workFlow == null) {
                    _logger.LogWarning("Workflow with ID {WorkFlowId} not found for deletion.", dto.ProgramId);
                    return ServiceResult<ProgramResponseDto>.Failure($"Workflow with ID {dto.ProgramId} was not found.");
                }

                // Prepare response before deletion
                var response = new ProgramResponseDto {
                    ProgramId = workFlow.Id,
                    ProgramName = workFlow.ProgramName,
                    Projects = workFlow.Project?.Select(p => new GetProgramProjectsDto {
                        ProjectId = p.Id,
                        ProjectTitle = p.ProjectTitle
                    }).ToList() ?? new List<GetProgramProjectsDto>(),
                    Users = workFlow.WorkFlowUsers?.Select(wu => new GetProgramUsersDto {
                        Id = wu.UserId,
                        UserName = wu.User?.UserName ?? ""
                    }).ToList() ?? new List<GetProgramUsersDto>()
                };

                _context.Programs.Remove(workFlow);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow '{ProgramName}' (ID: {WorkFlowId}) deleted successfully.", workFlow.ProgramName, workFlow.Id);

                return ServiceResult<ProgramResponseDto>.Success(response, "Workflow deleted successfully.");
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while deleting workflow with ID {WorkFlowId}", dto.ProgramId);
                return ServiceResult<ProgramResponseDto>.Failure(
                    "A database error occurred while deleting the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while deleting workflow with ID {WorkFlowId}", dto.ProgramId);
                return ServiceResult<ProgramResponseDto>.Failure(
                    "An unexpected error occurred while deleting the workflow.",
                    new[] { ex.Message });
            }
        }


        // Remove user from workflow
        public async Task<ServiceResult<UserDetailsDto>> RemoveUserFromProgram(AddUserProgramDto dto) {
            if (dto == null)
                return ServiceResult<UserDetailsDto>.Failure("Input data is required.");

            if (!dto.UserIds.Any())
                return ServiceResult<UserDetailsDto>.Failure("User ID is required.");

            if (dto.programId <= 0)
                return ServiceResult<UserDetailsDto>.Failure("Invalid workflow ID provided.");

            try {
                foreach (var user in dto.UserIds) {
                    await _context.WorkFlowUsers
                         .Where(u => u.UserId == user && u.WorkFlowId == dto.programId)
                         .ExecuteDeleteAsync();
                }

                _logger.LogInformation("Users deleted successfully");

                return ServiceResult<UserDetailsDto>.Success(
                    new UserDetailsDto {
                    },
                    "Users removed from workflow successfully."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error");
                return ServiceResult<UserDetailsDto>.Failure(
                    "A database error occurred while removing the user from the workflow.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Database error");
                return ServiceResult<UserDetailsDto>.Failure(
                    "An unexpected error occurred while removing the user from the workflow.",
                    new[] { ex.Message });
            }
        }
    }
}
