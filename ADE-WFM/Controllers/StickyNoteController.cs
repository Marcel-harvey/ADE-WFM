using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.StickyNoteDto;
using ADE_WFM.Services.StickyNoteService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StickyNoteController : ControllerBase
    {
        private readonly IStickyNoteService _stickyNoteService;
        public StickyNoteController(IStickyNoteService stickyNoteService)
        {
            _stickyNoteService = stickyNoteService;
        }

    }
}
