namespace ADE_WFM.Models.DTOs.SubTaskDtos
{
    public class AddSubTasksToTodoDto
    {
        public int TodoId { get; set; }
        public List<AddSubTaskDto> SubTasks { get; set; } = new();
    }

    public class AddSubTaskDto
    {
        public string Description { get; set; } = string.Empty;
    }
}
