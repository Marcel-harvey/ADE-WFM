namespace ADE_WFM.Models.DTOs.ProjectDtos {
    public class UpdateProjectUserResponseDto {
        public int? ProjectId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }
}
