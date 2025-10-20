namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class UpdateProjectTitleDto
    {
        public int projectId { get; set; }
        public string newProjectTitle { get; set; } = string.Empty;
    }
}
