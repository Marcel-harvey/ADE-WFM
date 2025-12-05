namespace ADE_WFM.Models.DTOs.ProjectDtos {
    public class UpdateProjectInfoDto {
        public int ProjectId { get; set; }
        public string? projectTitle { get; set; }
        public string? Description { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}
