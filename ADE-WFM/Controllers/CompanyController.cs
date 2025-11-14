using ADE_WFM.Models.DTOs.CompanyDtos;
using ADE_WFM.Services.CompanyService;
using Microsoft.AspNetCore.Mvc;

namespace ADE_WFM.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService) {
            _companyService = companyService;
        }

        // CREATE
        // Create a new tenant/company
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewTenant([FromBody] CreateTenantDto dto) {
            var result = await _companyService.CreateTenant(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Create an invite link - invite user to Tenant work flow
        [HttpGet("Invite")]
        public async Task<IActionResult> CreateInviteLink([FromQuery] InviteToTenantDto dto) {
            var result = await _companyService.CreateTenantInvite(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET
        [HttpPost("accept-invite")]
        public async Task<IActionResult> AcceptTokenInvite([FromBody] InviteTokenDto dto) {
            var result = await _companyService.AcceptTenantInvite(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // UPDATE
        // Update company infor
        [HttpPut]
        public async Task<IActionResult> UpdateCompanyInfo([FromBody] TenantInfoDto dto) {
            var result = await _companyService.UpdateTenantConnection(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // DELETE
        // Remove a company
        [HttpDelete]
        public async Task<IActionResult> DeleteTenant([FromBody] TenantInfoDto dto) {
            var result = await _companyService.DeleteTenant(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
