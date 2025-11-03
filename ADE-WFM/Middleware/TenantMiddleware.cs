using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Services.TenantService;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TenantContext tenantContext, ApplicationDbContext db)
        {
            // Extract tenant from subdomain or header
            string? host = context.Request.Host.Host;
            string? domain = host?.Split('.')?.FirstOrDefault();
            Tenant? tenant = null;

            if (!string.IsNullOrEmpty(domain))
            {
                tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Domain != null && t.Domain.Equals(domain));
            }

            // fallback to default tenant
            tenant ??= await db.Tenants.FirstOrDefaultAsync(t => t.Id == 1);

            if (tenant != null)
                tenantContext.SetTenant(tenant.Id, tenant.Name);

            await _next(context);
        }
    }
}
