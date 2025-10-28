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

        // GET API's
        // Get all comments in selected work flow
        [HttpGet("Get-workflow-comments/{id}")]
        public async Task<IActionResult> GetWorkFlowComments(int id)
        {
            var dto = new GetCommentsInSectionDto { WorkFlowId = id };
            var result = await _commentService.GetWorkFlowComments(dto);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        // UPDATE API's

        // DELETE API's
    }
}
