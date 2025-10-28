namespace ADE_WFM.Models.DTOs.CommentDtos
{
    public class UpdateCommentViewedResponseDto
    {
        public int CommentId { get; set; }
        public bool IsViewed { get; set; } = true;
    }
}
