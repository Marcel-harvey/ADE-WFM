using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ADE_WFM.Models
{
    public class Todo
    {
        public int Id { get; set; }
        [Required]
        public bool IsComplete { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]


        // Navigation Properties
        public ICollection<SubTask>? SubTasks { get; set; }


        // Foreign Keys
        // Identity uses string not int
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

       public int ProjectId { get; set; }
       public Project? Project { get; set; }

    }
}
