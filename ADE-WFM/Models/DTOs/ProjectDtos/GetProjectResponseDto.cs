using ADE_WFM.Models.DTOs.WorkFlowDtos;

namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class GetProjectResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }

        // Lists
        public List<ProjectUsersDto>? Users { get; set; }

        // Counts
        public int UserCount => Users?.Count ?? 0;
    }
}
