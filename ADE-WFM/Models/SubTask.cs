using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public bool IsCompleted { get; set; }

        // Foreign Keys
        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public WorkFlow Company { get; set; } = null!;

        public int TodoId { get; set; }
        [ForeignKey(nameof(TodoId))]
        public Todo Todo { get; set; } = null!;

    }
}
