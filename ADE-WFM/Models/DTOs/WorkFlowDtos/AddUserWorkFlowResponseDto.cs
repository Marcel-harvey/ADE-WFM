namespace ADE_WFM.Models.DTOs.WorkFlowDtos
{
    public class AddUserWorkFlowResponseDto
    {
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; } = string.Empty;
        public List<WorkFlowUserDto> Users { get; set; } = new();
    }
}
