using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.CompanyDtos;
using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.JwtService;
using ADE_WFM.Services.TenantService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.CompanyService
{
    public class CompanyService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CompanyService> _logger;
        private readonly TenantContext _tenantContext;
        private readonly IJwtService _jwtService;
        public CompanyService(
            IConfiguration config,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<CompanyService> logger,
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

        // CREATE services
        public async Task<ServiceResult<InviteToTenantResponseDto>> CreateTenantInvite(InviteToTenantDto dto)
        {
            if (dto == null)
                return ServiceResult<InviteToTenantResponseDto>.Failure("No information provided");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return ServiceResult<InviteToTenantResponseDto>.Failure("Email field required");

            if (string.IsNullOrWhiteSpace(dto.Role))
                return ServiceResult<InviteToTenantResponseDto>.Failure("Role field required");

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

                return ServiceResult<InviteToTenantResponseDto>.Success(
                    new InviteToTenantResponseDto
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
                return ServiceResult<InviteToTenantResponseDto>.Failure(
                    "Unexpected error occured when creating token",
                    new[] { ex.Message });
            }
            // Thrown by JwtService
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected error occured when creating token");
                return ServiceResult<InviteToTenantResponseDto>.Failure(
                    "Unexpected error occured when creating token",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured when creating token");
                return ServiceResult<InviteToTenantResponseDto>.Failure(
                    "Unexpected error occured when creating token",
                    new[] { ex.Message });
            }
        }


        // GET services
        public async Task<ServiceResult<AcceptTenantInviteResponseDto>> AcceptTenantInvite(TenantInfoDto dto)
        {
            if (dto == null)
                return ServiceResult<AcceptTenantInviteResponseDto>.Failure("No information provided");

            if (string.IsNullOrWhiteSpace(dto.TenantId))
                return ServiceResult<AcceptTenantInviteResponseDto>.Failure("No tenant id supplied");

            try
            {
                var tenantToken = await _context.TenantInvites
                    .FindAsync(dto.TenantId);

                if (tenantToken == null)
                {
                    _logger.LogInformation("No tenant invite found for ID {TenantId}", dto.TenantId);
                    return ServiceResult<AcceptTenantInviteResponseDto>.Failure("Invite does not exist");
                }

                if (DateTime.UtcNow <= tenantToken.ExpiryDate)
                {
                    _logger.LogInformation("Token expired");
                    return ServiceResult<AcceptTenantInviteResponseDto>.Failure("Token expired");
                }

                if (tenantToken.IsUsed)
                {
                    _logger.LogInformation("Token was already used");
                    return ServiceResult<AcceptTenantInviteResponseDto>.Failure("Token was already used");
                }

                return ServiceResult<AcceptTenantInviteResponseDto>.Success(
                    new AcceptTenantInviteResponseDto
                    {
                        TenantId = dto.TenantId,
                        TenantEmail = tenantToken.Email,
                        Role = tenantToken.Role
                    },
                    "Invite accepted"
                );

            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occured while trying to get tenant invite");
                return ServiceResult<AcceptTenantInviteResponseDto>.Failure(
                    "An unexpected error occured while trying to get tenant invite.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to get tenant invite");
                return ServiceResult<AcceptTenantInviteResponseDto>.Failure(
                    "An unexpected error occurred while trying to get tenant invite.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services


        // DELETE services
    }
}
