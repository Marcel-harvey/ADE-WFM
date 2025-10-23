namespace ADE_WFM.Models.DTOs.WorkFlowDtos
{
    public class RemoveUserFromWorkFlowDto
    {
        public string UserId { get; set; } = string.Empty;
        public int WorkFlowId { get; set; }
    }
}
