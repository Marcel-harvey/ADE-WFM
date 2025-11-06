using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ADE_WFM.Services.TenantService
{
    public class TenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Manual override values (for system or background operations)
        private int? _manualTenantId;
        private string? _manualTenantName;
        private string? _manualUserId;

        public TenantContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int TenantId
        {
            get
            {
                // 1️⃣ Try JWT claim
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id");
                if (claim != null && int.TryParse(claim.Value, out int tenantId))
                    return tenantId;

                // 2️⃣ Fallback to middleware manual set
                if (_manualTenantId.HasValue)
                    return _manualTenantId.Value;

                throw new InvalidOperationException("Tenant ID not found in context or token.");
            }
        }

        public string TenantName
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_name");
                return claim?.Value ?? _manualTenantName ?? string.Empty;
            }
        }

        public string UserId
        {
            get
            {
                // 1️⃣ Try JWT claim
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub);
                if (claim != null)
                    return claim.Value;

                // 2️⃣ Fallback to manual
                if (!string.IsNullOrEmpty(_manualUserId))
                    return _manualUserId;

                throw new InvalidOperationException("User ID not found in context or token.");
            }
        }

        public string? ConnectionString { get; private set; }

        public void SetTenant(int id, string name, string? connectionString = null)
        {
            _manualTenantId = id;
            _manualTenantName = name;
            ConnectionString = connectionString;
        }

        public void SetUser(string userId)
        {
            _manualUserId = userId;
        }
    }
}
