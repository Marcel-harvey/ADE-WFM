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


        // UPDATE API's


        // DELETE API's
    }
}
