namespace ADE_WFM.Models.DTOs.TodoDtos
{
    public class UpdateTodoDueDateDto
    {
        public int TodoId { get; set; }
        public DateTime NewDueDate { get; set; }
    }
}
