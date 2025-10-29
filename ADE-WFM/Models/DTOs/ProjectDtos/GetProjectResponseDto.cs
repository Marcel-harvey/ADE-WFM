using ADE_WFM.Models.DTOs.WorkFlowDtos;

namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class GetProjectResponseDto
    {
        public string WorkFlowName { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }

        // Lists
        public List<ProjectUsersInfoDto>? Users { get; set; }

        // Counts
        public int UserCount => Users?.Count ?? 0;
    }
}
