namespace ADE_WFM.Models
{
    public class WorkFlowUser
    {
        public string Role { get; set; } = "Standard";

        // Many to many relationship keys
        public int WorkFlowId { get; set; }
        public WorkFlow WorkFlow { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

    }
}
