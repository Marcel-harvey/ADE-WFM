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
        Task<ServiceResult<GetProjectResponseDto>> GetProjectById(GetProjectByIdDto dto);
        Task<ServiceResult<GetProjectUsersResponseDto>> GetUsersInProject(GetProjectUsersDto dto);

        // UPDATE services
        Task<ServiceResult<UpdateProjectInfoResponseDto>> UpdateProjectInfo(UpdateProjectInfoDto dto);

        // DELETE services
        Task<ServiceResult<DeleteProjectResponseDto>> DeleteProject(DeleteProjectDto dto);
        Task<ServiceResult<ProjectUsersInfoDto>> RemoveUserFromProject(RemoveUserFromProjectDto dto);

        // ADD API services


        // DELETE API services
    }
}
