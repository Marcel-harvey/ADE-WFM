using ADE_WFM.Services.TenantService;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models {
    public class BusinessProgram : ITenantEntity {
        public int Id { get; set; }
        [Required]
        public string ProgramName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly DueDate { get; set; }
        public bool Iscompleted { get; set; } = false;


        // Navigation Properties
        public ICollection<Project>? Project { get; set; }
        public ICollection<Comment>? Comments { get; set; }


        // Foreign Key many to many via WorkFlowUser
        public ICollection<WorkFlowUser> WorkFlowUsers { get; set; } = new List<WorkFlowUser>();


        // Tenand Setup
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;
    }
}
