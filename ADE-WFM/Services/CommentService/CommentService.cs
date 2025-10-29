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


        // ADD services
        // Add comment to workflow
        public async Task<ServiceResult<AddCommentResponseDto>> AddCommentToWorkFlow(AddCommentDto dto)
        {
            if (dto == null)
                return ServiceResult<AddCommentResponseDto>.Failure("Invalid request data.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<AddCommentResponseDto>.Failure("User ID is required.");

            if (string.IsNullOrWhiteSpace(dto.CommentContent))
                return ServiceResult<AddCommentResponseDto>.Failure("Comment content cannot be empty.");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<AddCommentResponseDto>.Failure("Valid WorkFlow ID is required.");


            var user = await _userManager
                .FindByIdAsync(dto.UserId);
            if (user == null)
                return ServiceResult<AddCommentResponseDto>.Failure("User not found.");

            var workFlow = await _context.WorkFlows
                .FindAsync(dto.WorkFlowId);
            if (workFlow == null)
                return ServiceResult<AddCommentResponseDto>.Failure("WorkFlow not found.");

            try
            {
                var comment = new Comment
                {
                    DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                    CommentContent = dto.CommentContent,
                    UserId = dto.UserId,
                    WorkFlowId = dto.WorkFlowId,
                    IsViewed = false,
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                return ServiceResult<AddCommentResponseDto>.Success(
                    new AddCommentResponseDto
                    {
                        Id = comment.Id,
                        DateCreated = comment.DateCreated,
                        CommentContent = comment.CommentContent,
                        UserName = user.UserName ?? "Unknown",
                        WorkFlowId = comment.WorkFlowId,
                    },
                    "Comment added to workflow successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error adding comment to work flow");
                return ServiceResult<AddCommentResponseDto>.Failure(
                    "A database error occurred while adding new comment to work flow.",
                    new[] { ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding comment to work flow");
                return ServiceResult<AddCommentResponseDto>.Failure(
                    "An unexpected error occurred while adding new comment to work flow.",
                    new[] { ex.Message }
                );
            }
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


        // GET serivces
        // Get all comments on a workflow
        public async Task<ServiceResult<List<GetCommentsResponseDto>>> GetWorkFlowComments(GetCommentsInSectionDto dto)
        {
            if (dto == null)
                return ServiceResult<List<GetCommentsResponseDto>>.Failure("Invalid request data.");

            if (dto.Id <= 0)
                return ServiceResult<List<GetCommentsResponseDto>>.Failure("Invalid WorkFlow ID.");

            try
            {
                var workflow = await _context.WorkFlows
                    .Include(wf => wf.Comments!)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.Id);

                // Check if workflow exists first before accessing comments
                if (workflow == null)
                {
                    _logger.LogInformation("WorkFlow not found for WorkFlow ID: {WorkFlowId}", dto.Id);
                    return ServiceResult<List<GetCommentsResponseDto>>.Failure("WorkFlow not found.");
                }

                if (workflow.Comments == null || !workflow.Comments.Any())
                {
                    _logger.LogInformation("No comments found for WorkFlow ID: {WorkFlowId}", dto.Id);
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
                            SectionName = workflow.WorkFlowName,
                            UserName = c.User?.UserName ?? "Unknown",
                        }).ToList(),
                        $"Work flow '{workflow.WorkFlowName}' comments retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow comments for WorkFlow ID {WorkFlowId}", dto.Id);
                return ServiceResult<List<GetCommentsResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving workflow comments.",
                    new[] { ex.Message });
            }
        }


        // Get all comments on project
        public async Task<ServiceResult<List<GetCommentsResponseDto>>> GetProjectComments(GetCommentsInSectionDto dto)
        {
            if (dto == null)
                return ServiceResult<List<GetCommentsResponseDto>>.Failure("Invalid request data.");

            if (dto.Id <= 0)
                return ServiceResult<List<GetCommentsResponseDto>>.Failure("Invalid Project ID.");

            try
            {
                var project = await _context.Projects
                    .Include(p => p.Comment!)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.Id);

                // Check if project exists first before accessing comments
                if (project == null)
                {
                    _logger.LogInformation("Project not found for project ID: {ProjectId}", dto.Id);
                    return ServiceResult<List<GetCommentsResponseDto>>.Failure("Project not found.");
                }

                    if (project.Comment == null || !project.Comment.Any())
                {
                    _logger.LogInformation("No comments found for project ID: {ProjectId}", dto.Id);
                    return ServiceResult<List<GetCommentsResponseDto>>.Success(new List<GetCommentsResponseDto>(), "No comments found for the specified Project.");
                }

                _logger.LogInformation("Successfully retrieved all comments in project {ProjectTitle}", project.ProjectTitle);
                return ServiceResult<List<GetCommentsResponseDto>>.Success(
                    project.Comment
                        .Select(c => new GetCommentsResponseDto
                        {
                            CommentId = c.Id,
                            CommentContent = c.CommentContent,
                            DateCreated = c.DateCreated,
                            SectionName = project.ProjectTitle,
                            UserName = c.User?.UserName ?? "Unknown",
                        }).ToList(),
                        $"Project'{project.ProjectTitle}' comments retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project comments for project ID {ProjectTitle}", dto.Id);
                return ServiceResult<List<GetCommentsResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving project comments.",
                    new[] { ex.Message });
            }
        }


        // Get all comments a user made
        public async Task<ServiceResult<List<GetCommentsResponseDto>>> GetUserComments(GetUserCommentsDto dto)
        {
            if (dto == null)
                return ServiceResult<List<GetCommentsResponseDto>>.Failure("Invalid request data.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<List<GetCommentsResponseDto>>.Failure("Invalid User ID.");

            try
            {
                var user = await _context.Users
                    .Include(u => u.Comment!)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.UserId);

                // Check if user exists first before accessing comments
                if (user == null)
                {
                    _logger.LogInformation("User not found for user ID: {UserId}", dto.UserId);
                    return ServiceResult<List<GetCommentsResponseDto>>.Failure("User not found.");
                }


                if (user.Comment == null || !user.Comment.Any())
                {
                    _logger.LogInformation("No comments found for user ID: {UserId}", dto.UserId);
                    return ServiceResult<List<GetCommentsResponseDto>>.Success(new List<GetCommentsResponseDto>(), "No comments found for the specified User.");
                }

                _logger.LogInformation("Successfully retrieved all comments for user {UserName}", user.UserName);
                return ServiceResult<List<GetCommentsResponseDto>>.Success(
                    user.Comment
                        .Select(c => new GetCommentsResponseDto
                        {
                            CommentId = c.Id,
                            CommentContent = c.CommentContent,
                            DateCreated = c.DateCreated,
                            UserName = user.UserName ?? "Unknown",
                        }).ToList(),
                        $"User'{user.UserName}' comments retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user comments for user ID {UserId}", dto.UserId);
                return ServiceResult<List<GetCommentsResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving project comments.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services
        public async Task<ServiceResult<UpdateCommentViewedResponseDto>> MarkCommentAsViewed(UpdateCommentViewedDto dto)
        {
            if (dto == null)
                return ServiceResult<UpdateCommentViewedResponseDto>.Failure("Invalid request data.");

            if (dto.CommentId <= 0)
                return ServiceResult<UpdateCommentViewedResponseDto>.Failure("Invalid Comment ID.");

            try
            {
                var comment = await _context.Comments
                    .FindAsync(dto.CommentId);
                if (comment == null)
                {
                    _logger.LogInformation("Comment not found for Comment ID: {CommentId}", dto.CommentId);
                    return ServiceResult<UpdateCommentViewedResponseDto>.Failure("Comment not found.");
                }

                if (comment.IsViewed)
                {
                    _logger.LogInformation("Comment ID {CommentId} is already marked as viewed.", dto.CommentId);
                    return ServiceResult<UpdateCommentViewedResponseDto>.Success(
                        new UpdateCommentViewedResponseDto
                        {
                            CommentId = comment.Id,
                            IsViewed = true
                        },
                        "Comment was already marked as viewed."
                    );
                }

                comment.IsViewed = dto.IsViewed;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Comment ID {CommentId} marked as viewed successfully.", comment.Id);

                return ServiceResult<UpdateCommentViewedResponseDto>.Success(
                    new UpdateCommentViewedResponseDto
                    {
                        CommentId = comment.Id,
                        IsViewed = comment.IsViewed
                    },
                    $"Comment ID {comment.Id} marked as viewed successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while marking comment as viewed (ID: {CommentId})", dto.CommentId);
                return ServiceResult<UpdateCommentViewedResponseDto>.Failure(
                    "A database error occurred while marking comment as viewed.",
                    new[] { ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while marking comment as viewed (ID: {CommentId})", dto.CommentId);
                return ServiceResult<UpdateCommentViewedResponseDto>.Failure(
                    "An unexpected error occurred while marking comment as viewed.",
                    new[] { ex.Message }
                );
            }
        }


        // DELETE services
        public async Task<ServiceResult<DeleteCommentResponseDto>> DeleteComment(DeleteCommentDto dto)
        {
            if (dto == null)
                return ServiceResult<DeleteCommentResponseDto>.Failure("Invalid request data.");

            if (dto.CommentId <= 0)
                return ServiceResult<DeleteCommentResponseDto>.Failure("Invalid Comment ID.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<DeleteCommentResponseDto>.Failure("Invalid User ID.");

            try
            {
                var comment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.Id == dto.CommentId && c.UserId == dto.UserId);

                if (comment == null)
                {
                    _logger.LogInformation("Comment not found or user unauthorized for Comment ID: {CommentId}", dto.CommentId);
                    return ServiceResult<DeleteCommentResponseDto>.Failure("Comment not found or user unauthorized, user can only delete own comments.");
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return ServiceResult<DeleteCommentResponseDto>.Success(
                    new DeleteCommentResponseDto
                    {
                        CommentId = comment.Id,
                    },
                    "Comment deleted successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting comment ID: {CommentId}", dto.CommentId);
                return ServiceResult<DeleteCommentResponseDto>.Failure(
                    "A database error occurred while deleting comment.",
                    new[] { ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting comment ID: {CommentId}", dto.CommentId);
                return ServiceResult<DeleteCommentResponseDto>.Failure(
                    "An unexpected error occurred while deleting comment.",
                    new[] { ex.Message }
                );
            }



        }
    }
}
