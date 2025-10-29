namespace ADE_WFM.Models.DTOs.CommentDtos
{
    public class DeleteCommentDto
    {
        public int CommentId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
