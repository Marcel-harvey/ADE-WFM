using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels;

namespace ADE_WFM.Services.WorkFlowService
{
    public interface IWorkFlowService
    {
        // GET services
        Task<List<WorkFlow>> GetAllWorkFlows();
        Task<WorkFlow> GetWorkFlowById(int id);
        Task<List<ApplicationUser>> GetUsersInWorkFlow(int workFlowId);
        Task<List<Comment>> GetWorkFlowComments(int workFlowId);

        // UPDATE services
        Task UpdateWorkFlowName(UpdateWorkFlowNameViewModel model);

        // ADD services
        Task AddWorkFlow(CreateWorkFlowViewModel model);
    }
}
