namespace ADE_WFM.Models.DTOs.UserDtos {
    public class RegisterUserResponseDto {
        public string TenantId { get; set; } = string.Empty;
        public string TenantEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
