namespace ADE_WFM.Models.DTOs.ProgramDtos {
    public class TodoResponseDto {
        public int todoId { get; set; }
        public string Task { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public DateOnly? DueDate { get; set; }
        public bool IsComplete { get; set; }
    }
}
