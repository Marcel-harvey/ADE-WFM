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
        public int WorkFlowId { get; set; }
        [ForeignKey(nameof(WorkFlowId))]
        public WorkFlow WorkFlow { get; set; } = null!;

        public int TodoId { get; set; }
        [ForeignKey(nameof(TodoId))]
        public Todo Todo { get; set; } = null!;

    }
}
