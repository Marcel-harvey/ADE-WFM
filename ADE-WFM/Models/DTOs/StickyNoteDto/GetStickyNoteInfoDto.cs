namespace ADE_WFM.Models.DTOs.StickyNoteDto
{
    public class GetStickyNoteInfoDto
    {
        public int StickyNoteId { get; set; }
        public string NewContent { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
