using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;
using ADE_WFM.Services.ProjectService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }


        // CREATE API's
        // Create a new projects
        [HttpPost("Create-new")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            try
            {
                var result = await _projectService.CreateProject(dto);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<CreateProjectResponseDto>.Failure(
                    "An unexpected error occurred while creating the project.",
                    new[] { ex.Message }
                ));
            }
        }


        // Add user to a project
        [HttpPost("Add-user")]
        public async Task<IActionResult> AddUserToProject([FromBody] AddUserToProjectDto dto)
        {
            try
            {
                var result = await _projectService.AddUserToProject(dto);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<ProjectUsersInfoDto>.Failure(
                    "An unexpected error occurred while adding the user to the project.",
                    new[] { ex.Message }
                ));
            }
        }


        // GET API's
        // Get all projects
        [HttpGet("Get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _projectService.GetAllProjects();

                if (!result.Succeeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<List<GetProjectResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving all projects.",
                    new[] { ex.Message }
                ));
            }
        }


        // Get project by Id
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var dto = new GetProjectByIdDto { Id = id };
                var result = await _projectService.GetProjectById(dto);

                if (!result.Succeeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<GetProjectResponseDto>.Failure(
                    "An unexpected error occurred while retrieving the project.",
                    new[] { ex.Message }
                ));
            }
        }


        // Get users in a project
        [HttpGet("Get-users/{projectId}")]
        public async Task<IActionResult> GetUsersInProject(int projectId)
        {
            try
            {
                var dto = new GetProjectUsersDto { Id = projectId };
                var result = await _projectService.GetUsersInProject(dto);

                if (!result.Succeeded)
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<GetProjectUsersResponseDto>.Failure(
                    "An unexpected error occurred while retrieving users in the project.",
                    new[] { ex.Message }
                ));
            }
        }

        // UPDATE API's
        // Update project info
        [HttpPut("Update-info")]
        public async Task<IActionResult> UpdateProjectInfo([FromBody] UpdateProjectInfoDto dto)
        {
            try
            {
                var result = await _projectService.UpdateProjectInfo(dto);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<UpdateProjectInfoResponseDto>.Failure(
                    "An unexpected error occurred while updating the project info.",
                    new[] { ex.Message }
                ));
            }
        }

        // DELETE API's
        // Delete a project
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteProject([FromBody] DeleteProjectDto dto)
        {
            try
            {
                var result = await _projectService.DeleteProject(dto);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<DeleteProjectResponseDto>.Failure(
                    "An unexpected error occurred while deleting the project.",
                    new[] { ex.Message }
                ));
            }
        }


        // Remove user from a project
        [HttpDelete("Remove-user")]
        public async Task<IActionResult> RemoveUserFromProject([FromBody] RemoveUserFromProjectDto dto)
        {
            try
            {
                var result = await _projectService.RemoveUserFromProject(dto);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<ProjectUsersInfoDto>.Failure(
                    "An unexpected error occurred while removing the user from the project.",
                    new[] { ex.Message }
                ));
            }
        }
    }
}
