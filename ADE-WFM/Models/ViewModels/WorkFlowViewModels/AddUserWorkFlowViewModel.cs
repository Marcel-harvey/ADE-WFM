namespace ADE_WFM.Models.ViewModels.WorkFlowViewModels
{
    public class AddUserWorkFlowViewModel
    {
        public List<string> UserIds { get; set; } = new();
        public int WorkFlowId { get; set; }
    }
}
