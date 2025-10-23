namespace ADE_WFM.Models.DTOs.WorkFlowDtos
{
    public class ResponseCreateWorkFlowDto
    {
        public int Id { get; set; }
        public string WorkFlowName { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; } = string.Empty;
        public List<string> AssignedUserIds { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = "Workflow created successfully";
    }
}
