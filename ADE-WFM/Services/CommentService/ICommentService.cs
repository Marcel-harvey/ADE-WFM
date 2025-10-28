using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.CommentDtos;
using ADE_WFM.Models.ViewModels.CommentViewModels;

namespace ADE_WFM.Services.CommentService
{
    public interface ICommentService
    {
        // CREATE services
        Task AddCommentToWorkFlow(AddCommentWorkFlowViewModel model);
        Task AddCommentToProject(AddCommentProjectViewModel model);

        // GET serivces
        Task<ServiceResult<List<GetCommentsResponseDto>>> GetWorkFlowComments(GetCommentsInSectionDto dto);
        Task <ServiceResult<List<GetCommentsResponseDto>>> GetProjectComments(GetCommentsInSectionDto dto);
        Task<ServiceResult<List<GetCommentsResponseDto>>> GetUserComments(GetUserCommentsDto dto);

        // UPDATE services
        Task MarkCommentAsViewed(int commentId);

        // DELETE services
        Task DeleteComment(int commentId);
    }
}

