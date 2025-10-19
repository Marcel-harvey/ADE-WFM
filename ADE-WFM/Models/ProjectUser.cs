namespace ADE_WFM.Models
{
    public class ProjectUser
    {
        // Many to many relationship keys
        public int ProjectId { get; set; }
        public Project Project{ get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
