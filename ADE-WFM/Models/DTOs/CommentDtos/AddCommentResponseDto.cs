namespace ADE_WFM.Models.DTOs.CommentDtos
{
    public class AddCommentResponseDto
    {
        // Response DTO used for work flow, project etc
        public int Id { get; set; }
        public DateOnly DateCreated { get; set; }
        public string CommentContent { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        // Optional selections
        public int? ProjectId { get; set; }
        public int? WorkFlowId { get; set; }
    }
}
