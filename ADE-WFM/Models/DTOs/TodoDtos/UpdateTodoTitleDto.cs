namespace ADE_WFM.Models.DTOs.TodoDtos
{
    public class UpdateTodoTitleDto
    {
        public int TodoId { get; set; }
        public string NewTitle { get; set; } = string.Empty;
    }
}
