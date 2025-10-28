using ADE_WFM.Models.ViewModels.CommentViewModels;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using ADE_WFM.Data;
using ADE_WFM.Models.DTOs.CommentDtos;
using Microsoft.AspNetCore.Identity;
using ADE_WFM.Models.DTOs.ProjectDtos;
using Microsoft.Extensions.Logging;

namespace ADE_WFM.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CommentService> _logger;

        public CommentService(

            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<CommentService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET serivces
        // Get all comments on a workflow
        public async Task<ServiceResult<List<GetCommentsResponseDto>>> GetWorkFlowComments(GetCommentsInSectionDto dto)
        {
            if (dto == null)
                return ServiceResult<List<GetCommentsResponseDto>>.Failure("Invalid request data.");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<List<GetCommentsResponseDto>>.Failure("Invalid WorkFlow ID.");

            try
            {
                var workflow = await _context.WorkFlows
                    .Include(wf => wf.Comments!)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId);

                if (workflow == null || workflow.Comments == null || !workflow.Comments.Any())
                {
                    _logger.LogInformation("No comments found for WorkFlow ID: {WorkFlowId}", dto.WorkFlowId);
                    return ServiceResult<List<GetCommentsResponseDto>>.Success(new List<GetCommentsResponseDto>(), "No comments found for the specified workflow.");
                }

                _logger.LogInformation("Successfully retrieved all comments in work flow {workFlowName}", workflow.WorkFlowName);
                return ServiceResult<List<GetCommentsResponseDto>>.Success(
                    workflow.Comments
                    .Select(c => new GetCommentsResponseDto
                    {
                        CommentId = c.Id,
                        CommentContent = c.CommentContent,
                        DateCreated = c.DateCreated,
                        WorkFlowName = workflow.WorkFlowName,
                        UserName = c.User?.UserName ?? "Unknown",
                    }).ToList(),
                    $"Work flow '{workflow.WorkFlowName}' comments retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow comments for WorkFlow ID {WorkFlowId}", dto.WorkFlowId);
                return ServiceResult<List<GetCommentsResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving workflow comments.",
                    new[] { ex.Message });
            }
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
                UserId = model.UserId,
                WorkFlowId = model.Comment.WorkFlowId,
                IsViewed = false,
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }


        // Add comment to project
        public async Task AddCommentToProject(AddCommentProjectViewModel model)
        {
            var comment = new Comment
            {
                DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                CommentContent = model.Comment.CommentContent,
                UserId = model.UserId,
                ProjectId = model.Comment.ProjectId,
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
