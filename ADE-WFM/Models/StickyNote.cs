using System.ComponentModel.DataAnnotations.Schema;

namespace ADE_WFM.Models
{
    public class StickyNote
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;


        // Navigation Properties
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;
    }
}
