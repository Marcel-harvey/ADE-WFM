namespace ADE_WFM.Models.DTOs.WorkFlowDtos {
    public class WorkFlowResponseDto {
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; } = string.Empty;
        public string createdUser { get; set; } = string.Empty;
        public DateTime dateCreated { get; set; }

        // Lists
        public List<GetWorkFlowProjectsDto>? Projects { get; set; }
        public List<GetWorkFlowUsersDto>? Users { get; set; }

        // Counts
        public int ProjectCount => Projects?.Count ?? 0;
        public int UserCount => Users?.Count ?? 0;
    }
}
