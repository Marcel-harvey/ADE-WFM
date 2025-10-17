using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models
{
    public class WorkFlow
    {
        public int Id { get; set; }
        [Required]
        public string WorkFlowName { get; set; } = string.Empty;


        // Navigation Properties
        public ICollection<Project>? Project { get; set; }
        public ICollection<Comment>? Comment { get; set; }


        // Foreign Key many to many via WorkFlowUser
        public ICollection<WorkFlowUser> WorkFlowUsers { get; set; } = new List<WorkFlowUser>();
    }
}
