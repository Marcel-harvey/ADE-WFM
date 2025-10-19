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
                .Include(wfComment => wfComment.Comments!)
                .ThenInclude(wfUser => wfUser.User)
                .FirstOrDefaultAsync(wfId => wfId.Id == workFlowId);

            var workFlowComment = workFlow?.Comments?.ToList() ?? new List<Comment>();

            return workFlowComment;
        }


        // Get all comments on project
        public async Task<List<Comment>> GetProjectComments(int projectId)
        {
            var projectComments = await _context.Comments
                .Where(projectComment => projectComment.ProjectId == projectId)
                .Include(user => user.User)
                .ToListAsync();

            return projectComments;
        }

        public async Task<List<Comment>> GetUserComments(string userId)
        {
            var userComments = await _context.Comments
                .Where(user => user.UserId == userId)
                .Include(user => user.User)
                .ToListAsync();

            return userComments;
        }


        // UPDATE services
        public async Task MarkCommentAsViewed(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
            }

            comment.IsViewed = true;
            await _context.SaveChangesAsync();
        }


        // ADD services
        // Add comment to workflow
        public async Task AddCommentToWorkFlow(AddCommentWorkFlowViewModel model)
        {
            var comment = new Comment
            {
                DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                CommentContent = model.Comment.CommentContent,
                UserId = model.Comment.UserId,
                WorkFlowId = model.Comment.WorkFlowId,
                IsViewed = false,
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }


        // DELETE services
        public async Task DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found.");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
