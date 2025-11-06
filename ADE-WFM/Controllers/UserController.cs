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
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto dto)
        {
            var result = await _userService.RegisterNewUser(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Register new user



        // GET:
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsers();

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE:
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteUserDto dto)
        {
            var result = await _userService.DeleteUser(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
