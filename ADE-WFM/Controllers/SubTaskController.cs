using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.SubTaskDtos;
using ADE_WFM.Services.SubTaskService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTaskController : ControllerBase
    {
        private readonly ISubTaskService _subTaskService;
        public SubTaskController(ISubTaskService subTaskService)
        {
            _subTaskService = subTaskService;
        }

        // CREATE API's
        // Create new subtask and add to a todo
        [HttpPost("Todo/Add")]
        public async Task<IActionResult> AddSubTaskToTodo([FromBody] AddSubTasksToTodoDto dto)
        {
            var result = await _subTaskService.AddSubTasksToTodo(dto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        // GET API's


        // UPDATE API's


        // DELETE API's
    }
}
