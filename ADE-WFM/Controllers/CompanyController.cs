using ADE_WFM.Models.DTOs.CompanyDtos;
using ADE_WFM.Models.DTOs.UserDtos;
using ADE_WFM.Services.CompanyService;
using ADE_WFM.Services.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // CREATE
        // Create a new tenant/company
        [HttpPost]
        public async Task<IActionResult> CreateNewTenant([FromBody] CreateTenantDto dto)
        {
            var result = await _companyService.CreateTenant(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // Create an invite link - invite user to Tenant work flow
        [HttpGet("Invite")]
        public async Task<IActionResult> CreateInviteLink([FromQuery] InviteToTenantDto dto)
        {
            var result = await _companyService.CreateTenantInvite(dto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        // GET


        // UPDATE


        // DELETE
    }
}
