using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.JwtService;
using ADE_WFM.Services.TenantService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.UserService {
    public class UserService : IUserService {
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
            IJwtService jwtService) {
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
        public async Task<ServiceResult<UserResponseDto>> RegisterNewUser(CreateUserDto dto) {
            if (dto == null)
                return ServiceResult<UserResponseDto>.Failure("CreateUserDto cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return ServiceResult<UserResponseDto>.Failure("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<UserResponseDto>.Failure("Password is required.");

            if (string.IsNullOrWhiteSpace(dto.UserName))
                dto.UserName = dto.Email;

            try {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    return ServiceResult<UserResponseDto>.Failure("A user with that email already exists.");

                var user = new ApplicationUser {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    EmailConfirmed = true
                };

                TenantInvite? tenantToken = null;

                if (dto.TenantToken.HasValue && dto.TenantToken.Value != Guid.Empty) {
                    tenantToken = await _context.TenantInvites
                        .FirstOrDefaultAsync(t => t.Id == dto.TenantToken.Value);

                    if (tenantToken != null) {
                        if (tenantToken.ExpiryDate < DateTime.UtcNow)
                            return ServiceResult<UserResponseDto>.Failure("Invite token has expired.");

                        if (tenantToken.IsUsed)
                            return ServiceResult<UserResponseDto>.Failure("Invite token has already been used.");

                        user.TenantId = tenantToken.TenantId;
                    }
                }
                else {
                    user.TenantId = dto.TenantId;
                }

                var userType = user.GetType();
                if (!string.IsNullOrEmpty(dto.FirstName) && userType.GetProperty("FirstName") != null)
                    userType.GetProperty("FirstName")!.SetValue(user, dto.FirstName);

                if (!string.IsNullOrEmpty(dto.LastName) && userType.GetProperty("LastName") != null)
                    userType.GetProperty("LastName")!.SetValue(user, dto.LastName);

                // Create user
                var createResult = await _userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded) {
                    var errors = createResult.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to create user {Email}: {Errors}", dto.Email, string.Join(", ", errors));
                    return ServiceResult<UserResponseDto>.Failure("User creation failed.", errors);
                }

                // Assign roles 
                if (tenantToken != null) {
                    await _userManager.AddToRoleAsync(user, tenantToken.Role);
                }
                else if (!string.IsNullOrEmpty(dto.Role)) {
                    await _userManager.AddToRoleAsync(user, dto.Role);
                }

                if (tenantToken != null) {
                    tenantToken.IsUsed = true;
                    _context.TenantInvites.Update(tenantToken);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("User created successfully with ID {UserId}", user.Id);

                return ServiceResult<UserResponseDto>.Success(
                    new UserResponseDto {
                        UserName = dto.UserName,
                        Email = dto.Email,
                        Id = user.Id
                    },
                    "User created successfully."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while creating user {Email}", dto.Email);
                return ServiceResult<UserResponseDto>.Failure("Database error while creating user.", new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occurred while creating user {Email}", dto.Email);
                return ServiceResult<UserResponseDto>.Failure("Unexpected error occurred while creating user.", new[] { ex.Message });
            }
        }


        // Login a current user
        public async Task<ServiceResult<LoginResponseDto>> LoginUser(LoginUserDto dto) {
            // Basic validation
            if (dto == null)
                return ServiceResult<LoginResponseDto>.Failure("Invalid login request.");

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<LoginResponseDto>.Failure("Email and password are required.");

            try {
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
                    new LoginResponseDto {
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
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error occurred during login for user {Email}", dto.Email);
                return ServiceResult<LoginResponseDto>.Failure(
                    "An unexpected error occurred while logging in.",
                    new[] { ex.Message }
                );
            }
        }


        // GET ALL:
        public async Task<ServiceResult<List<UserResponseDto>>> GetAllUsers() {
            try {
                var users = await _userManager.Users
                    .ToListAsync();

                if (users == null || users.Count == 0) {
                    _logger.LogWarning("No users found in the system.");
                    return ServiceResult<List<UserResponseDto>>.Failure("No users found.");
                }

                _logger.LogInformation("Retrieved {Count} users successfully.", users.Count);

                return ServiceResult<List<UserResponseDto>>.Success(
                    users.Select(u => new UserResponseDto {
                        Id = u.Id,
                        UserName = u.UserName ?? "Unknown",
                        Email = u.Email ?? "No email"
                    }).ToList(),
                    "Users retrieved successfully."
                );
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving users.");

                return ServiceResult<List<UserResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving users.",
                    new[] { ex.Message });
            }
        }


        // UPDATE:
        // Change user password
        public async Task<ServiceResult<LoginResponseDto>> ChangePassword(ChangePasswordDto dto) {
            if (dto == null)
                return ServiceResult<LoginResponseDto>.Failure("No information provided");

            if (dto.OldPassword == dto.NewPassword)
                return ServiceResult<LoginResponseDto>.Failure("Old and new password can not be identical");

            try {
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
                if (!passwordResult.Succeeded) {
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
                    new LoginResponseDto {
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
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "An error occured when trying to update your password");
                return ServiceResult<LoginResponseDto>.Failure(
                    "An unexpected error occurred while creating the user.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occured when trying to update your password");
                return ServiceResult<LoginResponseDto>.Failure(
                    "An unexpected error occurred while creating the user.",
                    new[] { ex.Message });
            }
        }


        // DELETEE:
        // Delete user by ID
        public async Task<ServiceResult<UserResponseDto>> DeleteUser(DeleteUserDto dto) {
            // General validation
            if (dto == null)
                return ServiceResult<UserResponseDto>.Failure("DeleteUserDto cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.Id))
                return ServiceResult<UserResponseDto>.Failure("User ID is required for deletion.");

            try {
                var user = await _userManager
                    .FindByIdAsync(dto.Id);

                if (user == null) {
                    _logger.LogWarning("User with ID {UserId} not found for deletion.", dto.Id);
                    return ServiceResult<UserResponseDto>.Failure("User not found.");
                }

                var respone = new UserResponseDto {
                    UserName = user.UserName ?? "Unknown",
                    Email = user.Email ?? "No Email",
                    Id = user.Id
                };

                var result = await _userManager
                    .DeleteAsync(user);

                if (!result.Succeeded) {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to delete user {UserId}: {Errors}", dto.Id, string.Join(", ", errors));
                    return ServiceResult<UserResponseDto>.Failure("Failed to delete user.", errors);
                }

                _logger.LogInformation("User deleted successfully with ID {UserId}", dto.Id);

                return ServiceResult<UserResponseDto>.Success(respone, "User deleted successfully.");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", dto.Id);
                return ServiceResult<UserResponseDto>.Failure(
                    "An unexpected error occurred while deleting the user.",
                    new[] { ex.Message });
            }
        }

    }
}
