using ADE_WFM.Models.DTOs;
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
        [HttpPost("Create-new")]
        public async Task<IActionResult> CreateWorkFlow([FromBody] CreateWorkFlowDto dto)
        {
            var result = await _workFlowService.AddWorkFlow(dto);

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);
        }


        // Add multiple users to a workflow
        [HttpPost("Add-users")]
        public async Task<IActionResult> AddUsersToWorkFlow([FromBody] AddUserWorkFlowDto dto)
        {
            var result = await _workFlowService.AddUserToWorkFlow(dto);

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);
        }


        // GET API's
        // Return all workflows
        [HttpGet("Get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _workFlowService.GetAllWorkFlows();
            if (!result.Succeeded)
                return NotFound(result.Message);

            return Ok(result);
        }


        // Return workflow by ID
        [HttpPost("Get-by-id")]
        public async Task<IActionResult> GetWorkFlowById([FromBody] GetWorkFlowByIdDto dto)
        {
            try
            {
                var result = await _workFlowService.GetWorkFlowById(dto);

                if (!result.Succeeded)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<GetAllWorkFlowsDtoResponse>.Failure(
                    "An unexpected error occurred while retrieving the workflow.",
                    new[] { ex.Message }
                ));
            }
        }


        // UPDATE API's
        [HttpPut("Update-name")]
        public async Task<IActionResult> UpdateWorkFlowName([FromBody] UpdateWorkFlowNameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult<object>.Failure(
                    "Invalid data provided.",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                ));

            try
            {
                var result = await _workFlowService.UpdateWorkFlowName(dto);

                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ServiceResult<object>.Failure(
                    "An unexpected error occurred while updating the workflow name.",
                    new[] { ex.Message }
                ));
            }
        }


        // DELETE API's
        // Delete a workflow via id
        [HttpDelete("Delete-work-flow")]
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
            catch (Exception)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while deleting the workflow." });
            }
        }


        // Remove a user from a workflow
        [HttpDelete("Remove-user-from-work-flow")]
        public async Task<IActionResult> RemoveUserFromWorkFlow([FromBody] RemoveUserFromWorkFlowDto dto)
        {
            if (dto == null || dto.WorkFlowId <= 0 || dto.UserId == null)
                return BadRequest("Invalid workflow ID.");

            try
            {
                var result = await _workFlowService.RemoveUserFromWorkFlow(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while removing the user from your work flow." });
            }

        }



    }
}
