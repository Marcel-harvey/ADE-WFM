namespace ADE_WFM.Models.DTOs.WorkFlowDtos
{
    public class ResponseGetWorkFlowsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Lists
        public List<GetWorkFlowProjectsDto>? Projects { get; set; }
        public List<GetWorkFlowUsersDto>? Users { get; set; }

        // Counts
        public int ProjectCount => Projects?.Count ?? 0;
        public int UserCount => Users?.Count ?? 0;
    }
}
