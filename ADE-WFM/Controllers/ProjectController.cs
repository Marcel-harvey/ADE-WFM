using ADE_WFM.Models.DTOs.ProjectDtos;
using ADE_WFM.Services.ProjectService;
using Microsoft.AspNetCore.Mvc;

namespace ADE_WFM.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService) {
            _projectService = projectService;
        }


        // CREATE API's
        // Create a new projects
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto) {
            var result = await _projectService.CreateProject(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Add user to a project
        [HttpPost("{projectId:int}user/{userId}")]
        public async Task<IActionResult> AddUserToProject(int projectId, string userId) {
            var dto = new AddUserToProjectDto {
                ProjectId = projectId,
                AddUserId = userId
            };
            var result = await _projectService.AddUserToProject(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET API's
        // Get all projects
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var result = await _projectService.GetAllProjects();

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // Get project by Id
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetById(int projectId) {
            var dto = new GetProjectDto { ProjectId = projectId };
            var result = await _projectService.GetProjectById(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // UPDATE API's
        // Update project info
        [HttpPut]
        public async Task<IActionResult> UpdateProjectInfo([FromBody] UpdateProjectInfoDto dto, [FromQuery] int? projectId = null) {
            dto.ProjectId = projectId ?? dto.ProjectId;
            var result = await _projectService.UpdateProjectInfo(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // DELETE API's
        // Delete a project
        [HttpDelete]
        public async Task<IActionResult> DeleteProject([FromBody] GetProjectDto dto) {
            var result = await _projectService.DeleteProject(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Remove user from a project
        [HttpDelete("{projectId:int}/user/{userId}")]
        public async Task<IActionResult> RemoveUserFromProject(int projectId, string userId) {
            var dto = new GetProjectDto {
                ProjectId = projectId,
                UserId = userId
            };
            var result = await _projectService.RemoveUserFromProject(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
