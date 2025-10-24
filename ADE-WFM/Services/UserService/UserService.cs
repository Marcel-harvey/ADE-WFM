using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.UserDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ADE_WFM.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // CREATE:
        public async Task<ServiceResult<CreateUserResponseDto>> AddUser(CreateUserDto dto)
        {
            try
            {
                // --- Basic validation ---
                if (string.IsNullOrWhiteSpace(dto.Email))
                    return ServiceResult<CreateUserResponseDto>.Failure("Email is required.");

                if (string.IsNullOrWhiteSpace(dto.UserName))
                    dto.UserName = dto.Email; // fallback

                if (string.IsNullOrWhiteSpace(dto.Password))
                    return ServiceResult<CreateUserResponseDto>.Failure("Password is required.");

                // --- Check for existing user ---
                var existing = await _userManager.FindByEmailAsync(dto.Email);
                if (existing != null)
                {
                    return ServiceResult<CreateUserResponseDto>.Failure("A user with that email already exists.");
                }

                // --- Build new user object ---
                var user = new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    EmailConfirmed = true
                };

                // Optional mapping for extra properties
                var userType = user.GetType();
                if (!string.IsNullOrEmpty(dto.FirstName) && userType.GetProperty("FirstName") != null)
                    userType.GetProperty("FirstName")!.SetValue(user, dto.FirstName);

                if (!string.IsNullOrEmpty(dto.LastName) && userType.GetProperty("LastName") != null)
                    userType.GetProperty("LastName")!.SetValue(user, dto.LastName);

                // --- Create user ---
                var createResult = await _userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to create user {Email}: {Errors}",
                        dto.Email, string.Join(", ", errors));

                    return ServiceResult<CreateUserResponseDto>.Failure("User creation failed.", errors);
                }

                _logger.LogInformation("User created successfully with ID {UserId}", user.Id);

                return ServiceResult<CreateUserResponseDto>.Success(
                    new CreateUserResponseDto
                    {
                        Succeeded = true,
                        UserId = user.Id,
                        Errors = Array.Empty<string>()
                    },
                    "User created successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating user {Email}", dto.Email);
                return ServiceResult<CreateUserResponseDto>.Failure(
                    "An unexpected error occurred while creating the user.",
                    new[] { ex.Message });
            }
        }

        // GET ALL:
        public async Task<ServiceResult<List<GetAllUsersResponseDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users
                    .ToListAsync();

                if (users == null || users.Count == 0)
                {
                    _logger.LogWarning("No users found in the system.");
                    return ServiceResult<List<GetAllUsersResponseDto>>.Failure("No users found.");
                }

                var response = users.Select(user => new GetAllUsersResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty
                }).ToList();

                _logger.LogInformation("Retrieved {Count} users successfully.", response.Count);

                return ServiceResult<List<GetAllUsersResponseDto>>.Success(response, "Users retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users.");

                return ServiceResult<List<GetAllUsersResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving users.",
                    new[] { ex.Message });
            }
        }

        // DELETEE:
        // Delete user by ID
        public async Task<ServiceResult<DeleteUserResponseDto>> DeleteUser(DeleteUserDto dto)
        {
            try
            {
                // --- Find user by Id ---
                var user = await _userManager
                    .FindByIdAsync(dto.Id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion.", dto.Id);
                    return ServiceResult<DeleteUserResponseDto>.Failure("User not found.");
                }

                // --- Delete the user ---
                var result = await _userManager
                    .DeleteAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to delete user {UserId}: {Errors}", dto.Id, string.Join(", ", errors));
                    return ServiceResult<DeleteUserResponseDto>.Failure("Failed to delete user.", errors);
                }

                // --- Success ---
                _logger.LogInformation("User deleted successfully with ID {UserId}", dto.Id);
                return ServiceResult<DeleteUserResponseDto>.Success(
                    new DeleteUserResponseDto
                    {
                        Name = user.UserName ?? string.Empty
                    },
                    "User deleted successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", dto.Id);
                return ServiceResult<DeleteUserResponseDto>.Failure(
                    "An unexpected error occurred while deleting the user.",
                    new[] { ex.Message });
            }
        }

    }
}
