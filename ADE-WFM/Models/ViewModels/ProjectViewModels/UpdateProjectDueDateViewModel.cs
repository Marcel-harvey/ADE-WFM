namespace ADE_WFM.Models.ViewModels.ProjectViewModels
{
    public class UpdateProjectDueDateViewModel
    {
        public int ProjectId { get; set; }
        public DateOnly NewDueDate { get; set; }
    }
}
