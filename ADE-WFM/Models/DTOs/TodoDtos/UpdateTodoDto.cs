namespace ADE_WFM.Models.DTOs.TodoDtos {
    public class UpdateTodoDto {
        public int TodoId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
}
