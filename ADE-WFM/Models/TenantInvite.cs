using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models
{
    public class TenantInvite
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; } = false;

        // Relations
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; } = null!;
    }
}
