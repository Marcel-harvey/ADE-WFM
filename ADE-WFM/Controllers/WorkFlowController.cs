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
        [HttpPost]
        public async Task<IActionResult> CreateWorkFlow([FromBody] CreateWorkFlowDto dto)
        {
            var result = await _workFlowService.AddWorkFlow(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Add multiple users to a workflow
        [HttpPost("users/add")]
        public async Task<IActionResult> AddUsers([FromBody] AddUserWorkFlowDto dto)
        {
            var result = await _workFlowService.AddUserToWorkFlow(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET API's
        // Return all workflows
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _workFlowService.GetAllWorkFlows();

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // Return workflow by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = new GetWorkFlowInfoDto { WorkFlowId = id };
            var result = await _workFlowService.GetWorkFlowById(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // UPDATE API's
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateName(int id, [FromBody] UpdateWorkFlowNameDto dto)
        {
            if (id != dto.WorkFlowId)
                return BadRequest("Workflow ID in URL does not match body.");

            var result = await _workFlowService.UpdateWorkFlowName(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE API's
        // Delete a workflow via id
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new GetWorkFlowInfoDto { WorkFlowId = id };
            var result = await _workFlowService.DeleteWorkFlow(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Remove a user from a workflow
        [HttpDelete("{id:int}/users/{userId}")]
        public async Task<IActionResult> RemoveUser(int id, string userId)
        {
            var dto = new RemoveUserFromWorkFlowDto
            {
                WorkFlowId = id,
                UserId = userId
            };
            var result = await _workFlowService.RemoveUserFromWorkFlow(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
