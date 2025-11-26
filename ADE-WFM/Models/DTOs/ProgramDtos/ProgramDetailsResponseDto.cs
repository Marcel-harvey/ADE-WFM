namespace ADE_WFM.Models.DTOs.ProgramDtos {
    public class ProgramDetailsResponseDto {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProgramAuthor { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }

        // Lists of Program children
        public List<ProgramProjectDetailsDto>? Projects { get; set; }
        public List<ProgramCommentDetailsDto>? Comments { get; set; }
        public List<UserDetailsDto>? Users { get; set; }
    }
}
