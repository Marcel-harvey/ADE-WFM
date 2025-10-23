using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.UserService;
using Microsoft.AspNetCore.Identity;
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

    public async Task<ResponseCreateUserDto> AddUser(CreateUserDto dto)
    {
        // --- Basic validation ---
        if (string.IsNullOrWhiteSpace(dto.Email))
            return new ResponseCreateUserDto { Succeeded = false, Errors = new[] { "Email is required." } };

        if (string.IsNullOrWhiteSpace(dto.UserName))
            dto.UserName = dto.Email; // fallback

        if (string.IsNullOrWhiteSpace(dto.Password))
            return new ResponseCreateUserDto { Succeeded = false, Errors = new[] { "Password is required." } };

        // --- Check for existing user ---
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
        {
            return new ResponseCreateUserDto
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

            return new ResponseCreateUserDto
            {
                Succeeded = false,
                Errors = createResult.Errors.Select(e => e.Description)
            };
        }

        _logger.LogInformation("User created successfully with ID {UserId}", user.Id);

        return new ResponseCreateUserDto
        {
            Succeeded = true,
            UserId = user.Id,
            Errors = Array.Empty<string>()
        };
    }
}
