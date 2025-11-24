namespace ADE_WFM.Models.DTOs.WorkFlowDtos {
    public class WorkFlowResponseDto {
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }

        // Lists
        public List<GetWorkFlowProjectsDto>? Projects { get; set; }
        public List<GetWorkFlowUsersDto>? Users { get; set; }
        public List<GetWorkFlowCommentsDto>? Comments { get; set; }

        // Counts
        public int ProjectCount => Projects?.Count ?? 0;
        public int UserCount => Users?.Count ?? 0;
        public int CommentCount => Comments?.Count ?? 0;
    }
}
