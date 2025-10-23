using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ADE_WFM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        // CREATE:
        /// Create a new user
        [HttpPost("Create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _userService.AddUser(dto);

                if (!response.Succeeded)
                {
                    _logger.LogWarning("User creation failed for {Email}: {Errors}", dto.Email, string.Join(", ", response.Errors));
                    return BadRequest(response);
                }

                _logger.LogInformation("User created successfully with ID {UserId}", response.UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email {Email}", dto.Email);
                return StatusCode(500, new ResponseCreateUserDto
                {
                    Succeeded = false,
                    Errors = new[] { "An unexpected error occurred while creating the user." }
                });
            }
        }






    }
}
