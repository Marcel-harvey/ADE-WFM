namespace ADE_WFM.Models.DTOs.ProjectDtos
{
    public class ProjectTodosInfoDto
    {
        public int TodoId { get; set; }
        public bool TodoIsComplete { get; set; }
        public string TodoTitle { get; set; } = string.Empty;
        public string TodoDescription { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
        public string UserName { get; set; } = string.Empty;

        public List<ProjectTodoSubTasksInfoDto> ProjectTodoSubTasks { get; set; } = new();
    }

    public class ProjectTodoSubTasksInfoDto
    {
        public int SubTaskId { get; set; }
        public string SubTaskDescription { get; set; } = string.Empty;
        public bool SubTaskIsCompleted { get; set; }
    }
}
