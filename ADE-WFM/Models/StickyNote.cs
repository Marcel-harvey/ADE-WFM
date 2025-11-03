using ADE_WFM.Services.TenantService;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models
{
    public class StickyNote : ITenantEntity
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;


        // Navigation Properties
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;


        // Tenand Setup
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;
    }
}
