namespace ADE_WFM.Models.DTOs.WorkFlowDtos {
    public class ProgramResponseDto {
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedUser { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }

        // Lists
        public List<GetProgramProjectsDto>? Projects { get; set; }
        public List<GetProgramUsersDto>? Users { get; set; }
        public List<GetProgramCommentsDto>? Comments { get; set; }

        // Counts
        public int ProjectCount => Projects?.Count ?? 0;
        public int UserCount => Users?.Count ?? 0;
        public int CommentCount => Comments?.Count ?? 0;
    }
}
