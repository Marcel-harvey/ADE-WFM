namespace ADE_WFM.Models.DTOs.WorkFlowDtos
{
    public class GetAllWorkFlowsDto
    {
    }

    public class ResponseGetAllWorkFlowsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Lists
        public List<GetWorkFlowProjects>? Projects { get; set; }
        public List<GetWorkFlowUsers>? Users { get; set; }

        // Counts
        public int ProjectCount => Projects?.Count ?? 0;
        public int UserCount => Users?.Count ?? 0;
    }

    public class GetWorkFlowProjects
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }

    public class GetWorkFlowUsers
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
