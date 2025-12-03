using ADE_WFM.Models.DTOs.WorkFlowDtos;
using ADE_WFM.Models.DTOs.WorkFlowViewModels;
using ADE_WFM.Services.WorkFlowService;
using Microsoft.AspNetCore.Mvc;

namespace ADE_WFM.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController : ControllerBase {
        private readonly IProgramService _workFlowService;
        public ProgramController(IProgramService workFlowService) {
            _workFlowService = workFlowService;
        }


        // CREATE API's
        // Create a new workflow
        [HttpPost]
        public async Task<IActionResult> CreateProgram([FromBody] CreateProgramDto dto) {
            var result = await _workFlowService.AddProgram(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Add multiple users to a workflow
        [HttpPost("users")]
        public async Task<IActionResult> AddUsers([FromBody] AddUserProgramDto dto) {
            var result = await _workFlowService.AddUserToProgram(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET API's
        // Return all workflows
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var result = await _workFlowService.GetAllPrograms();

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // Return workflow by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) {
            var dto = new GetProgramInfoDto { ProgramId = id };
            var result = await _workFlowService.GetProgramById(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }

        // Return program details
        [HttpGet("details")]
        public async Task<IActionResult> GetProgramDetails([FromQuery] GetProgramInfoDto dto) {
            var result = await _workFlowService.GetProgramDetails(dto);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        // UPDATE API's
        [HttpPut]
        public async Task<IActionResult> UpdateProgram([FromBody] UpdateProgramNameDto dto) {
            var result = await _workFlowService.UpdateProgram(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE API's
        // Delete a workflow via id
        [HttpDelete("{programId:int}")]
        public async Task<IActionResult> Delete(int programId) {
            var dto = new GetProgramInfoDto { ProgramId = programId };
            var result = await _workFlowService.DeleteProgram(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Remove a user from a workflow
        [HttpDelete("users")]
        public async Task<IActionResult> RemoveUser([FromBody] AddUserProgramDto dto) {
            var result = await _workFlowService.RemoveUserFromProgram(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
