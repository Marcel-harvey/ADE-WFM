namespace ADE_WFM.Models.ViewModels.ProjectViewModels
{
    public class UpdateProjectDescriptionViewModel
    {
        public int projectId { get; set; }
        public string newProjectDescription { get; set; } = string.Empty;
    }
}
