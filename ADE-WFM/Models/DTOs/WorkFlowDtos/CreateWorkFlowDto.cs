namespace ADE_WFM.Models.DTOs.WorkFlowDtos
{
    public class CreateWorkFlowDto
    {
        public string WorkFlowName { get; set; } = string.Empty;
        public string CurrentUserId { get; set; } = string.Empty;
        public List<string> UserIds { get; set; } = new();
    }
}
