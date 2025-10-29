namespace ADE_WFM.Models.DTOs.TodoDtos
{
    public class GetToDoDto
    {
        public int? ToDoId { get; set; }
        public string? UserId { get; set; }
        public int? ProjectId { get; set; }
    }
}
