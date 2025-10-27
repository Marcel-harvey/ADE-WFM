using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;
using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Services.ProjectService;
using ADE_WFM.Services.WorkFlowService;
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
        public async Task<IActionResult> CreatePorject([FromBody] CreateProjectDto dto)
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


        // GET API's
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
            catch(Exception ex)
            {
                return StatusCode(500, ServiceResult<GetProjectByIdResponseDto>.Failure(
                    "An unexpected error occurred while retrieving the project.",
                    new[] { ex.Message }
                ));
            }
        }

        // UPDATE API's

        // DELETE API's

    }
}
