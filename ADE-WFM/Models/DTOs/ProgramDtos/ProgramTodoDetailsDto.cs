namespace ADE_WFM.Models.DTOs.ProgramDtos {
    public class ProgramTodoDetailsDto {
        public int TodoId { get; set; }
        public bool isComplete { get; set; }
        public string Task { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }
    }
}
