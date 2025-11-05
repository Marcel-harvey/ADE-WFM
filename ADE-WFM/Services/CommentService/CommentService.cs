using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using ADE_WFM.Data;
using ADE_WFM.Models.DTOs.CommentDtos;
using Microsoft.AspNetCore.Identity;
using ADE_WFM.Models.DTOs.ProjectDtos;
using Microsoft.Extensions.Logging;
using ADE_WFM.Services.TenantService;

namespace ADE_WFM.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CommentService> _logger;
        private readonly TenantContext _tenantContext;

        public CommentService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<CommentService> logger,
            TenantContext tenantContext)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _tenantContext = tenantContext;
        }


        // ADD services
        // Add comment to workflow
        public async Task<ServiceResult<CommentResponseDto>> AddCommentToWorkFlow(AddCommentDto dto)
        {
            // General validations
            if (dto == null)
                return ServiceResult<CommentResponseDto>.Failure("Invalid request data.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<CommentResponseDto>.Failure("User ID is required.");

            if (string.IsNullOrWhiteSpace(dto.CommentContent))
                return ServiceResult<CommentResponseDto>.Failure("Comment content cannot be empty.");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<CommentResponseDto>.Failure("Valid Work flow ID is required.");

            try
            {
                var user = await _userManager
                    .FindByIdAsync(dto.UserId);
                if (user == null)
                    return ServiceResult<CommentResponseDto>.Failure("User not found.");

                var workFlow = await _context.WorkFlows
                    .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId && wf.TenantId == _tenantContext.TenantId);
                if (workFlow == null)
                    return ServiceResult<CommentResponseDto>.Failure("Work flow not found.");

                var comment = new Comment
                {
                    DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                    CommentContent = dto.CommentContent,
                    UserId = dto.UserId,
                    WorkFlowId = workFlow.Id,
                    IsViewed = false,
                    TenantId = _tenantContext.TenantId,
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                return ServiceResult<CommentResponseDto>.Success(
                    new CommentResponseDto
                    {
                        CommentId = comment.Id,
                        DateCreated = comment.DateCreated,
                        CommentContent = comment.CommentContent,
                        UserId = user.Id,
                        UserName = user.UserName ?? "Unknown",
                        WorkFlowId = workFlow.Id,
                        WorkFlowName = workFlow.WorkFlowName,
                    },
                    "Comment added to work flow successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error adding comment to work flow");
                return ServiceResult<CommentResponseDto>.Failure("A database error occurred while adding new comment to work flow.",
                    new[] { ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding comment to work flow");
                return ServiceResult<CommentResponseDto>.Failure("An unexpected error occurred while adding new comment to work flow.",
                    new[] { ex.Message }
                );
            }
        }


        // Add comment to project
        public async Task<ServiceResult<CommentResponseDto>> AddCommentToProject(AddCommentDto dto)
        {
            // General validations
            if (dto == null)
                return ServiceResult<CommentResponseDto>.Failure("Invalid request data.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<CommentResponseDto>.Failure("User ID is required.");

            if (string.IsNullOrWhiteSpace(dto.CommentContent))
                return ServiceResult<CommentResponseDto>.Failure("Comment content cannot be empty.");

            if (dto.ProjectId <= 0)
                return ServiceResult<CommentResponseDto>.Failure("Valid project ID is required.");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<CommentResponseDto>.Failure("Valid Work flow ID is required.");

            try
            {
                var user = await _userManager
                    .FindByIdAsync(dto.UserId);
                if (user == null)
                    return ServiceResult<CommentResponseDto>.Failure("User not found.");

                var project = await _context.Projects
                    .Include(wf => wf.WorkFlows)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.TenantId == _tenantContext.TenantId);
                if (project == null)
                    return ServiceResult<CommentResponseDto>.Failure("Project not found.");

                // Need to supply work flow id for relationship
                var comment = new Comment
                {
                    DateCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                    CommentContent = dto.CommentContent,
                    UserId = dto.UserId,
                    ProjectId = dto.ProjectId,
                    WorkFlowId = project.WorkFlowId,
                    IsViewed = false,
                    TenantId =_tenantContext.TenantId,
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                return ServiceResult<CommentResponseDto>.Success(
                    new CommentResponseDto
                    {
                        CommentId = comment.Id,
                        DateCreated = comment.DateCreated,
                        CommentContent = comment.CommentContent,
                        UserId = user.Id,
                        UserName = user.UserName ?? "Unknown",
                        ProjectId = comment.ProjectId,
                        ProjectTitle = project.ProjectTitle,
                        WorkFlowId = project.WorkFlowId,
                        WorkFlowName = project.WorkFlows.WorkFlowName,
                    },
                    "Comment added to project successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error adding comment to project");
                return ServiceResult<CommentResponseDto>.Failure(
                    "A database error occurred while adding new comment to project.",
                    new[] { ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding comment to project");
                return ServiceResult<CommentResponseDto>.Failure(
                    "An unexpected error occurred while adding new comment to project.",
                    new[] { ex.Message }
                );
            }
        }


        // GET serivces
        // Get all comments on a workflow
        public async Task<ServiceResult<List<CommentResponseDto>>> GetWorkFlowComments(GetCommentInfoDto dto)
        {
            if (dto == null)
                return ServiceResult<List<CommentResponseDto>>.Failure("Invalid request data.");

            if (dto.WorkFlowId <= 0)
                return ServiceResult<List<CommentResponseDto>>.Failure("Invalid WorkFlow ID.");

            try
            {
                var workflow = await _context.WorkFlows
                    .Include(wf => wf.Comments!)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.WorkFlowId && wf.TenantId == _tenantContext.TenantId);

                // Check if workflow exists first before accessing comments
                if (workflow == null)
                {
                    _logger.LogInformation("WorkFlow not found for WorkFlow ID: {WorkFlowId}", dto.WorkFlowId);
                    return ServiceResult<List<CommentResponseDto>>.Failure("WorkFlow not found.");
                }

                if (workflow.Comments == null || !workflow.Comments.Any())
                {
                    _logger.LogInformation("No comments found for WorkFlow ID: {WorkFlowId}", dto.WorkFlowId);
                    return ServiceResult<List<CommentResponseDto>>.Success(new List<CommentResponseDto>(), "No comments found for the specified workflow.");
                }

                _logger.LogInformation("Successfully retrieved all comments in work flow {workFlowName}", workflow.WorkFlowName);
                return ServiceResult<List<CommentResponseDto>>.Success(
                    workflow.Comments
                        .Select(c => new CommentResponseDto
                        {
                            CommentId = c.Id,
                            CommentContent = c.CommentContent,
                            DateCreated = c.DateCreated,
                            IsViewed = c.IsViewed,
                            UserId = c.UserId,
                            UserName = c.User?.UserName ?? "Unknown",
                            WorkFlowId = workflow.Id,
                            WorkFlowName = workflow.WorkFlowName,
                        }).ToList(),
                        $"Work flow '{workflow.WorkFlowName}' comments retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow comments for WorkFlow ID {WorkFlowId}", dto.WorkFlowId);
                return ServiceResult<List<CommentResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving workflow comments.",
                    new[] { ex.Message });
            }
        }


        // Get all comments on project
        public async Task<ServiceResult<List<CommentResponseDto>>> GetProjectComments(GetCommentInfoDto dto)
        {
            if (dto == null)
                return ServiceResult<List<CommentResponseDto>>.Failure("Invalid request data.");

            if (dto.ProjectId <= 0)
                return ServiceResult<List<CommentResponseDto>>.Failure("Invalid Project ID.");

            try
            {
                var project = await _context.Projects
                    .Include(p => p.Comment!)
                        .ThenInclude(c => c.User)
                    .Include(p => p.WorkFlows)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.ProjectId);

                // Check if project exists first before accessing comments
                if (project == null)
                {
                    _logger.LogInformation("Project not found for project ID: {ProjectId}", dto.ProjectId);
                    return ServiceResult<List<CommentResponseDto>>.Failure("Project not found.");
                }

                    if (project.Comment == null || !project.Comment.Any())
                {
                    _logger.LogInformation("No comments found for project ID: {ProjectId}", dto.ProjectId);
                    return ServiceResult<List<CommentResponseDto>>.Success(new List<CommentResponseDto>(), "No comments found for the specified Project.");
                }

                _logger.LogInformation("Successfully retrieved all comments in project {ProjectTitle}", project.ProjectTitle);
                return ServiceResult<List<CommentResponseDto>>.Success(
                    project.Comment
                        .Select(c => new CommentResponseDto
                        {
                            CommentId = c.Id,
                            CommentContent = c.CommentContent,
                            DateCreated = c.DateCreated,
                            IsViewed = c.IsViewed,
                            UserId = c.UserId,
                            UserName = c.User?.UserName ?? "Unknown",
                            ProjectId = project.Id,
                            ProjectTitle = project.ProjectTitle,
                            WorkFlowId = project.WorkFlowId,
                            WorkFlowName = project.WorkFlows?.WorkFlowName ?? "No work flow name",
                        }).ToList(),
                        $"Project' {project.ProjectTitle}' comments retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project comments for project ID {ProjectTitle}", dto.ProjectId);
                return ServiceResult<List<CommentResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving project comments.",
                    new[] { ex.Message });
            }
        }


        // Get all comments a user made
        public async Task<ServiceResult<List<CommentResponseDto>>> GetUserComments(GetCommentInfoDto dto)
        {
            if (dto == null)
                return ServiceResult<List<CommentResponseDto>>.Failure("Invalid request data.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<List<CommentResponseDto>>.Failure("Invalid User ID.");

            try
            {
                var user = await _context.Users
                    .Include(u => u.Comment!)
                        .ThenInclude(c => c.Project)
                    .Include(u => u.Comment!)
                        .ThenInclude(c => c.WorkFlow)
                    .FirstOrDefaultAsync(wf => wf.Id == dto.UserId);

                // Check if user exists first before accessing comments
                if (user == null)
                {
                    _logger.LogInformation("User not found for user ID: {UserId}", dto.UserId);
                    return ServiceResult<List<CommentResponseDto>>.Failure("User not found.");
                }


                if (user.Comment == null || !user.Comment.Any())
                {
                    _logger.LogInformation("No comments found for user ID: {UserId}", dto.UserId);
                    return ServiceResult<List<CommentResponseDto>>.Success(new List<CommentResponseDto>(), "No comments found for the specified User.");
                }

                _logger.LogInformation("Successfully retrieved all comments for user {UserName}", user.UserName);
                return ServiceResult<List<CommentResponseDto>>.Success(
                    user.Comment
                        .Select(c => new CommentResponseDto
                        {
                            CommentId = c.Id,
                            CommentContent = c.CommentContent,
                            DateCreated = c.DateCreated,
                            IsViewed = c.IsViewed,
                            UserId = c.UserId,
                            UserName = user.UserName ?? "Unknown",
                            ProjectId = c.ProjectId,
                            ProjectTitle = c.Project?.ProjectTitle,
                            WorkFlowId = c.WorkFlowId,
                            WorkFlowName = c.WorkFlow?.WorkFlowName ?? "No work flow name",
                        }).ToList(),
                        $"User'{user.UserName}' comments retrieved successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user comments for user ID {UserId}", dto.UserId);
                return ServiceResult<List<CommentResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving project comments.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services
        public async Task<ServiceResult<CommentResponseDto>> MarkCommentAsViewed(UpdateCommentViewedDto dto)
        {
            if (dto == null)
                return ServiceResult<CommentResponseDto>.Failure("Invalid request data.");

            if (dto.CommentId <= 0)
                return ServiceResult<CommentResponseDto>.Failure("Invalid Comment ID.");

            try
            {
                var comment = await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Project)
                    .Include(c => c.WorkFlow)
                    .FirstOrDefaultAsync(c => c.Id == dto.CommentId);
                if (comment == null)
                {
                    _logger.LogInformation("Comment not found for Comment ID: {CommentId}", dto.CommentId);
                    return ServiceResult<CommentResponseDto>.Failure("Comment not found.");
                }

                if (comment.IsViewed)
                {
                    _logger.LogInformation("Comment ID {CommentId} is already marked as viewed.", dto.CommentId);
                    return ServiceResult<CommentResponseDto>.Success(
                        new CommentResponseDto
                        {
                            CommentId = comment.Id,
                            CommentContent = comment.CommentContent,
                            DateCreated = comment.DateCreated,
                            IsViewed = true,
                            UserId = comment.User.Id,
                            UserName = comment.User.UserName ?? "Unknown",
                            ProjectId = comment.Project?.Id,
                            ProjectTitle = comment.Project?.ProjectTitle,
                            WorkFlowId = comment.WorkFlow.Id,
                            WorkFlowName = comment.WorkFlow.WorkFlowName
                        },
                        "Comment was already marked as viewed."
                    );
                }

                comment.IsViewed = dto.IsViewed;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Comment ID {CommentId} marked as viewed successfully.", comment.Id);

                return ServiceResult<CommentResponseDto>.Success(
                    new CommentResponseDto
                    {
                        CommentId = comment.Id,
                        CommentContent = comment.CommentContent,
                        DateCreated = comment.DateCreated,
                        IsViewed = true,
                        UserId = comment.User.Id,
                        UserName = comment.User.UserName ?? "Unknown",
                        ProjectId = comment.Project?.Id,
                        ProjectTitle = comment.Project?.ProjectTitle,
                        WorkFlowId = comment.WorkFlow.Id,
                        WorkFlowName = comment.WorkFlow.WorkFlowName
                    },
                    $"Comment ID {comment.Id} marked as viewed successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while marking comment as viewed (ID: {CommentId})", dto.CommentId);
                return ServiceResult<CommentResponseDto>.Failure(
                    "A database error occurred while marking comment as viewed.",
                    new[] { ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while marking comment as viewed (ID: {CommentId})", dto.CommentId);
                return ServiceResult<CommentResponseDto>.Failure(
                    "An unexpected error occurred while marking comment as viewed.",
                    new[] { ex.Message }
                );
            }
        }


        // DELETE services
        public async Task<ServiceResult<CommentResponseDto>> DeleteComment(DeleteCommentDto dto)
        {
            if (dto == null)
                return ServiceResult<CommentResponseDto>.Failure("Invalid request data.");

            if (dto.CommentId <= 0)
                return ServiceResult<CommentResponseDto>.Failure("Invalid Comment ID.");

            if (string.IsNullOrWhiteSpace(dto.UserId))
                return ServiceResult<CommentResponseDto>.Failure("Invalid User ID.");

            try
            {
                var comment = await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Project)
                    .Include(c => c.WorkFlow)
                    .FirstOrDefaultAsync(c => c.Id == dto.CommentId && c.UserId == dto.UserId);

                if (comment == null)
                {
                    _logger.LogInformation("Comment not found or user unauthorized for Comment ID: {CommentId}", dto.CommentId);
                    return ServiceResult<CommentResponseDto>.Failure("Comment not found or user unauthorized, user can only delete own comments.");
                }

                var response = new CommentResponseDto
                {
                    CommentId = comment.Id,
                    CommentContent = comment.CommentContent,
                    DateCreated = comment.DateCreated,
                    IsViewed = true,
                    UserId = comment.User.Id,
                    UserName = comment.User.UserName ?? "Unknown",
                    ProjectId = comment.Project?.Id,
                    ProjectTitle = comment.Project?.ProjectTitle,
                    WorkFlowId = comment.WorkFlow.Id,
                    WorkFlowName = comment.WorkFlow.WorkFlowName
                };

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return ServiceResult<CommentResponseDto>.Success(response, "Comment deleted successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting comment ID: {CommentId}", dto.CommentId);
                return ServiceResult<CommentResponseDto>.Failure(
                    "A database error occurred while deleting comment.",
                    new[] { ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting comment ID: {CommentId}", dto.CommentId);
                return ServiceResult<CommentResponseDto>.Failure(
                    "An unexpected error occurred while deleting comment.",
                    new[] { ex.Message }
                );
            }
        }
    }
}
