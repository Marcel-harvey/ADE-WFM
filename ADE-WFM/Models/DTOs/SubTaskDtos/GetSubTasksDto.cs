namespace ADE_WFM.Models.DTOs.SubTaskDtos
{
    public class GetSubTasksDto
    {
        public int? SubTaskId { get; set; }
        public string? UserId { get; set; } = string.Empty;
        public int? TodoId { get; set; }
    }
}
