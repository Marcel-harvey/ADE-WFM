using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.TodoDtos;
using ADE_WFM.Services.TodoService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }


        // CREATE API's
        // Add a new todo
        [HttpPost("{userId}")]
        public async Task<IActionResult> AddTodo([FromBody] AddTodoDto dto, string userId, [FromQuery] int? projectId = null)
        {
            dto.UserId = userId ?? dto.UserId;
            dto.ProjectId = projectId ?? dto.ProjectId;
            var result = await _todoService.AddTodo(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET API's
        // Get all todos for a user
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetAllUserTodos([FromBody] GetToDoDto dto, string userId)
        {
            dto.UserId = userId ?? dto.UserId;
            var result = await _todoService.GetAllUserTodos(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Get all todos for a project
        [HttpGet("Project/{projectId}")]
        public async Task<IActionResult> GetAllProjectTodos([FromBody] GetToDoDto dto, int? projectId)
        {
            dto.ProjectId = projectId ?? dto.ProjectId;
            var result = await _todoService.GetAllProjectTodos(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // UPDATE API's
        // Update a todo
        [HttpPut]
        public async Task<IActionResult> UpdateTodo([FromBody] UpdateTodoDto dto, [FromQuery] int? todoId = null)
        {
            dto.TodoId = todoId ?? dto.TodoId;
            var result = await _todoService.UpdateTodo(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Mark a todo as complete/incomplete
        [HttpPut("is-complete")]
        public async Task<IActionResult> MarkTodoCompletion([FromBody] MarkTodoCompletionDto dto, [FromQuery] int? todoId = null, [FromQuery] bool? isComplete = true)
        {
            dto.ToDoId = todoId ?? dto.ToDoId;
            dto.IsComplete = isComplete ?? dto.IsComplete;
            var result = await _todoService.MarkTodoCompletion(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE API's
        // Delete a todo
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteTodo([FromBody] GetToDoDto dto)
        {
            var result = await _todoService.DeleteTodo(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
