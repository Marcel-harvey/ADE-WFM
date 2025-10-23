namespace ADE_WFM.Models.DTOs.UserDtos
{
    public class ResponseCreateUserDto
    {
        public bool Succeeded { get; set; }
        public string? UserId { get; set; }
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
        public string? TemporaryPassword { get; set; }
    }
}
