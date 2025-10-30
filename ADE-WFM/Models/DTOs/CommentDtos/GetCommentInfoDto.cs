namespace ADE_WFM.Models.DTOs.CommentDtos
{
    public class GetCommentInfoDto
    {
        public int? CommentId { get; set; }
        public int? ProjectId { get; set; }
        public int? WorkFlowId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
