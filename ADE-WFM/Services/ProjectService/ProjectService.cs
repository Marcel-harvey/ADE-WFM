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
        // Get all projects
        public async Task<List<Project>> GetAllProjects()
        {
            var projects = await _context.Projects
                .Include(workFlow => workFlow.WorkFlowId)
                .Include(user => user.Users)
                .ToListAsync();

            return projects;
        }


        // Get project by id
        public async Task<Project> GetProjectById(int projectId)
        {
            var project = await _context.Projects
                .Include(workFlow => workFlow.WorkFlowId)
                .Include(user => user.Users)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID: {projectId} was not found");
            }

            return project;
        }


        // Get all users involved in project
        public async Task<List<ApplicationUser>> GetUsersInProject(int projectId)
        {
            var projectUsers = await _context.ProjectUsers
                .Where(project => project.ProjectId == projectId)
                .Select(user => user.User)
                .ToListAsync();

            return projectUsers;
        }

        // UPDATE services
        // Update project title
        public async Task<Project> UpdateProjectTitle(UpdateProjectTitleViewModel model)
        {
            // Check if the project title is not empty
            if (string.IsNullOrWhiteSpace(model.newProjectTitle))
            {
                throw new ArgumentException("Project title cannot be empty", nameof(model.newProjectTitle));
            }

            var project = await _context.Projects
                .FindAsync(model.projectId);

            // Confirm that the project exists
            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID: {model.projectId} was not found");
            }

            project.ProjectTitle = model.newProjectTitle;
            await _context.SaveChangesAsync();

            return project;
        }

        // ADD services

        // DELETE services
    }
}
