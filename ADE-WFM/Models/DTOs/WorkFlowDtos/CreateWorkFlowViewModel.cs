using System.ComponentModel.DataAnnotations;

namespace ADE_WFM.Models.DTOs.WorkFlowViewModels
{
    public class CreateWorkFlowViewModel
    {
        [Required]
        [Display(Name = "Work Flow Name")]
        public string WorkFlowName { get; set; } = string.Empty;
        [Required]
        public string CurrentUserId { get; set; } = string.Empty;
        public List<string> UserIds { get; set; } = new();
    }
}
