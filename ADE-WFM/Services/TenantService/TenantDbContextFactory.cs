using ADE_WFM.Data;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.TenantService {
    public class TenantDbContextFactory {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantDbContextFactory(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public ApplicationDbContext CreateDbContext(string? tenantConnectionString = null) {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            if (!string.IsNullOrEmpty(tenantConnectionString)) {
                optionsBuilder.UseNpgsql(tenantConnectionString);
            }
            else {
                var defaultConnection = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(defaultConnection);
            }

            return new ApplicationDbContext(optionsBuilder.Options, _httpContextAccessor);
        }
    }
}
