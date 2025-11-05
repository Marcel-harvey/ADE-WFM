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
        [HttpPost("Todo")]
        public async Task<IActionResult> AddSubTaskToTodo([FromBody] AddSubTasksToTodoDto dto, [FromQuery] int? todoId = null)
        {
            dto.TodoId = todoId ?? dto.TodoId;
            var result = await _subTaskService.AddSubTasksToTodo(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET API's
        // Get all subtasks for a specific todo
        [HttpGet("Todo")]
        public async Task<IActionResult> GetTodoSubTasks([FromQuery] GetSubTasksDto dto)
        {
            var result = await _subTaskService.GetTodoSubTasks(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // UPDATE API's
        // Update a subtask description
        [HttpPut("Description")]
        public async Task<IActionResult> UpdateSubTaskDescription([FromBody] UpdateSubTaskDto dto, [FromQuery] int? todoId = null, [FromQuery] int? subTaskIs = null)
        {
            dto.TodoId = todoId ?? dto.TodoId;
            dto.SubTaskId = subTaskIs ?? dto.SubTaskId;
            var result = await _subTaskService.UpdateSubTask(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Mark a subtask as completed or not completed
        [HttpPut("is-complete")]
        public async Task<IActionResult> MarkSubTaskCompletion([FromQuery] MarkSubTaskCompletionDto dto)
        {
            var result = await _subTaskService.MarkSubTaskCompletion(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE API's
        // Delete a subtask
        [HttpDelete()]
        public async Task<IActionResult> DeleteSubTask([FromQuery] GetSubTasksDto dto)
        {
            var result = await _subTaskService.DeleteSubTask(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
