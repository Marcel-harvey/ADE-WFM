using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.CommentDtos;

namespace ADE_WFM.Services.CommentService
{
    public interface ICommentService
    {
        // CREATE services
        Task<ServiceResult<AddCommentResponseDto>> AddCommentToWorkFlow(AddCommentDto dto);
        Task<ServiceResult<AddCommentResponseDto>> AddCommentToProject(AddCommentDto dto);

        // GET serivces
        Task<ServiceResult<List<GetCommentsResponseDto>>> GetWorkFlowComments(GetCommentsInSectionDto dto);
        Task <ServiceResult<List<GetCommentsResponseDto>>> GetProjectComments(GetCommentsInSectionDto dto);
        Task<ServiceResult<List<GetCommentsResponseDto>>> GetUserComments(GetUserCommentsDto dto);

        // UPDATE services
        Task<ServiceResult<UpdateCommentViewedResponseDto>> MarkCommentAsViewed(UpdateCommentViewedDto dto);

        // DELETE services
        Task<ServiceResult<DeleteCommentResponseDto>> DeleteComment(DeleteCommentDto dto);
    }
}

