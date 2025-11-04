namespace ADE_WFM.Services.TenantService
{
    public class TenantContext
    {
        public int TenantId { get; private set; }
        public string TenantName { get; private set; } = string.Empty;
        public string? ConnectionString { get; private set; }

        public void SetTenant(int id, string name, string? connectionString = null)
        {
            TenantId = id;
            TenantName = name;
            ConnectionString = connectionString;
        }
    }
}
