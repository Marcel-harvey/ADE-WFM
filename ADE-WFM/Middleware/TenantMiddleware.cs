using ADE_WFM.Data;
using ADE_WFM.Models;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                string? host = context.Request.Host.Host.ToLower();
                Tenant? tenant = null;

                // Option 1: use subdomain (e.g. acme.adewfm.com)
                if (host.Contains('.'))
                {
                    var subdomain = host.Split('.')[0];
                    tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Domain == subdomain);
                }

                // Option 2: fallback to header (for APIs)
                if (tenant == null && context.Request.Headers.TryGetValue("X-Tenant", out var tenantHeader))
                {
                    tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == tenantHeader);
                }

                if (tenant != null)
                {
                    context.Items["TenantId"] = tenant.Id;
                    context.Items["Tenant"] = tenant;
                    _logger.LogInformation("Resolved tenant: {TenantName}", tenant.Name);
                }
                else
                {
                    _logger.LogWarning("Tenant not found for host: {Host}", host);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving tenant.");
            }

            await _next(context);
        }
    }

    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TenantMiddleware>();
        }
    }
}
