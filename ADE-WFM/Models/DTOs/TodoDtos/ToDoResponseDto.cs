namespace ADE_WFM.Models.DTOs.TodoDtos
{
    public class ToDoResponseDto
    {
        public int Id { get; set; }
        public bool IsComplete { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
    }
}
