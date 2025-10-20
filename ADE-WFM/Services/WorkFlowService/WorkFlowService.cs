using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels.CommentViewModels;
using ADE_WFM.Models.ViewModels.WorkFlowViewModels;
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


        // GET
        // list of all workflows  
        public async Task<List<WorkFlow>> GetAllWorkFlows()
        {
            var workFlow = await _context.WorkFlows
                .Include(project => project.Project)
                .ToListAsync();

            return workFlow;
        }


        // Get workflow by ID
        public async Task<WorkFlow> GetWorkFlowById(int id)
        {
            var workFlow = await _context.WorkFlows
                .FindAsync(id);

            if (workFlow == null)
            {
                throw new KeyNotFoundException($"Work flow with ID: {id} was not found");
            }

            return workFlow;
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


        // ADD:
        // Add new workflow with user the created and extra list of users if selected
        public async Task AddWorkFlow(CreateWorkFlowViewModel model)
        {

            var workFlow = new WorkFlow
            {
                WorkFlowName = model.WorkFlowName,
                WorkFlowUsers = new List<WorkFlowUser>(),
            };

            workFlow.WorkFlowUsers.Add(new WorkFlowUser
            {
                UserId = model.CurrentUserId,
                Role = "Admin",
            });

            foreach (var userId in model.UserIds)
            {
                if (userId != model.CurrentUserId)
                {
                    workFlow.WorkFlowUsers.Add(new WorkFlowUser
                    {
                        UserId = userId,
                        Role = "Standard",
                    });
                }                
            }

            _context.WorkFlows.Add(workFlow);
            await _context.SaveChangesAsync();
        }


        // Add users to existing workflow
        public async Task AddUserToWorkFlow(AddUserWorkFlowViewModel model)
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
