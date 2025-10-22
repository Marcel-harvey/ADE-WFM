namespace ADE_WFM.Models.DTOs.WorkFlowDtos
{
    public class GetAllWorkFlowsDto
    {
    }

    public class ResponseGetAllWorkFlowsDto
    {
        public int Id { get; set; }
        public string WorkFlowName { get; set; } = string.Empty;

        public List<GetWorkFlowProjects>? Projects { get; set; }
        public List<GetWorkFlowUsers>? Users { get; set; }
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
