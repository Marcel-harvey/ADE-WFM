using ADE_WFM.Services.TenantService;
using System.ComponentModel.DataAnnotations;

namespace ADE_WFM.Models
{
    public class Tenant : ITenantEntity
    {
        public int Id { get; set; }
        // Interface ITenantEntity implementation
        int ITenantEntity.TenantId
        {
            get => Id;
            set => Id = value;
        }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Subdomain or domain (e.g. acme.adewfm.com)
        public string? Domain { get; set; }

        // Store connection string if want to move to DB per tenant
        public string? ConnectionString { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Navigation props (optional - helpful)
        public ICollection<WorkFlow>? WorkFlows { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public ICollection<Todo>? Todos { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<TaskPlanning>? Tasks { get; set; }
        public ICollection<SubTask>? SubTasks { get; set; }
        public ICollection<StickyNote>? StickyNotes { get; set; }
        public ICollection<TenantInvite>? TenantInvites { get; set;}
    }
}
