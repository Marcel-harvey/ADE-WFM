namespace ADE_WFM.Models.DTOs.CommentDtos {
    public class AddCommentDto {
        // DTO used for work flow, project etc
        public string CommentContent { get; set; } = string.Empty;

        // Optional selections
        public int? ProjectId { get; set; }
        public int? WorkFlowId { get; set; }
    }
}
