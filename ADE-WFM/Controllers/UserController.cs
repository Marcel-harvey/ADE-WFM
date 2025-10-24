using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace ADE_WFM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        // CREATE:
        [HttpPost("Create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult<object>.Failure("Invalid data provided.",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));

            var result = await _userService.AddUser(dto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        // GET:
        [HttpGet("All-users")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsers();
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }


        // DELETE:
        [HttpDelete("Delete-user")]
        public async Task<IActionResult> Delete([FromBody] DeleteUserDto dto)
        {
            var result = await _userService.DeleteUser(dto);

            if (!result.Succeeded)
                return BadRequest(new { message = result.Message, errors = result.Errors });

            return Ok(new { message = result.Message, data = result.Data });
        }
    }
}
