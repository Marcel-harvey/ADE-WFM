using ADE_WFM.Models.DTOs;
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
        // Create a new user
        [HttpPost("Create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult<object>.Failure("Invalid data provided.",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));

            try
            {
                var result = await _userService.AddUser(dto);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("User creation failed for {Email}: {Errors}",
                        dto.Email, string.Join(", ", result.Errors));
                    return BadRequest(result);
                }

                _logger.LogInformation("User created successfully with ID {UserId}", result.Data?.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email {Email}", dto.Email);
                return StatusCode(500, ServiceResult<object>.Failure(
                    "An unexpected error occurred while creating the user.",
                    new[] { ex.Message }));
            }
        }


        // GET:
        /// Get all users
        [HttpGet("All-users")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _userService.GetAllUsers();

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to retrieve users: {Message}", result.Message);
                    return NotFound(result);
                }

                _logger.LogInformation("Retrieved {Count} users successfully", result.Data?.Count() ?? 0);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, ServiceResult<object>.Failure(
                    "An unexpected error occurred while retrieving users.",
                    new[] { ex.Message }));
            }
        }


        // DELETE: 
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteUserDto dto)
        {
            var result = await _userService.DeleteUser(dto);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed to delete user: {Message}", result.Message);
                return BadRequest(new
                {
                    message = result.Message,
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }
    }
}
