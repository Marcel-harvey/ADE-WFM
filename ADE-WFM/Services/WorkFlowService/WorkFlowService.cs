using ADE_WFM.Data;
using ADE_WFM.Models;
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

        public WorkFlowService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // CREATE:
        // Add new workflow with user the created and extra list of users if selected
        public async Task<ResponseCreateWorkFlowDto> AddWorkFlow(CreateWorkFlowDto dto)
        {
            // Create the new workflow entity
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

            // Save workflow to database
            _context.WorkFlows.Add(workFlow);
            await _context.SaveChangesAsync();

            // Build response DTO
            var response = new ResponseCreateWorkFlowDto
            {
                Id = workFlow.Id,
                WorkFlowName = workFlow.WorkFlowName,
                CreatedByUserId = dto.CurrentUserId,
                AssignedUserIds = workFlow.WorkFlowUsers.Select(u => u.UserId).ToList(),
                CreatedAt = DateTime.UtcNow,
                Message = "Workflow created successfully"
            };

            return response;
        }



        // Add users to existing workflow
        public async Task<ResponseAddUserWorkFlowDto> AddUserToWorkFlow(AddUserWorkFlowDto model)
        {
            // Check if the workflow exists
            var workFlow = await _context.WorkFlows
                .Include(wf => wf.WorkFlowUsers)
                .FirstOrDefaultAsync(wf => wf.Id == model.WorkFlowId)
                ?? throw new KeyNotFoundException($"Workflow with ID {model.WorkFlowId} not found.");

            var existingUserIds = workFlow.WorkFlowUsers
                .Select(wfUser => wfUser.UserId)
                .ToList();

            var addedUsers = new List<WorkFlowUserDto>();

            foreach (var userId in model.UserIds)
            {
                if (!existingUserIds.Contains(userId))
                {

                    // Verify the user exists in Identity
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                        throw new KeyNotFoundException($"User with ID {userId} not found.");

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
            }

            await _context.SaveChangesAsync();

            return new ResponseAddUserWorkFlowDto
            {
                WorkFlowId = workFlow.Id,
                WorkFlowName = workFlow.WorkFlowName,
                Users = addedUsers,
                Message = addedUsers.Any()
                    ? "Users added successfully."
                    : "No new users were added (duplicates skipped)."
            };
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

            var response = workflows.Select(wf => new ResponseGetWorkFlowsDto
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

            return response;
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

            var response = new ResponseGetWorkFlowsDto
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

            return response;
        }


        // Get users that is part of the workflow
        public async Task<List<ApplicationUser>> GetUsersInWorkFlow(int workFlowId)
        {
            var workFlowUsers = await _context.WorkFlowUsers
                .Where(id => id.WorkFlowId == workFlowId)
                .Select(wfUser => wfUser.User)
                .ToListAsync();

            return workFlowUsers;
        }

        public async Task<List<Project>> GetProjectsInWorkFlow(int workFlowId)
        {
            var projects = await _context.Projects
                .Where(p => p.WorkFlowId == workFlowId)
                .ToListAsync();

            if (projects == null)
            {
                throw new KeyNotFoundException($"Key with ID: {workFlowId} was not found");
            }

            return projects;
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
