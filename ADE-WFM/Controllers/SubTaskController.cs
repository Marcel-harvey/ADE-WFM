using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.SubTaskDtos;
using ADE_WFM.Services.SubTaskService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTaskController : ControllerBase
    {
        private readonly ISubTaskService _subTaskService;
        public SubTaskController(ISubTaskService subTaskService)
        {
            _subTaskService = subTaskService;
        }
    }
}
