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
                return StatusCode(500, new CreateUserResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "An unexpected error occurred while creating the user." }
                });
            }
        }


        // GET:
        // Get all users
        [HttpGet("All-users")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsers();

            // service returns an error as a single-item list with Message populated
            if (result == null)
            {
                _logger.LogError("GetAll: service returned null.");
                return StatusCode(500, new { message = "Unexpected error." });
            }

            // if single item with non-empty Message and no Id/email, treat as error/info
            if (result.Count == 1 &&
                string.IsNullOrWhiteSpace(result[0].Id) &&
                string.IsNullOrWhiteSpace(result[0].Email) &&
                !string.IsNullOrWhiteSpace(result[0].Message) &&
                result[0].Message != "OK")
            {
                // decide status code based on message — usually 404 or 500
                if (result[0].Message.Contains("No users found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result);

                return StatusCode(500, result);
            }

            return Ok(result);
        }






    }
}
