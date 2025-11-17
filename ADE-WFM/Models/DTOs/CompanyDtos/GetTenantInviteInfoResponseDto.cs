namespace ADE_WFM.Models.DTOs.CompanyDtos {
    public class GetTenantInviteInfoResponseDto {
        public Guid TenantToken { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}
