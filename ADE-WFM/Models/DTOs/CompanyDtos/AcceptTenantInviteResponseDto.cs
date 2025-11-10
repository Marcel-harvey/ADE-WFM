namespace ADE_WFM.Models.DTOs.CompanyDtos
{
    public class AcceptTenantInviteResponseDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string TenantEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
