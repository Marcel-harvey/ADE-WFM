namespace ADE_WFM.Models.DTOs.WorkFlowViewModels {
    public class AddUserProgramDto {
        public List<string> UserIds { get; set; } = new();
        public int programId { get; set; }
    }
}
