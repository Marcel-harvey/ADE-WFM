using ADE_WFM.Models.ViewModels.CommentViewModels;
using ADE_WFM.Models;
using Microsoft.EntityFrameworkCore;
using ADE_WFM.Data;

namespace ADE_WFM.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET serivces
        // Get all comments on a workflow
        public async Task<List<Comment>> GetWorkFlowComments(int workFlowId)
        {
            var workFlow = await _context.WorkFlows
                .Include(wfComment => wfComment.Comment!)
                .ThenInclude(wfUser => wfUser.User)
                .FirstOrDefaultAsync(wfId => wfId.Id == workFlowId);

            var workFlowComment = workFlow?.Comment?.ToList() ?? new List<Comment>();

            return workFlowComment;
        }

        // UPDATE services

        // ADD services
        // Add comment to workflow
        public async Task AddCommentToWorkFlow(AddCommentWorkFlowViewModel model)
        {
            var comment = new Comment
            {
                DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                CommentContent = model.Comment.CommentContent,
                UserId = model.Comment.User.Id,
                WorkFlowId = model.Comment.WorkFlowId,
                IsViewed = false,
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }


        // DELETE services
    }
}
