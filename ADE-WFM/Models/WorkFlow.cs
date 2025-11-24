using ADE_WFM.Services.TenantService;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models {
    public class WorkFlow : ITenantEntity {
        public int Id { get; set; }
        [Required]
        public string WorkFlowName { get; set; } = string.Empty;
        public string userCreated { get; set; } = string.Empty;
        public DateTime dateCreated { get; set; } = DateTime.UtcNow;


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
