using ADE_WFM.Models.ViewModels.CommentViewModels;

namespace ADE_WFM.Services.CommentService
{
    public interface ICommentService
    {
        // GET serivces

        // UPDATE services

        // ADD services
        Task AddCommentToWorkFlow(AddCommentWorkFlowViewModel model);

        // DELETE services
    }
}
