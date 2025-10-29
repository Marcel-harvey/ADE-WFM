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
        [HttpPost("WorkFlow/Add")]
        public async Task<IActionResult> AddCommentToWorkFlow([FromBody] AddCommentDto dto)
        {
            var result = await _commentService.AddCommentToWorkFlow(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        // GET API's
        // Get all comments in selected work flow
        [HttpGet("WorkFlow/Get/{id}")]
        public async Task<IActionResult> GetWorkFlowComments(int id)
        {
            var dto = new GetCommentsInSectionDto { Id = id };
            var result = await _commentService.GetWorkFlowComments(dto);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }


        // Get all comments in selected project
        [HttpGet("Project/Get/{id}")]
        public async Task<IActionResult> GetProjectComments(int id)
        {
            var dto = new GetCommentsInSectionDto { Id = id };
            var result = await _commentService.GetProjectComments(dto);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }


        // Get all comments made by a user
        [HttpGet("User/Get/{userId}")]
        public async Task<IActionResult> GetUserComments(string userId)
        {
            var dto = new GetUserCommentsDto { UserId = userId };
            var result = await _commentService.GetUserComments(dto);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }


        // UPDATE API's
        // Mark comment as viewed/unviewed
        [HttpPut("Update/Viewed")]
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
