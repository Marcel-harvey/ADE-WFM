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
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            var result = await _projectService.CreateProject(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Add user to a project
        [HttpPost("User/Add")]
        public async Task<IActionResult> AddUserToProject([FromBody] AddUserToProjectDto dto)
        {
            var result = await _projectService.AddUserToProject(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        // GET API's
        // Get all projects
        [HttpGet("Get/All")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _projectService.GetAllProjects();

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }


        // Get project by Id
        [HttpGet("Get/{projectId}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = new GetProjectDto { ProjectId = id };
            var result = await _projectService.GetProjectById(dto);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }


        // UPDATE API's
        // Update project info
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProjectInfo([FromBody] UpdateProjectInfoDto dto)
        {
            var result = await _projectService.UpdateProjectInfo(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        // DELETE API's
        // Delete a project
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteProject([FromBody] GetProjectDto dto)
        {
            var result = await _projectService.DeleteProject(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        // Remove user from a project
        [HttpDelete("User/Remove")]
        public async Task<IActionResult> RemoveUserFromProject([FromBody] GetProjectDto dto)
        {
            var result = await _projectService.RemoveUserFromProject(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
