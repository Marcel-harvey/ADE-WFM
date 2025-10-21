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
            // Cascade deletes for WorkFlow related entities
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

            // Cascade deletes for Project related entities
            // Project - Comments
            builder.Entity<Project>()
                .HasMany(p => p.Comment)
                .WithOne(c => c.Project)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Project - Todos (if linked)
            builder.Entity<Project>()
                .HasMany(p => p.PorjectTodos)
                .WithOne(c => c.Project)
                .OnDelete(DeleteBehavior.Cascade);

            // Project - TaskPlanning (if linked)
            builder.Entity<Project>()
                .HasMany(p => p.Task)
                .WithOne(c => c.Project)
                .OnDelete(DeleteBehavior.Cascade);

            // DO NOT CASCADE — because multiple Projects share a WorkFlow
            builder.Entity<Project>()
                .HasOne(p => p.WorkFlows)
                .WithMany(wf => wf.Project) // if WorkFlow has a Projects collection
                .HasForeignKey(p => p.WorkFlowId)
                .OnDelete(DeleteBehavior.Restrict);

            // When deleting user will cascade delete their StickyNotes - NOT otherway around
            builder.Entity<StickyNote>()
                .HasOne(sn => sn.User)
                .WithMany(u => u.StickyNote)
                .HasForeignKey(sn => sn.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade deletes for Todo related entities
            builder.Entity<Todo>()
                .HasMany(t => t.SubTasks)
                .WithOne(st => st.Todo)
                .HasForeignKey(st => st.TodoId)
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
