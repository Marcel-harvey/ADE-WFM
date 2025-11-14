namespace ADE_WFM.Models.DTOs.ProjectDtos {
    public class AddUserToProjectDto {
        // TODO: Create as list entrys to add multiple users at once
        public int ProjectId { get; set; }
        public string AddUserId { get; set; } = string.Empty;
    }
}
