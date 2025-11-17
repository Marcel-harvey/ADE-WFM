using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.CompanyDtos;

namespace ADE_WFM.Services.CompanyService {
    public interface ICompanyService {
        // CREATE
        Task<ServiceResult<InviteToTenantResponseDto>> CreateTenantInvite(InviteToTenantDto dto);
        Task<ServiceResult<TenantResponseDto>> CreateTenant(CreateTenantDto dto);
        Task<ServiceResult<AcceptTenantInviteResponseDto>> AcceptTenantInvite(InviteTokenDto dto);

        // GET
        Task<ServiceResult<GetTenantInviteInfoResponseDto>> GetTenantInviteInfo(InviteTokenDto dto);

        // UPDATE
        Task<ServiceResult<TenantResponseDto>> UpdateTenantConnection(TenantInfoDto dto);

        // DELETE
        Task<ServiceResult<TenantResponseDto>> DeleteTenant(TenantInfoDto dto);
    }
}
