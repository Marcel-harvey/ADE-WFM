namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class UpdateProjectInfoResponseDto
    {
        public string ProjectTitle { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}
