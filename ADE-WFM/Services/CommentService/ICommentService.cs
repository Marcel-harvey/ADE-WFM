using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.CommentDtos;

namespace ADE_WFM.Services.CommentService
{
    public interface ICommentService
    {
        // CREATE services
        Task<ServiceResult<CommentResponseDto>> AddCommentToWorkFlow(AddCommentDto dto);
        Task<ServiceResult<CommentResponseDto>> AddCommentToProject(AddCommentDto dto);

        // GET serivces
        Task<ServiceResult<List<CommentResponseDto>>> GetWorkFlowComments(GetCommentsInSectionDto dto);
        Task <ServiceResult<List<CommentResponseDto>>> GetProjectComments(GetCommentsInSectionDto dto);
        Task<ServiceResult<List<CommentResponseDto>>> GetUserComments(GetUserCommentsDto dto);

        // UPDATE services
        Task<ServiceResult<CommentResponseDto>> MarkCommentAsViewed(UpdateCommentViewedDto dto);

        // DELETE services
        Task<ServiceResult<CommentResponseDto>> DeleteComment(DeleteCommentDto dto);
    }
}

