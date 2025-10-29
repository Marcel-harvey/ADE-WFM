namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class CreateProjectResponseDto
    {
        public string WorkFlowName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }

        // Lists of User IDs
        public List<ProjectUsersInfoDto> AddedUsers { get; set; } = new();
        public List<ProjectUsersInfoDto> SkippedUsers { get; set; } = new();

    }
}
