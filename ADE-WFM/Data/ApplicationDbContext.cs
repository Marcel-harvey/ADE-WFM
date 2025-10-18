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

            builder.Entity<WorkFlowUser>()
                .HasKey(wu => new { wu.WorkFlowId, wu.UserId }); // composite key

            // Cascade delete
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
                .HasMany(wf => wf.Comment)
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
    }
}
