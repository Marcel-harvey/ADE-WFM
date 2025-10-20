namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class UpdateProjectDescriptionDto
    {
        public int projectId { get; set; }
        public string newProjectDescription { get; set; } = string.Empty;
    }
}
