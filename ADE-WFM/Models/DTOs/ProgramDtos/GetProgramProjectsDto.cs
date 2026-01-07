using ADE_WFM.Models.DTOs.ProgramDtos;

namespace ADE_WFM.Models.DTOs.WorkFlowDtos {
    public class GetProgramProjectsDto {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public List<TodoResponseDto>? Todos { get; set; }

        // Counts
        public int TodoCount => Todos?.Count ?? 0;
    }
}
