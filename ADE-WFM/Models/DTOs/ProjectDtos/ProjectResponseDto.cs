namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class ProjectResponseDto
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjectDescription { get; set; }
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; } = string.Empty;


        // Lists
        public List<ProjectUsersInfoDto>? Users { get; set; }
        public List<ProjectUsersInfoDto>? SkippedUsers { get; set; }
        public List<ProjectCommentsInfoDto>? Comments { get; set; }
        public List<ProjectTodosInfoDto>? Todos { get; set; }

        // Counts
        public int UserCount => Users?.Count ?? 0;
        public int CommentCount => Comments?.Count ?? 0;
        public int TodoCount => Todos?.Count ?? 0;

    }
}
