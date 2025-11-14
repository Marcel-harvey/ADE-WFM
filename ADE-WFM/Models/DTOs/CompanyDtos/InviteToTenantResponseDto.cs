namespace ADE_WFM.Models.DTOs.CompanyDtos {
    public class InviteToTenantResponseDto {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string InviteUserEmail { get; set; } = string.Empty;
        public string InviteUrl { get; set; } = string.Empty;
    }
}
