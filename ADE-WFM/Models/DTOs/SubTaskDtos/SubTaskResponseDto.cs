namespace ADE_WFM.Models.DTOs.SubTaskDtos
{
    public class SubTaskResponseDto
    {
        public int SubTaskId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        public int? TodoId { get; set; }
        public string? TodoTitle { get; set; } = string.Empty;
    }
}
