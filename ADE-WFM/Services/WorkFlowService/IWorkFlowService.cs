using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;

namespace ADE_WFM.Services.WorkFlowService
{
    public interface IWorkFlowService
    {
        // ADD services
        Task <ServiceResult<WorkFlowResponseDto>> AddWorkFlow(CreateWorkFlowDto dto);
        Task <ServiceResult<WorkFlowResponseDto>> AddUserToWorkFlow(AddUserWorkFlowDto dto);

        // GET services
        Task<ServiceResult<List<WorkFlowResponseDto>>> GetAllWorkFlows();
        Task<ServiceResult<WorkFlowResponseDto>> GetWorkFlowById(GetWorkFlowInfoDto dto);

        // UPDATE services
        Task <ServiceResult<WorkFlowResponseDto>> UpdateWorkFlowName(UpdateWorkFlowNameDto dto);
        
        // DELETE services
        Task <ServiceResult<WorkFlowResponseDto>> DeleteWorkFlow(GetWorkFlowInfoDto dto);
        Task <ServiceResult<WorkFlowResponseDto>> RemoveUserFromWorkFlow(RemoveUserFromWorkFlowDto dto);
    }
}
