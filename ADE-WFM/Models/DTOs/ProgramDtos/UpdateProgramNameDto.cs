namespace ADE_WFM.Models.DTOs.WorkFlowViewModels {
    public class UpdateProgramNameDto {
        public string? ProgramName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateOnly? DueDate { get; set; }
        public int ProgramId { get; set; }
    }
}
