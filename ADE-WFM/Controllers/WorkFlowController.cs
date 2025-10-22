using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Services.WorkFlowService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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


        // GET API's
        // Return all workflows
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var workflows = await _workFlowService.GetAllWorkFlows();

            return Ok(workflows);
        }


        // Return workflow by ID
        [HttpGet("{id}")]
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
    }
}
