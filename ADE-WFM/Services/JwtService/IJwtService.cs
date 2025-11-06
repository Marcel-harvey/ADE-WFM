using ADE_WFM.Models;

namespace ADE_WFM.Services.JwtService
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, Tenant tenant, string role);
    }
}
