namespace ADE_WFM.Models.ViewModels.WorkFlowViewModels
{
    public class AddCommentWorkFlowViewModel
    {
        public int WorkFlowId { get; set; }
        public Comment Comment { get; set; } = new Comment();
    }
}
