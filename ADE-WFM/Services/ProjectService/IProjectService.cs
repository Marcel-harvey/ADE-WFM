using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;

namespace ADE_WFM.Services.ProjectService
{
    public interface IProjectService
    {

        // ADD services
        Task<ServiceResult<ProjectResponseDto>> CreateProject(CreateProjectDto dto);
        // TODO: Convert to batch add
        Task<ServiceResult<ProjectResponseDto>> AddUserToProject(AddUserToProjectDto dto);

        // GET services
        Task<ServiceResult<List<ProjectResponseDto>>> GetAllProjects();
        Task<ServiceResult<ProjectResponseDto>> GetProjectById(GetProjectDto dto);

        // UPDATE services
        Task<ServiceResult<ProjectResponseDto>> UpdateProjectInfo(UpdateProjectInfoDto dto);

        // DELETE services
        Task<ServiceResult<ProjectResponseDto>> DeleteProject(GetProjectDto dto);
        Task<ServiceResult<ProjectResponseDto>> RemoveUserFromProject(GetProjectDto dto);

        // ADD API services


        // DELETE API services
    }
}
