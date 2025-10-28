namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class AddUserToProjectDto
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
