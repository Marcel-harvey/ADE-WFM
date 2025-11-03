using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Services.CommentService;
using ADE_WFM.Services.WorkFlowService;
using ADE_WFM.Services.ProjectService;
using ADE_WFM.Services.StickyNoteService;
using ADE_WFM.Services.TodoService;
using ADE_WFM.Services.SubTaskService;
using ADE_WFM.Services.UserService;
using ADE_WFM.Services.TenantService;
using ADE_WFM.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ======================================
// 1️⃣ Core Framework Services
// ======================================
builder.Services.AddControllers(); // API only, no MVC Views

// ======================================
// 2️⃣ Database Context (Single DB setup)
// ======================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================================
// 3️⃣ Identity (Core Authentication)
// ======================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ======================================
// 4️⃣ Custom App Services
// ======================================
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IWorkFlowService, WorkFlowService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStickyNoteService, StickyNoteService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ISubTaskService, SubTaskService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TenantContext>();

// ======================================
// 5️⃣ Swagger (Docs & Testing)
// ======================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================================
// 6️⃣ CORS (Allow Angular / Frontend access)
// ======================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ======================================
// 7️⃣ Development Environment Config
// ======================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ADE-WFM API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root "/"
    });
}

// ======================================
// 8️⃣ Database Setup & Default Data
// ======================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Standard", "View" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var adminEmail = "marcel@ade.com";
    var adminPassword = "Admin123!";
    var adminRole = "Admin";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, adminRole);
        else
            Console.WriteLine("Failed to create admin: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    // Run DB seeder
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DbSeeder.SeedAsync(ctx, logger);
}

// ======================================
// 9️⃣ Middleware Pipeline
// ======================================
app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<TenantMiddleware>(); // 🏢 Identify tenant

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Check Endpoint
app.MapGet("/health", () => Results.Ok("ADE-WFM API is running ✅"));

// ======================================
app.Run();
