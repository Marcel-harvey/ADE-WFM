namespace ADE_WFM.Models.ViewModels.CommentViewModels
{
    public class AddCommentWorkFlowViewModel
    {
        public int WorkFlowId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Comment Comment { get; set; } = new Comment();
    }
}
