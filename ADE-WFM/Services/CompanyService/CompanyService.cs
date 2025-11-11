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
    public class CompanyService : ICompanyService
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
        // Create a new tenant
        public async Task<ServiceResult<TenantResponseDto>> CreateTenant(CreateTenantDto dto)
        {
            if (dto == null)
                return ServiceResult<TenantResponseDto>.Failure("No information provided");

            if (string.IsNullOrWhiteSpace(dto.CompanyName))
                return ServiceResult<TenantResponseDto>.Failure("Company name is required");

            try
            {
                var checkTenant = await _context.Tenants
                    .FindAsync(dto.CompanyName);

                if (checkTenant == null)
                    return ServiceResult<TenantResponseDto>.Failure("Company with this name already exists");

                var tenant = new Tenant
                {
                    Name = dto.CompanyName
                };

                if (!string.IsNullOrWhiteSpace(dto.Domain))
                    tenant.Domain = dto.Domain;

                if (!string.IsNullOrWhiteSpace(dto.ConnetctionString))
                    tenant.ConnectionString = dto.ConnetctionString;

                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tenant {TenantName} created successfully {TenantId}", tenant.Name, tenant.Id);

                return ServiceResult<TenantResponseDto>.Success(
                    new TenantResponseDto
                    {
                        TenantId = tenant.Id,
                        CompanyName = tenant.Name,
                        UserName = _tenantContext.UserName
                    },
                    "Company profile created successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occured while trying to add tenant ");
                return ServiceResult<TenantResponseDto>.Failure(
                    "An unexpected error occured while trying to add tenant.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to add tenant ");
                return ServiceResult<TenantResponseDto>.Failure(
                    "An unexpected error occurred while trying to add tenant.",
                    new[] { ex.Message });
            }
        }


        // Create a tenant invite token
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
        public async Task<ServiceResult<AcceptTenantInviteResponseDto>> AcceptTenantInvite(InviteTokenDto dto)
        {
            if (dto == null)
                return ServiceResult<AcceptTenantInviteResponseDto>.Failure("No information provided");

            if (dto.TenantId <= 0 && dto.InviteToken == Guid.Empty)
                return ServiceResult<AcceptTenantInviteResponseDto>.Failure("No valid tenant invite token supplied");

            try
            {
                var tenantInvite = await _context.TenantInvites
                    .FirstOrDefaultAsync(t => t.Id == dto.InviteToken);

                if (tenantInvite == null)
                {
                    _logger.LogWarning("No tenant invite found for token {InviteToken}", dto.InviteToken);
                    return ServiceResult<AcceptTenantInviteResponseDto>.Failure("Invite does not exist");
                }

                if (tenantInvite.ExpiryDate < DateTime.UtcNow)
                {
                    _logger.LogInformation("Token expired for invite {InviteToken}", dto.InviteToken);
                    return ServiceResult<AcceptTenantInviteResponseDto>.Failure("Invite token has expired");
                }

                if (tenantInvite.IsUsed)
                {
                    _logger.LogInformation("Invite token {InviteToken} already used", dto.InviteToken);
                    return ServiceResult<AcceptTenantInviteResponseDto>.Failure("Invite token has already been used");
                }

                var tenant = await _context.Tenants.FindAsync(tenantInvite.TenantId);
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant not found for invite {InviteToken}", dto.InviteToken);
                    return ServiceResult<AcceptTenantInviteResponseDto>.Failure("Associated tenant does not exist");
                }

                tenantInvite.IsUsed = true;
                _context.TenantInvites.Update(tenantInvite);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Invite {InviteToken} accepted successfully for tenant {TenantName}", dto.InviteToken, tenant.Name);

                return ServiceResult<AcceptTenantInviteResponseDto>.Success(
                    new AcceptTenantInviteResponseDto
                    {
                        TenantId = tenant.Id,
                        TenantEmail = tenantInvite.Email,
                        Role = tenantInvite.Role,
                        CompanyName = tenant.Name,
                        Domain = tenant.Domain
                    }, 
                    "Invite accepted successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while accepting tenant invite");
                return ServiceResult<AcceptTenantInviteResponseDto>.Failure(
                    "A database error occurred while accepting the tenant invite.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while accepting tenant invite");
                return ServiceResult<AcceptTenantInviteResponseDto>.Failure(
                    "An unexpected error occurred while accepting the tenant invite.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services
        // Update tenant connection properties
        public async Task<ServiceResult<TenantResponseDto>> UpdateTenantConnection(TenantInfoDto dto)
        {
            if (dto == null)
                return ServiceResult<TenantResponseDto>.Failure("No information provided");

            if (dto.TenantId <= 0)
                return ServiceResult<TenantResponseDto>.Failure("Tenant ID is required");

            try
            {
                var tenant = await _context.Tenants
                    .FindAsync(dto.TenantId);
                if (tenant == null)
                    return ServiceResult<TenantResponseDto>.Failure("No tenant with supplied ID found");

                if (!string.IsNullOrWhiteSpace(dto.CompanyName))
                {
                    _logger.LogInformation("Tenant {OldName} has changed name to {NewName}", tenant.Name, dto.CompanyName);
                    tenant.Name = dto.CompanyName;
                }

                if (!string.IsNullOrWhiteSpace(dto.Domain))
                {
                    bool domainExists = await _context.Tenants
                        .AnyAsync(t => t.Domain == dto.Domain && t.Id != dto.TenantId);
                    if (domainExists)
                        return ServiceResult<TenantResponseDto>.Failure("Domain already in use by another tenant.");

                    _logger.LogInformation("Tenant {TenantName} has updated domain successfully to {Domain}", tenant.Name, dto.Domain);
                    tenant.Domain = dto.Domain;
                }

                await _context.SaveChangesAsync();

                return ServiceResult<TenantResponseDto>.Success(
                    new TenantResponseDto
                    {
                        TenantId = dto.TenantId,
                        CompanyName = tenant.Name,
                        UserName = _tenantContext.UserName
                    },
                    "Tenant updated successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occured while trying to update tenant");
                return ServiceResult<TenantResponseDto>.Failure(
                    "An unexpected error occured while trying to update tenant.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to update tenant");
                return ServiceResult<TenantResponseDto>.Failure(
                    "An unexpected error occurred while trying to update tenant.",
                    new[] { ex.Message });
            }
        }


        // DELETE services
        public async Task<ServiceResult<TenantResponseDto>> DeleteTenant(TenantInfoDto dto)
        {
            if (dto == null)
                return ServiceResult<TenantResponseDto>.Failure("No information provided");

            if (dto.TenantId <= 0)
                return ServiceResult<TenantResponseDto>.Failure("Tenant ID is required");

            try
            {
                var tenant = await _context.Tenants
                    .FindAsync(dto.TenantId);
                if (tenant == null)
                    return ServiceResult<TenantResponseDto>.Failure("No tenant found");

                var response = new TenantResponseDto
                {
                    CompanyName = tenant.Name,
                    UserName = _tenantContext.UserName,
                    TenantId = dto.TenantId,
                };

                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tenant removed succesfully");

                return ServiceResult<TenantResponseDto>.Success(response, "Tenant removed succesfully");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occured while trying to delete tenant");
                return ServiceResult<TenantResponseDto>.Failure(
                    "An unexpected error occured while trying to delete tenant.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to delete tenant");
                return ServiceResult<TenantResponseDto>.Failure(
                    "An unexpected error occurred while trying to delete tenant.",
                    new[] { ex.Message });
            }
        }
    }
}
