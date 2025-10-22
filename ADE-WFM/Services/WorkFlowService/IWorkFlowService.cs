using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;

namespace ADE_WFM.Services.WorkFlowService
{
    public interface IWorkFlowService
    {
        // GET services
        Task<List<ResponseGetAllWorkFlowsDto>> GetAllWorkFlows();
        Task<WorkFlow> GetWorkFlowById(int id);
        Task<List<ApplicationUser>> GetUsersInWorkFlow(int workFlowId);
        Task <List<Project>> GetProjectsInWorkFlow(int workFlowId);

        // UPDATE services
        Task UpdateWorkFlowName(UpdateWorkFlowNameViewModel model);

        // ADD services
        Task AddWorkFlow(CreateWorkFlowViewModel model);
        Task AddUserToWorkFlow(AddUserWorkFlowViewModel model);
        
        // DELETE services
        Task DeleteWorkFlow(int workFlowId);
    }
}
