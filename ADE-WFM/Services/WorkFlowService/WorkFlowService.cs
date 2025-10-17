using ADE_WFM.Data;
using ADE_WFM.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace ADE_WFM.Services.WorkFlowService
{
    public class WorkFlowService : IWorkFlowService
    {
        private readonly ApplicationDbContext _context;

        public WorkFlowService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<WorkFlow>> GetAllWorkFlows()
        {
            var workFlow = await _context.WorkFlows.ToListAsync();

            return workFlow;
        }
    }
}
