namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class CreateProjectDto
    {
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjectDescription { get; set; }
        public DateOnly DueDate { get; set; }
        public int WorkFlowId { get; set; }
        public string CurrentUserId { get; set; } = string.Empty;
        public List<string> UserIds { get; set; } = new();
    }
}
