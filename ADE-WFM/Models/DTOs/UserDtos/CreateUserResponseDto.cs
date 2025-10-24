namespace ADE_WFM.Models.DTOs.UserDtos
{
    public class CreateUserResponseDto
    {
        public bool Succeeded { get; set; }
        public string? UserId { get; set; }
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
    }
}
