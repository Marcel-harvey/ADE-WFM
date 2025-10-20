using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.ProjectDtos;

namespace ADE_WFM.Services.ProjectService
{
    public interface IProjectService
    {
        // GET services
        Task<List<Project>> GetAllProjects();
        Task<Project> GetProjectById(int projectId);
        Task<List<ApplicationUser>> GetUsersInProject(int projectId);

        // UPDATE services


        // ADD services
        Task AddProject(CreateProjectDto dto);

        // DELETE services


        // API services
        // UPDATE API services
        Task<Project> UpdateProjectTitle(UpdateProjectTitleDto model);
        Task<Project> UpdateProjectDescription(UpdateProjectDescriptionDto model);
        Task<Project> UpdateProjectDueDate(UpdateProjectDueDateDto model);

        // ADD API services
        Task<ApplicationUser> AddUserToProject(AddUserProjectDto model);

        // DELETE API services
        Task DeleteProject(DeleteProjectDto dto);
        Task RemoveUserFromProject(RemoveUserFromProjectDto dto);
    }
}
