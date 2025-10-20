using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjectDescription { get; set; }
        [Required]
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate {  get; set; }


        // Navigation Properties
        public ICollection<Todo>? PorjectTodos { get; set; }
        public ICollection<Comment>? Comment { get; set; }
        public ICollection<TaskPlanning>? Task { get; set; }


        // Foreign Key
        public int WorkFlowId { get; set; }
        [ForeignKey(nameof(WorkFlowId))]
        public WorkFlow WorkFlows { get; set; } = null!;

        // Foreign Key many to many via WorkFlowUser
        public ICollection<ProjectUser> ProjectUsers{ get; set; } = new List<ProjectUser>();
    }
}
