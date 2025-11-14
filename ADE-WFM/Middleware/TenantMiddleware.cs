using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Services.TenantService;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace ADE_WFM.Middleware {
    public class TenantMiddleware {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TenantContext tenantContext, ApplicationDbContext db) {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            Tenant? tenant = null;

            // 1️⃣ If JWT token present, prefer extracting tenant info from it
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ")) {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var tenantIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
                var tenantNameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "tenant_name")?.Value ?? string.Empty;

                if (!string.IsNullOrEmpty(userId) && int.TryParse(tenantIdClaim, out int tenantId)) {
                    // Retrieve tenant connection info if needed
                    tenant = await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId);
                    tenantContext.SetTenant(tenant?.Id ?? tenantId, tenant?.Name ?? tenantNameClaim, tenant?.ConnectionString);
                    tenantContext.SetUser(userId);
                    context.Items["TenantContext"] = tenantContext;
                    await _next(context);
                    return;
                }
            }

            // 2️⃣ Otherwise fallback to subdomain
            string? host = context.Request.Host.Host;
            string? subdomain = host?.Split('.')?.FirstOrDefault();

            if (!string.IsNullOrEmpty(subdomain)) {
                tenant = await db.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Domain != null && t.Domain.Equals(subdomain));
            }

            // 3️⃣ Default fallback (tenant 1)
            tenant ??= await db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == 1);

            if (tenant != null) {
                tenantContext.SetTenant(tenant.Id, tenant.Name, tenant.ConnectionString);
                context.Items["TenantContext"] = tenantContext;
            }

            await _next(context);
        }
    }
}
