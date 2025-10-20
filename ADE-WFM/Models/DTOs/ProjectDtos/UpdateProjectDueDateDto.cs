namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class UpdateProjectDueDateDto
    {
        public int ProjectId { get; set; }
        public DateOnly NewDueDate { get; set; }
    }
}
