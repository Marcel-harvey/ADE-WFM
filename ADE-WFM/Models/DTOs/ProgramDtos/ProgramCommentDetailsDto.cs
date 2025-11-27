namespace ADE_WFM.Models.DTOs.ProgramDtos {
    public class ProgramCommentDetailsDto {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
    }
}
