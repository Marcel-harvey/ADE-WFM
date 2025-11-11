namespace ADE_WFM.Models.DTOs.CompanyDtos
{
    public class AcceptTenantInviteResponseDto
    {
        public int TenantId { get; set; }
        public string TenantEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Domain { get; set; }
    }
}
