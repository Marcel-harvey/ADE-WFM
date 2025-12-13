using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace ADE_WFM.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase {
        private readonly IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }


        // CREATE:
        // Register new user
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto dto) {
            var result = await _userService.RegisterNewUser(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET:
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var result = await _userService.GetAllUsers();

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Get users in tenant
        [HttpGet("Company")]
        public async Task<IActionResult> GetCompanyUsers() {
            var result = await _userService.GetTenantUsers();

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Get users in program
        [HttpGet("Program/{programId}")]
        public async Task<IActionResult> GetProgramUsers(int programId) {
            var dto = new GetInfoForUsersListDto {
                ProgramId = programId
            };

            var result = await _userService.GetProgramUsers(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // Get users in project
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectUsers(int projectId) {
            var dto = new GetInfoForUsersListDto {
                ProjectId = projectId
            };

            var result = await _userService.GetProjectUsers(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Login User
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto) {
            var result = await _userService.LoginUser(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // UPDATE:
        // Update user password
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto) {
            var result = await _userService.ChangePassword(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE:
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteUserDto dto) {
            var result = await _userService.DeleteUser(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
