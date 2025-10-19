using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels.CommentViewModels;

namespace ADE_WFM.Services.CommentService
{
    public interface ICommentService
    {
        // GET serivces
        Task<List<Comment>> GetWorkFlowComments(int workFlowId);

        // UPDATE services
        Task MarkCommentAsViewed(int commentId);

        // ADD services
        Task AddCommentToWorkFlow(AddCommentWorkFlowViewModel model);

        // DELETE services
        Task DeleteComment(int commentId);
    }
}
