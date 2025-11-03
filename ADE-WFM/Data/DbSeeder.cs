using ADE_WFM.Models;

namespace ADE_WFM.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext ctx, ILogger logger)
        {
            if (!ctx.Tenants.Any())
            {
                ctx.Tenants.Add(new Tenant { Name = "Default", Domain = "default" });
                await ctx.SaveChangesAsync();
                logger.LogInformation("Seeded default tenant.");
            }
        }
    }
}
