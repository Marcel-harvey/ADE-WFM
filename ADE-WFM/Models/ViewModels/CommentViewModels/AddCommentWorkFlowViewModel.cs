namespace ADE_WFM.Models.ViewModels.CommentViewModels
{
    public class AddCommentWorkFlowViewModel
    {
        public int WorkFlowId { get; set; }
        public Comment Comment { get; set; } = new Comment();
    }
}
