namespace ADE_WFM.Models.DTOs.UserDtos {
    public class CreateUserDto {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? TenantId { get; set; }
        public Guid? TenantToken { get; set; }
        //public string TenantName { get; set; } = string.Empty;
        //public string? TenantDomain { get; set; }
        //public string? TenantConnectionString { get; set; }

        public string Role { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
