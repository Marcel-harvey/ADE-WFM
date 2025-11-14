using ADE_WFM.Data;
using ADE_WFM.Models;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.TenantService {
    public class TenantResolver : ITenantResolver {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TenantResolver> _logger;

        public TenantResolver(ApplicationDbContext context, ILogger<TenantResolver> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task<Tenant?> ResolveTenantAsync(HttpContext context) {
            // 1️⃣  Determine tenant key from host or header
            string? host = context.Request.Host.Host;
            string? subDomain = host?.Split('.')?.FirstOrDefault();
            string? headerTenant = context.Request.Headers["X-Tenant"].FirstOrDefault();

            string? tenantKey = headerTenant ?? subDomain;

            // 2️⃣  Lookup tenant in DB
            Tenant? tenant = null;
            if (!string.IsNullOrEmpty(tenantKey)) {
                tenant = await _context.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        t.Domain != null &&
                        (t.Domain.Equals(tenantKey) || t.Domain.StartsWith($"{tenantKey}.")));
            }

            // 3️⃣  Fallback to default tenant
            if (tenant == null) {
                tenant = await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == 1);
                _logger.LogWarning("Tenant not found for key '{TenantKey}', fallback to default.", tenantKey);
            }

            return tenant;
        }
    }
}
