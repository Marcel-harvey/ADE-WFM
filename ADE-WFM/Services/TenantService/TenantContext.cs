namespace ADE_WFM.Services.TenantService
{
    // Request Scoped Service for tenants
    public class TenantContext
    {
        public int? TenantId { get; private set; }
        public string? TenantName { get; private set; }

        public void SetTenant(int id, string name)
        {
            TenantId = id;
            TenantName = name;
        }
    }
}
