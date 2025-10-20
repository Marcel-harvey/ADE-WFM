namespace ADE_WFM.Models.DTOs.StickyNoteDto
{
    public class UpdateStickyNoteDto
    {
        public int StickyNoteId { get; set; }
        public string NewContent { get; set; } = string.Empty;
    }
}
