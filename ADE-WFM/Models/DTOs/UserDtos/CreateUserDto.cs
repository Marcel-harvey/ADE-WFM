namespace ADE_WFM.Models.DTOs.UserDtos
{
    public class CreateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? TenantToken { get; set; }
        //public string TenantName { get; set; } = string.Empty;
        //public string? TenantDomain { get; set; }
        //public string? TenantConnectionString { get; set; }

        public IEnumerable<string>? Roles { get; set; } 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
