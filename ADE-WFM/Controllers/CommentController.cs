using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.CommentDtos;
using ADE_WFM.Services.CommentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        // CREATE API's
        // Add comment to selected work flow
        // Added id field as query parameter for easier access and made optional
        // Work flow id can be passed as query parameter or in the body dto
        [HttpPost("WorkFlow")]
        public async Task<IActionResult> AddCommentToWorkFlow([FromBody] AddCommentDto dto, [FromQuery] int? workFlowId = null)
        {
            dto.WorkFlowId = workFlowId ?? dto.WorkFlowId;
            var result = await _commentService.AddCommentToWorkFlow(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Add comment to selected project
        // Added id field as query parameter for easier access and made optional
        // Project flow id can be passed as query parameter or in the body dto
        [HttpPost("Project")]
        public async Task<IActionResult> AddCommentToProject([FromBody] AddCommentDto dto, [FromQuery] int? projectId = null)
        {
            dto.ProjectId = projectId ?? dto.ProjectId;
            var result = await _commentService.AddCommentToProject(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET API's
        // Get all comments in selected work flow
        [HttpGet("WorkFlow/{workFlowId}")]
        public async Task<IActionResult> GetWorkFlowComments(int workFlowId)
        {
            var dto = new GetCommentInfoDto { WorkFlowId = workFlowId };
            var result = await _commentService.GetWorkFlowComments(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // Get all comments in selected project
        [HttpGet("Project/{projectId}")]
        public async Task<IActionResult> GetProjectComments(int projectId)
        {
            var dto = new GetCommentInfoDto { ProjectId = projectId };
            var result = await _commentService.GetProjectComments(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // Get all comments made by a user
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetUserComments(string userId)
        {
            var dto = new GetCommentInfoDto { UserId = userId };
            var result = await _commentService.GetUserComments(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // UPDATE API's
        // Mark comment as viewed/unviewed
        [HttpPut("Update/Is-Viewed")]
        public async Task<IActionResult> MarkCommentAsViewed([FromBody] UpdateCommentViewedDto dto)
        {
            var result = await _commentService.MarkCommentAsViewed(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        // DELETE API's
        // Delete a users comment
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteComment([FromBody] DeleteCommentDto dto)
        {
            var result = await _commentService.DeleteComment(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
