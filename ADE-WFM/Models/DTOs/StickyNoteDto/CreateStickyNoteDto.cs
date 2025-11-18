using System.ComponentModel.DataAnnotations;

namespace ADE_WFM.Models.DTOs.StickyNoteDto {
    public class CreateStickyNoteDto {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
