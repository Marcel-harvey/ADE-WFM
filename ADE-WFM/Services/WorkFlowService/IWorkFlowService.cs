using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;

namespace ADE_WFM.Services.WorkFlowService
{
    public interface IWorkFlowService
    {
        // ADD services
        Task <ResponseCreateWorkFlowDto> AddWorkFlow(CreateWorkFlowDto dto);
        Task <ResponseAddUserWorkFlowDto> AddUserToWorkFlow(AddUserWorkFlowDto dto);

        // GET services
        Task<List<ResponseGetWorkFlowsDto>> GetAllWorkFlows();
        Task<ResponseGetWorkFlowsDto> GetWorkFlowById(GetWorkFlowByIdDto dto);

        // UPDATE services
        Task <ResponseUpdateWorkFlowNameDto> UpdateWorkFlowName(UpdateWorkFlowNameDto dto);
        
        // DELETE services
        Task <ResponseDeleteWorkFlowDto> DeleteWorkFlow(DeleteWorkFlowDto dto);
        Task <ResponseRemoveUserFromWorkFlowDto> RemoveUserFromWorkFlow(RemoveUserFromWorkFlowDto dto);
    }
}
