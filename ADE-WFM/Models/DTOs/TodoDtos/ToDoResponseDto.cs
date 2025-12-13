namespace ADE_WFM.Models.DTOs.TodoDtos {
    public class ToDoResponseDto {
        public int todoId { get; set; }
        public bool IsComplete { get; set; }
        public string Task { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public List<TodoSubTasksResponseDto> SubTasks { get; set; } = new();
    }

    public class TodoSubTasksResponseDto {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
