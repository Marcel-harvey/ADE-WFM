namespace ADE_WFM.Models.DTOs.WorkFlowDtos {
    public class CreateWorkFlowDto {
        public string WorkFlowName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public List<string> UserIds { get; set; } = new();
    }
}
