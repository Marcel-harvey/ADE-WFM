namespace ADE_WFM.Models.DTOs.CommentDtos {
    public class CommentResponseDto {
        public int CommentId { get; set; }
        public string CommentContent { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public bool IsViewed { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public int? programId { get; set; }
        public string? ProgramName { get; set; }
    }
}
