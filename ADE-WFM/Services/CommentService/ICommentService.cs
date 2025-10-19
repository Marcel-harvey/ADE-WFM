using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels.CommentViewModels;

namespace ADE_WFM.Services.CommentService
{
    public interface ICommentService
    {
        // GET serivces
        Task<List<Comment>> GetWorkFlowComments(int workFlowId);
        Task <List<Comment>> GetProjectComments(int projectId);
        Task<List<Comment>> GetUserComments(string userId);

        // UPDATE services
        Task MarkCommentAsViewed(int commentId);

        // ADD services
        Task AddCommentToWorkFlow(AddCommentWorkFlowViewModel model);
        Task AddCommentToProject(AddCommentProjectViewModel model);

        // DELETE services
        Task DeleteComment(int commentId);
    }
}
