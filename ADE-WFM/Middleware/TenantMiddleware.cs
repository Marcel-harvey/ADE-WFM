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
            string? host = context.Request.Host.Host;
            string? subdomain = host?.Split('.')?.FirstOrDefault();

            Tenant? tenant = null;

            if (!string.IsNullOrEmpty(subdomain))
            {
                tenant = await db.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Domain != null && t.Domain.Equals(subdomain));
            }

            tenant ??= await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == 1);

            if (tenant != null)
            {
                tenantContext.SetTenant(tenant.Id, tenant.Name, tenant.ConnectionString);
                context.Items["TenantContext"] = tenantContext;
            }

            await _next(context);
        }

    }
}
