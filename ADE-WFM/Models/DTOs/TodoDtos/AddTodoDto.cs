namespace ADE_WFM.Models.DTOs.TodoDtos
{
    public class AddTodoDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public List<AddSubTaskWithTodoDto>? SubTasks { get; set; }
    }

    // Only linked to Todo
    public class AddSubTaskWithTodoDto
    {
        public bool IsCompleted { get; set; } = false;
        public string Description { get; set; } = string.Empty;
    }
}
