namespace ADE_WFM.Models.DTOs.SubTaskDtos
{
    public class UpdateSubTaskDto
    {
        public int TodoId { get; set; }
        public int SubTaskId { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
