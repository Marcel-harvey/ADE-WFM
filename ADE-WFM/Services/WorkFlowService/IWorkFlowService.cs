using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;

namespace ADE_WFM.Services.WorkFlowService
{
    public interface IWorkFlowService
    {
        // ADD services
        Task <ResponseCreateWorkFlowDto> AddWorkFlow(CreateWorkFlowDto dto);
        Task AddUserToWorkFlow(AddUserWorkFlowDto model);

        // GET services
        Task<List<ResponseGetWorkFlowsDto>> GetAllWorkFlows();
        Task<ResponseGetWorkFlowsDto> GetWorkFlowById(GetWorkFlowByIdDto dto);
        Task<List<ApplicationUser>> GetUsersInWorkFlow(int workFlowId);
        Task <List<Project>> GetProjectsInWorkFlow(int workFlowId);

        // UPDATE services
        Task UpdateWorkFlowName(UpdateWorkFlowNameViewModel model);
        
        // DELETE services
        Task DeleteWorkFlow(int workFlowId);
    }
}
