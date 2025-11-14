namespace ADE_WFM.Models.DTOs.ProjectDtos {
    public class ProjectCommentsInfoDto {
        public int CommentId { get; set; }
        public string CommentContent { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public bool IsViewed { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
