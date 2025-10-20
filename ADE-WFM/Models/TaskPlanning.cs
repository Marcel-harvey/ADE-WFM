namespace ADE_WFM.Models
{
    public class TaskPlanning
    {
        public int Id { get; set; }

        // Foreign Keys
        // Identity uses string not int
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

    }
}
