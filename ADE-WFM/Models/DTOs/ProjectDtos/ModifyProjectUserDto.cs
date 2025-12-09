namespace ADE_WFM.Models.DTOs.ProjectDtos {
    public class ModifyProjectUserDto {
        public int ProjectId { get; set; }
        public List<string> UserIds { get; set; } = new();
    }
}
