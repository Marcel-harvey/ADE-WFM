namespace ADE_WFM.Models.DTOs.TodoDtos {
    public class AddTodoDto {
        public string Task { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public string? UserId { get; set; }
        public int ProjectId { get; set; }
    }
}
