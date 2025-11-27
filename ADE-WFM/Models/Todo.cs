using ADE_WFM.Services.TenantService;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models {
    public class Todo : ITenantEntity {
        public int Id { get; set; }
        [Required]
        public bool IsComplete { get; set; }
        [Required]
        public string Task { get; set; } = string.Empty;
        [Required]
        public DateOnly DateCreated { get; set; }
        [Required]
        public DateOnly? DueDate { get; set; }


        // Navigation Properties
        public ICollection<SubTask>? SubTasks { get; set; }


        // Foreign Keys// Identity uses string not int
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;


        // Tenant Setup
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;

    }
}
