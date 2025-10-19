using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ADE_WFM.Models; 

namespace ADE_WFM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Composite key
            builder.Entity<WorkFlowUser>()
                .HasKey(wu => new { wu.WorkFlowId, wu.UserId }); 

            builder.Entity<ProjectUser>()
                .HasKey(wu => new { wu.ProjectId, wu.UserId });

            // Cascade deletes
            // WorkFlowUser relationships
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

            // ProjectUser relationships
            builder.Entity<ProjectUser>()
                .HasOne(wu => wu.Project)
                .WithMany(w => w.ProjectUsers)
                .HasForeignKey(wu => wu.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectUser>()
                .HasOne(wu => wu.User)
                .WithMany(u => u.ProjectUsers)
                .HasForeignKey(wu => wu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkFlow>()
                .HasMany(wf => wf.Comments)
                .WithOne(c => c.WorkFlow)
                .HasForeignKey(c => c.WorkFlowId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        // Company Info
        public DbSet<WorkFlow> WorkFlows { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<StickyNote> StickyNotes { get; set; }
        public DbSet<WorkFlowUser> WorkFlowUsers { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
    }
}
