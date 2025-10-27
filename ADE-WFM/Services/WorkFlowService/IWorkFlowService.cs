using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;

namespace ADE_WFM.Services.WorkFlowService
{
    public interface IWorkFlowService
    {
        // ADD services
        Task <ServiceResult<CreateWorkFlowResponseDto>> AddWorkFlow(CreateWorkFlowDto dto);
        Task <ServiceResult<AddUserWorkFlowResponseDto>> AddUserToWorkFlow(AddUserWorkFlowDto dto);

        // GET services
        Task<ServiceResult<List<GetAllWorkFlowsDtoResponse>>> GetAllWorkFlows();
        Task<ServiceResult<GetAllWorkFlowsDtoResponse>> GetWorkFlowById(GetWorkFlowByIdDto dto);

        // UPDATE services
        Task <ServiceResult<UpdateWorkFlowNameResponseDto>> UpdateWorkFlowName(UpdateWorkFlowNameDto dto);
        
        // DELETE services
        Task <ServiceResult<DeleteWorkFlowResponseDto>> DeleteWorkFlow(DeleteWorkFlowDto dto);
        Task <ServiceResult<RemoveUserFromWorkFlowResponseDto>> RemoveUserFromWorkFlow(RemoveUserFromWorkFlowDto dto);
    }
}
