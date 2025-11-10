using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.TodoDtos;
using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.TenantService;
using ADE_WFM.Services.JwtService;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ADE_WFM.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;
        private readonly TenantContext _tenantContext;
        private readonly IJwtService _jwtService;

        public UserService(
            IConfiguration config,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserService> logger,
            TenantContext tenantContext,
            IJwtService jwtService)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _tenantContext = tenantContext;
            _jwtService = jwtService;
        }

        // CREATE:
        // Register a new user
        public async Task<ServiceResult<UserResponseDto>> RegisterNewUser(CreateUserDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<UserResponseDto>.Failure("CreateUserDto cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return ServiceResult<UserResponseDto>.Failure("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<UserResponseDto>.Failure("Password is required.");

            if (string.IsNullOrWhiteSpace(dto.TenantName))
                return ServiceResult<UserResponseDto>.Failure("TenantName is required.");

            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                dto.UserName = dto.Email;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if user exists with the same email
                var existingUser = await _userManager
                    .FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    return ServiceResult<UserResponseDto>.Failure("A user with that email already exists.");

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

                var existingTenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Name == dto.TenantName);
                if (existingTenant != null)
                {
                    user.TenantId = existingTenant.Id;
                }
                else
                {
                    var tenant = new Tenant
                    {
                        Name = dto.TenantName,
                        Domain = dto.TenantDomain,
                        ConnectionString = dto.TenantConnectionString
                    };

                    await _context.Tenants.AddAsync(tenant);
                    await _context.SaveChangesAsync();

                    user.TenantId = tenant.Id;
                }

                // --- Create user ---
                var createResult = await _userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to create user {Email}: {Errors}",
                        dto.Email, string.Join(", ", errors));

                    return ServiceResult<UserResponseDto>.Failure("User creation failed.", errors);
                }

                await _userManager.AddToRoleAsync(user, "Admin");

                _logger.LogInformation("User created successfully with ID {UserId} with role 'Admin'", user.Id);

                await transaction.CommitAsync();
                return ServiceResult<UserResponseDto>.Success(
                    new UserResponseDto
                    {
                        UserName = dto.UserName,
                        Email = dto.Email ?? dto.UserName,
                        Id = user.Id
                    },
                    "User created successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Unexpected error occurred while adding user {Email} to database", dto.Email);
                return ServiceResult<UserResponseDto>.Failure(
                    "An unexpected error occurred while creating the user.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Unexpected error occurred while creating user {Email}", dto.Email);
                return ServiceResult<UserResponseDto>.Failure(
                    "An unexpected error occurred while creating the user.",
                    new[] { ex.Message });
            }
        }


        // Login a current user
        public async Task<ServiceResult<LoginResponseDto>> LoginUser(LoginUserDto dto)
        {
            // Basic validation
            if (dto == null)
                return ServiceResult<LoginResponseDto>.Failure("Invalid login request.");

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<LoginResponseDto>.Failure("Email and password are required.");

            try
            {
                // Find user by email
                var user = await _userManager
                    .FindByEmailAsync(dto.Email);
                if (user == null)
                    return ServiceResult<LoginResponseDto>.Failure("Invalid credentials.");

                // Verify password
                var isPasswordValid = await _userManager
                    .CheckPasswordAsync(user, dto.Password);
                if (!isPasswordValid)
                    return ServiceResult<LoginResponseDto>.Failure("Invalid credentials.");

                // Get Tenant info
                var tenant = await _context.Tenants.FindAsync(user.TenantId);
                if (tenant == null)
                    return ServiceResult<LoginResponseDto>.Failure("Tenant not found for user.");

                // Get roles
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "User";

                // Generate JWT
                var token = _jwtService.GenerateToken(user, tenant, userRole);

                // Return response
                return ServiceResult<LoginResponseDto>.Success(
                    new LoginResponseDto
                    {
                        Token = token,
                        Email = user.Email ?? string.Empty,
                        UserId = user.Id,
                        TenantId = tenant.Id,
                        TenantName = tenant.Name,
                        Role = userRole
                    },
                    "Login successful."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during login for user {Email}", dto.Email);
                return ServiceResult<LoginResponseDto>.Failure(
                    "An unexpected error occurred while logging in.",
                    new[] { ex.Message }
                );
            }
        }


        // Create an invite link - invite user to Tenant work flow
        public async Task<ServiceResult<InviteTokenResponseDto>> CreateTenantInvite(InviteTokenDto dto)
        {
            if (dto == null)
                return ServiceResult<InviteTokenResponseDto>.Failure("No information provided");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return ServiceResult<InviteTokenResponseDto>.Failure("Email field required");

            if (string.IsNullOrWhiteSpace(dto.Role))
                return ServiceResult<InviteTokenResponseDto>.Failure("Role field required");

            try
            {
                var inviteToken = new TenantInvite
                {
                    Email = dto.Email,
                    Role = dto.Role,
                    ExpiryDate = DateTime.UtcNow.AddDays(1),
                    IsUsed = false,
                    TenantId = _tenantContext.TenantId
                };

                _context.TenantInvites.Add(inviteToken);
                await _context.SaveChangesAsync();


                _logger.LogInformation("Token {Token} generated for user {UserEmail}", inviteToken.Id, dto.Email);

                return ServiceResult<InviteTokenResponseDto>.Success(
                    new InviteTokenResponseDto
                    {
                        UserId = _tenantContext.UserId,
                        UserName = _tenantContext.UserName,
                        InviteUserEmail = dto.Email,
                        InviteUrl = $"{_config["App:FrontendUrl"]}/invite?token={inviteToken.Id}"
                    }
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Unexpected error occured when creating token");
                return ServiceResult<InviteTokenResponseDto>.Failure(
                    "Unexpected error occured when creating token",
                    new[] { ex.Message });
            }
            // Thrown by JwtService
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected error occured when creating token");
                return ServiceResult<InviteTokenResponseDto>.Failure(
                    "Unexpected error occured when creating token",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured when creating token");
                return ServiceResult<InviteTokenResponseDto>.Failure(
                    "Unexpected error occured when creating token",
                    new[] { ex.Message });
            }

        }


        // GET ALL:
        public async Task<ServiceResult<List<UserResponseDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users
                    .ToListAsync();

                if (users == null || users.Count == 0)
                {
                    _logger.LogWarning("No users found in the system.");
                    return ServiceResult<List<UserResponseDto>>.Failure("No users found.");
                }

                _logger.LogInformation("Retrieved {Count} users successfully.", users.Count);

                return ServiceResult<List<UserResponseDto>>.Success(
                    users.Select(u => new UserResponseDto
                    {
                        Id = u.Id,
                        UserName = u.UserName ?? "Unknown",
                        Email = u.Email ?? "No email"
                    }).ToList(),
                    "Users retrieved successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users.");

                return ServiceResult<List<UserResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving users.",
                    new[] { ex.Message });
            }
        }


        // UPDATE:
        // Change user password
        public async Task<ServiceResult<LoginResponseDto>> ChangePassword(ChangePasswordDto dto)
        {
            if (dto == null)
                return ServiceResult<LoginResponseDto>.Failure("No information provided");

            if (dto.OldPassword == dto.NewPassword)
                return ServiceResult<LoginResponseDto>.Failure("Old and new password can not be identical");

            try
            {
                var user = await _userManager
                    .FindByIdAsync(_tenantContext.UserId);
                if (user == null)
                    return ServiceResult<LoginResponseDto>.Failure("User does not exist");

                // Verify password
                var isPasswordValid = await _userManager
                    .CheckPasswordAsync(user, dto.OldPassword);
                if (!isPasswordValid)
                    return ServiceResult<LoginResponseDto>.Failure("Old password is incorrect.");

                var passwordResult = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    _logger.LogWarning("Could not update password for User {UserId} - {UserName}", _tenantContext.UserId, _tenantContext.UserName);
                    return ServiceResult<LoginResponseDto>.Failure("An error occured when trying to update your password");
                }

                // Get Tenant info
                var tenant = await _context.Tenants.FindAsync(user.TenantId);
                if (tenant == null)
                    return ServiceResult<LoginResponseDto>.Failure("Tenant not found for user.");

                // Get roles
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "User";

                // Generate JWT
                var token = _jwtService.GenerateToken(user, tenant, userRole);

                _logger.LogInformation("User password updated successfully for user {UserId} - {UserName}", _tenantContext.UserId, _tenantContext.UserName);

                return ServiceResult<LoginResponseDto>.Success(
                    new LoginResponseDto
                    {
                        Token = token,
                        Email = user.Email ?? string.Empty,
                        UserId = user.Id,
                        TenantId = tenant.Id,
                        TenantName = tenant.Name,
                        Role = userRole
                    },
                    "Login successful."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occured when trying to update your password");
                return ServiceResult<LoginResponseDto>.Failure(
                    "An unexpected error occurred while creating the user.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured when trying to update your password");
                return ServiceResult<LoginResponseDto>.Failure(
                    "An unexpected error occurred while creating the user.",
                    new[] { ex.Message });
            }
        }


        // DELETEE:
        // Delete user by ID
        public async Task<ServiceResult<UserResponseDto>> DeleteUser(DeleteUserDto dto)
        {
            // General validation
            if (dto == null)
                return ServiceResult<UserResponseDto>.Failure("DeleteUserDto cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.Id))
                return ServiceResult<UserResponseDto>.Failure("User ID is required for deletion.");

            try
            {
                var user = await _userManager
                    .FindByIdAsync(dto.Id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion.", dto.Id);
                    return ServiceResult<UserResponseDto>.Failure("User not found.");
                }

                var respone = new UserResponseDto
                {
                    UserName = user.UserName ?? "Unknown",
                    Email = user.Email ?? "No Email",
                    Id = user.Id
                };

                var result = await _userManager
                    .DeleteAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to delete user {UserId}: {Errors}", dto.Id, string.Join(", ", errors));
                    return ServiceResult<UserResponseDto>.Failure("Failed to delete user.", errors);
                }

                _logger.LogInformation("User deleted successfully with ID {UserId}", dto.Id);

                return ServiceResult<UserResponseDto>.Success(respone, "User deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", dto.Id);
                return ServiceResult<UserResponseDto>.Failure(
                    "An unexpected error occurred while deleting the user.",
                    new[] { ex.Message });
            }
        }

    }
}
