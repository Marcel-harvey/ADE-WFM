namespace ADE_WFM.Models.DTOs.ProgramDtos {
    public class ProgramProjectDetailsDto {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }

        // List of project children
        public List<UserDetailsDto>? Users { get; set; }
        public List<ProgramTodoDetailsDto>? Todos { get; set; }
    }
}
