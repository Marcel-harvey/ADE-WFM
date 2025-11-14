using ADE_WFM.Data;
using ADE_WFM.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ADE_WFM.Services.JwtService {
    public class JwtService : IJwtService {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public JwtService(
            IConfiguration config,
            ApplicationDbContext context) {
            _config = config;
            _context = context;
        }

        public string GenerateToken(ApplicationUser user, Tenant tenant, string role) {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
                new Claim("user_name", user.UserName ?? ""),
                new Claim("tenant_id", tenant.Id.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpiryHours"] ?? "12")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
