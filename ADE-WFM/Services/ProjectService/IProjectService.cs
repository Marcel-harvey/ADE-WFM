using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels.ProjectViewModels;

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
    }
}
