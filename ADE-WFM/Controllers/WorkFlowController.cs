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
        [HttpPost("Create")]
        public async Task<IActionResult> CreateWorkFlow([FromBody] CreateWorkFlowDto dto)
        {
            var result = await _workFlowService.AddWorkFlow(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        // Add multiple users to a workflow
        [HttpPost("User/Add")]
        public async Task<IActionResult> AddUsersToWorkFlow([FromBody] AddUserWorkFlowDto dto)
        {
            var result = await _workFlowService.AddUserToWorkFlow(dto);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        // GET API's
        // Return all workflows
        [HttpGet("Get/All")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _workFlowService.GetAllWorkFlows();

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }


        // Return workflow by ID
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetWorkFlowById(int id)
        {
            var dto = new GetWorkFlowInfoDto { WorkFlowId = id };
            var result = await _workFlowService.GetWorkFlowById(dto);

            if (!result.Succeeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }


        // UPDATE API's
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateWorkFlowName([FromBody] UpdateWorkFlowNameDto dto)
        {
            var result = await _workFlowService.UpdateWorkFlowName(dto);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);            
        }


        // DELETE API's
        // Delete a workflow via id
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteWorkFlow([FromBody] GetWorkFlowInfoDto dto)
        {
            var result = await _workFlowService.DeleteWorkFlow(dto);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        // Remove a user from a workflow
        [HttpDelete("User/Remove")]
        public async Task<IActionResult> RemoveUser([FromBody] RemoveUserFromWorkFlowDto dto)
        {
            var result = await _workFlowService.RemoveUserFromWorkFlow(dto);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
