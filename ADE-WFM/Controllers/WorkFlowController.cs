using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;
using ADE_WFM.Services.WorkFlowService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkFlowController : ControllerBase
    {
        private readonly IWorkFlowService _workFlowService;
        public WorkFlowController(IWorkFlowService workFlowService)
        {
            _workFlowService = workFlowService;
        }


        // CREATE API's
        // Create a new workflow
        [HttpPost("Create-new-work-flow")]
        public async Task<IActionResult> CreateNewWorkFlow([FromBody] CreateWorkFlowDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _workFlowService.AddWorkFlow(dto);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                // Database update errors
                return StatusCode(500, new { Message = "Database error occurred.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                // Catch-all for unexpected errors
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }


        // Add multiple users to a workflow
        [HttpPost("add-users")]
        public async Task<IActionResult> AddUsersToWorkFlow([FromBody] AddUserWorkFlowDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _workFlowService.AddUserToWorkFlow(dto);

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                // Thrown when workflow or user not found
                return NotFound(new { Message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                // Handle DB-related issues like constraint violations
                return StatusCode(500, new { Message = "Database error occurred.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                // Fallback for unexpected exceptions
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }


        // GET API's
        // Return all workflows
        [HttpGet("Get-all-work-flows")]
        public async Task<IActionResult> GetAll()
        {
            var workflows = await _workFlowService.GetAllWorkFlows();

            return Ok(workflows);
        }


        // Return workflow by ID
        [HttpGet("get-work-flow-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var dto = new GetWorkFlowByIdDto { Id = id };
                var workflow = await _workFlowService.GetWorkFlowById(dto);
                return Ok(workflow);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An unexpected error occurred. {ex.Message}" });
            }
        }


        // UPDATE API's
        [HttpPut("update-name")]
        public async Task<IActionResult> UpdateWorkFlowName([FromBody] UpdateWorkFlowNameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _workFlowService.UpdateWorkFlowName(dto);
                return Ok(new
                {
                    Data = response
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Workflow not found
                return NotFound(new { Message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                // Database save issues
                return StatusCode(500, new { Message = "Database error occurred while updating workflow.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                // Any other unhandled exceptions
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }


        // DELETE API's
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteWorkFlow([FromBody] DeleteWorkFlowDto dto)
        {
            if (dto == null || dto.Id <= 0)
                return BadRequest("Invalid workflow ID.");

            try
            {
                var result = await _workFlowService.DeleteWorkFlow(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the workflow." });
            }
        }


    }
}
