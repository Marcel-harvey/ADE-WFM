using ADE_WFM.Models;
using ADE_WFM.Services.TenantService;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Data {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor
        ) : base(options) {
            _httpContextAccessor = httpContextAccessor;
        }

        // ==============================================
        //                  DbSets
        // ==============================================
        public DbSet<WorkFlow> WorkFlows { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<StickyNote> StickyNotes { get; set; }
        public DbSet<WorkFlowUser> WorkFlowUsers { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantInvite> TenantInvites { get; set; }

        // ==============================================
        //                  OnModelCreating
        // ==============================================
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            // ===========================
            //  COMPOSITE KEYS
            // ===========================
            builder.Entity<WorkFlowUser>().HasKey(wu => new { wu.WorkFlowId, wu.UserId });
            builder.Entity<ProjectUser>().HasKey(pu => new { pu.ProjectId, pu.UserId });

            // ===========================
            //  WORKFLOW RELATIONSHIPS
            // ===========================
            builder.Entity<WorkFlowUser>()
                .HasOne(wu => wu.WorkFlow)
                .WithMany(w => w.WorkFlowUsers)
                .HasForeignKey(wu => wu.WorkFlowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkFlowUser>()
                .HasOne(wu => wu.User)
                .WithMany(u => u.WorkFlowUsers)
                .HasForeignKey(wu => wu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkFlow>()
                .HasMany(wf => wf.Comments)
                .WithOne(c => c.WorkFlow)
                .HasForeignKey(c => c.WorkFlowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkFlow>()
                .HasMany(wf => wf.Project)
                .WithOne(p => p.WorkFlows)
                .HasForeignKey(p => p.WorkFlowId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===========================
            //  PROJECT RELATIONSHIPS
            // ===========================
            builder.Entity<ProjectUser>()
                .HasOne(pu => pu.Project)
                .WithMany(p => p.ProjectUsers)
                .HasForeignKey(pu => pu.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectUser>()
                .HasOne(pu => pu.User)
                .WithMany(u => u.ProjectUsers)
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>()
                .HasMany(p => p.Comment)
                .WithOne(c => c.Project)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>()
                .HasMany(p => p.PorjectTodos)
                .WithOne(t => t.Project)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>()
                .HasMany(p => p.Task)
                .WithOne(t => t.Project)
                .OnDelete(DeleteBehavior.Cascade);

            // ===========================
            //  STICKY NOTE RELATIONSHIP
            // ===========================
            builder.Entity<StickyNote>()
                .HasOne(sn => sn.User)
                .WithMany(u => u.StickyNote)
                .HasForeignKey(sn => sn.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===========================
            //  TODO RELATIONSHIPS
            // ===========================
            builder.Entity<Todo>()
                .HasMany(t => t.SubTasks)
                .WithOne(st => st.Todo)
                .HasForeignKey(st => st.TodoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===========================
            //  TENANT RELATIONSHIPS
            // ===========================
            builder.Entity<Tenant>()
                .HasMany(t => t.Projects)
                .WithOne(p => p.Tenant)
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tenant>()
                .HasMany(t => t.WorkFlows)
                .WithOne(wf => wf.Tenant)
                .HasForeignKey(wf => wf.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tenant>()
                .HasMany(t => t.Todos)
                .WithOne(td => td.Tenant)
                .HasForeignKey(td => td.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===========================
            //  GLOBAL TENANT FILTERS
            // ===========================
            foreach (var entityType in builder.Model.GetEntityTypes()) {
                if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType) &&
                    entityType.ClrType != typeof(Tenant)) // <-- exclude Tenant itself
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.MakeGenericMethod(entityType.ClrType);

                    method?.Invoke(this, new object[] { builder });
                }
            }

        }

        // =========================================================
        // Applies Tenant filter automatically for all ITenantEntity
        // =========================================================
        private void SetTenantFilter<T>(ModelBuilder builder) where T : class, ITenantEntity {
            builder.Entity<T>().HasQueryFilter(e => e.TenantId == CurrentTenantId || CurrentTenantId == null);
        }

        // =========================================================
        //  Determine current tenant from context or middleware
        // =========================================================
        private int? CurrentTenantId {
            get {
                // Prefer middleware tenant context (set in HttpContext.Items)
                var tenantContext = _httpContextAccessor.HttpContext?.Items["TenantContext"] as TenantContext;
                if (tenantContext != null && tenantContext.TenantId > 0)
                    return tenantContext.TenantId;

                // Fallback: read from user claims (for APIs)
                var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
                return int.TryParse(tenantClaim, out var id) ? id : null;
            }
        }
    }
}
