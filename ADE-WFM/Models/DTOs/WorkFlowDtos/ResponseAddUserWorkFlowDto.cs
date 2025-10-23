namespace ADE_WFM.Models.DTOs.WorkFlowDtos
{
    public class ResponseAddUserWorkFlowDto
    {
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; } = string.Empty;
        public List<WorkFlowUserDto> Users { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
