namespace ADE_WFM.Models.DTOs.WorkFlowViewModels
{
    public class AddUserWorkFlowDto
    {
        public List<string> UserIds { get; set; } = new();
        public int WorkFlowId { get; set; }
    }
}
