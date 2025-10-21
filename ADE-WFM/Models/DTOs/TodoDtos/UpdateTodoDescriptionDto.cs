namespace ADE_WFM.Models.DTOs.TodoDtos
{
    public class UpdateTodoDescriptionDto
    {
        public int TodoId { get; set; }
        public string NewDescription { get; set; } = string.Empty;
    }
}
