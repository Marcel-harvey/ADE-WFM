using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels;
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


        // Get all comments on a workflow
        public async Task<List<Comment>> GetWorkFlowComments(int workFlowId)
        {
            var workFlow = await _context.WorkFlows
                .Include(wfComment => wfComment.Comment!)
                .ThenInclude(wfUser => wfUser.User)
                .FirstOrDefaultAsync(wfId => wfId.Id == workFlowId);

            var workFlowComment = workFlow?.Comment?.ToList() ?? new List<Comment>();

            return workFlowComment;
        }


        //UPDATE:
        // Update workflow's name
        public async Task UpdateWorkFlowName(UpdateWorkFlowNameViewModel model)
        {
            var workFlow = await _context.WorkFlows
                .FirstOrDefaultAsync(wfId => wfId.Id == model.WorkFlow.Id);

            if (workFlow == null)
            {
                throw new KeyNotFoundException($"Work flow with ID {model.WorkFlow.Id} was not found");
            }

            workFlow.WorkFlowName = model.WorkFlowName;

            await _context.SaveChangesAsync();
        }


        // ADD:
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
    }
}
