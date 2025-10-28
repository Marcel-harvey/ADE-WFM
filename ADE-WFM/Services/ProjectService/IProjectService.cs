using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;

namespace ADE_WFM.Services.ProjectService
{
    public interface IProjectService
    {

        // ADD services
        Task<ServiceResult<CreateProjectResponseDto>> CreateProject(CreateProjectDto dto);
        Task<ServiceResult<ProjectUsersInfoDto>> AddUserToProject(AddUserToProjectDto dto);

        // GET services
        Task<ServiceResult<List<GetProjectResponseDto>>> GetAllProjects();
        Task<ServiceResult<List<GetProjectResponseDto>>> GetProjectById(GetProjectByIdDto dto);
        Task<ServiceResult<GetProjectUsersResponseDto>> GetUsersInProject(GetProjectUsersDto dto);

        // UPDATE services
        Task<ServiceResult<UpdateProjectInfoResponseDto>> UpdateProjectInfo(UpdateProjectInfoDto dto);

        // DELETE services


        // API services
        // UPDATE API services
        Task<Project> UpdateProjectTitle(UpdateProjectTitleDto model);
        Task<Project> UpdateProjectDescription(UpdateProjectDescriptionDto model);
        Task<Project> UpdateProjectDueDate(UpdateProjectDueDateDto model);

        // ADD API services
        

        // DELETE API services
        Task DeleteProject(DeleteProjectDto dto);
        Task RemoveUserFromProject(RemoveUserFromProjectDto dto);
    }
}
