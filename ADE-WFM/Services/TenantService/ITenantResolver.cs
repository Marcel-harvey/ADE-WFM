using ADE_WFM.Models;

namespace ADE_WFM.Services.TenantService
{
    public interface ITenantResolver
    {
        Task<Tenant?> ResolveTenantAsync(HttpContext context);
    }
}
