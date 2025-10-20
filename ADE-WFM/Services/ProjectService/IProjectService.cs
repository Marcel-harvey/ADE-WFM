using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels.ProjectViewModels;
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

        // DELETE services

        // UPDATE API services
        Task<Project> UpdateProjectTitle(UpdateProjectTitleViewModel model);
        Task<Project> UpdateProjectDescription(UpdateProjectDescriptionViewModel model);
        Task<Project> UpdateProjectDueDate(UpdateProjectDueDateViewModel model);

        // ADD API services
        Task<ApplicationUser> AddUserToProject(AddUserProjectViewModel model);

        // DELETE API services
        Task<Project> DeleteProject(DeleteProjectDto dto);
        Task RemoveUserFromProject(RemoveUserFromProjectDto dto);
    }
}
