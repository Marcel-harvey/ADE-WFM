using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    public async Task<CreateUserResponseDto> AddUser(CreateUserDto dto)
    {
        // --- Basic validation ---
        if (string.IsNullOrWhiteSpace(dto.Email))
            return new CreateUserResponseDto { Succeeded = false, Errors = new[] { "Email is required." } };

        if (string.IsNullOrWhiteSpace(dto.UserName))
            dto.UserName = dto.Email; // fallback

        if (string.IsNullOrWhiteSpace(dto.Password))
            return new CreateUserResponseDto { Succeeded = false, Errors = new[] { "Password is required." } };

        // --- Check for existing user ---
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
        {
            return new CreateUserResponseDto
            {
                Succeeded = false,
                Errors = new[] { "A user with that email already exists." }
            };
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
            _logger.LogWarning("Failed to create user {Email}: {Errors}",
                dto.Email, string.Join(", ", createResult.Errors.Select(e => e.Description)));

            return new CreateUserResponseDto
            {
                Succeeded = false,
                Errors = createResult.Errors.Select(e => e.Description)
            };
        }

        _logger.LogInformation("User created successfully with ID {UserId}", user.Id);

        return new CreateUserResponseDto
        {
            Succeeded = true,
            UserId = user.Id,
            Errors = Array.Empty<string>()
        };
    }


    // GET:
    // Get all users
    public async Task<List<GetAllUsersResponseDto>> GetAllUsers()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || users.Count == 0)
            {
                _logger.LogInformation("GetAllUsers: no users found.");

                return new List<GetAllUsersResponseDto>
                {
                    new GetAllUsersResponseDto
                    {
                        Message = "No users found."
                    }
                };
            }

            var response = users.Select(u => new GetAllUsersResponseDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                Message = "OK"
            }).ToList();

            _logger.LogInformation("GetAllUsers: retrieved {Count} users.", response.Count);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllUsers: unexpected error retrieving users.");

            return new List<GetAllUsersResponseDto>
            {
                new GetAllUsersResponseDto
                {
                    Message = "An error occurred while retrieving users."
                }
            };
        }
    }



    // UPDATE:

    // DELETE:







}
