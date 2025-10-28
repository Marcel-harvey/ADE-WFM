namespace ADE_WFM.Models.DTOs.CommentDtos
{
    public class GetCommentsResponseDto
    {
        // DTO is used for response in Get users, Get projects, Get workflows, Get comments APIs
        public int CommentId { get; set; }
        public string CommentContent { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public string? WorkFlowName { get; set; }
        public string? ProjectTitle { get; set; }
        public string? UserName { get; set; }
    }
}
