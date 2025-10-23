namespace ADE_WFM.Models.DTOs.UserDtos
{
    public class CreateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;    
        public string? Password { get; set; }
        // For multiple errors using Identity roles
        public IEnumerable<string>? Roles { get; set; } 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
