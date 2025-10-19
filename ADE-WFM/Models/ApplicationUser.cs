using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Navigation Properties
        public ICollection<Project>? Projects { get; set; }
        public ICollection<Todo>? Todo { get; set; }
        public ICollection<Comment>? Comment { get; set; }
        public ICollection<StickyNote>? StickyNote { get; set; }


        // Foreign Key - many to many relationship via WorkFlowUser
        public ICollection<WorkFlowUser> WorkFlowUsers { get; set; } = new List<WorkFlowUser>();
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}
