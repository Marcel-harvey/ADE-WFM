using ADE_WFM.Models.DTOs.WorkFlowDtos;

namespace ADE_WFM.Models.DTOs.ProgramDtos {
    public class UsersProgramResponseDto {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public DateOnly DueDate { get; set; }

        // Lists
        public List<GetProgramProjectsDto>? IncompleteProjects { get; set; }
        public List<GetProgramUsersDto>? Users { get; set; }

        // Counts
        public int ProjectCount => IncompleteProjects?.Count ?? 0;
        public int UserCount => Users?.Count ?? 0;
        // Sum total of all todo counts per project to give total count for entire program
        public int TotalTodoCount => IncompleteProjects?.Sum(p => p.TodoCount) ?? 0;
    }
}
