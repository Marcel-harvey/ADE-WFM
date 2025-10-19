using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels.ProjectViewModels;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.ProjectService
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET services
        public async Task<List<Project>> GetAllProjects()
        {
            var projects = await _context.Projects
                .Include(workFlow => workFlow.WorkFlowId)
                .Include(user => user.Users)
                .ToListAsync();

            return projects;
        }

        // UPDATE services

        // ADD services

        // DELETE services
    }
}
