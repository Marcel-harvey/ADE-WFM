namespace ADE_WFM.Models.DTOs.TodoDtos {
    public class UpdateTodoDto {
        public int TodoId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Task { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public bool? IsComplete { get; set; }
    }
}
