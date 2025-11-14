namespace ADE_WFM.Models.DTOs.SubTaskDtos {
    public class MarkSubTaskCompletionDto {
        public int TodoId { get; set; }
        public int SubTaskId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
