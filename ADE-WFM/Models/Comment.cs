using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public DateOnly DateCreated { get; set; }
        public string CommentContent { get; set; } = string.Empty;
        public bool IsViewed { get; set; } = false;


        // Foreign Keys
        public int ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public WorkFlow Company { get; set;} = null!;
    }
}
