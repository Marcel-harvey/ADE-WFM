namespace ADE_WFM.Models.DTOs.TodoDtos
{
    public class MarkTodoCompletionDto
    {
        public int ToDoId { get; set; }
        public bool IsComplete { get; set; } = true;
    }
}
