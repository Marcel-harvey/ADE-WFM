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
        public async Task AddUserToWorkFlow(AddUserWorkFlowDto model)
        {

            // Duplicate check
            var existingUserIds = await _context.WorkFlowUsers
                .Where(wfUser => wfUser.WorkFlowId == model.WorkFlowId)
                .Select(wfUser => wfUser.UserId)
                .ToListAsync();

            foreach (var user in model.UserIds)
            {
                if (!existingUserIds.Contains(user))
                {
                    var wfUser = new WorkFlowUser
                    {
                        WorkFlowId = model.WorkFlowId,
                        UserId = user,
                        Role = "Standard"
                    };

                    _context.WorkFlowUsers.Add(wfUser);
                }
            }

            await _context.SaveChangesAsync();
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
        public async Task UpdateWorkFlowName(UpdateWorkFlowNameViewModel model)
        {
            var workFlow = await _context.WorkFlows
                .FirstOrDefaultAsync(wfId => wfId.Id == model.WorkFlowId);

            if (workFlow == null)
            {
                throw new KeyNotFoundException($"Work flow with ID {model.WorkFlowId} was not found");
            }

            workFlow.WorkFlowName = model.WorkFlowName;

            await _context.SaveChangesAsync();
        }        


        // DELETE services
        // Delete workflow
        public async Task DeleteWorkFlow(int workFlowId)
        {
            var workFlow = await _context.WorkFlows
                .Include(w => w.Comments)
                .FirstOrDefaultAsync(w => w.Id == workFlowId);

            if (workFlow == null)
                throw new KeyNotFoundException($"Workflow with ID {workFlowId} was not found.");

            _context.WorkFlows.Remove(workFlow);
            await _context.SaveChangesAsync();
        }




    }
}
