namespace ADE_WFM.Models.DTOs.SubTaskDtos
{
    public class SubTaskResponseDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        public string? TodoTitle { get; set; } = string.Empty;

    }
}
