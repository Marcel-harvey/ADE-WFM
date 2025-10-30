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
        [HttpPost("Create")]
        public async Task<IActionResult> AddTodo([FromBody] AddTodoDto dto)
        {
            var result = await _todoService.AddTodo(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        // GET API's
        // Get all todos for a user
        [HttpPost("User/Get/All")]
        public async Task<IActionResult> GetAllUserTodos([FromBody] GetToDoDto dto)
        {
            var result = await _todoService.GetAllUserTodos(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        // Get all todos for a project
        [HttpPost("Project/Get/All")]
        public async Task<IActionResult> GetAllProjectTodos([FromBody] GetToDoDto dto)
        {
            var result = await _todoService.GetAllProjectTodos(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        // UPDATE API's
        // Update a todo
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateTodo([FromBody] UpdateTodoDto dto)
        {
            var result = await _todoService.UpdateTodo(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
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
