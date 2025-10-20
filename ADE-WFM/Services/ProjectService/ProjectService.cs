using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels.ProjectViewModels;
using ADE_WFM.Models.DTOs.ProjectDtos;
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
                .FirstOrDefaultAsync(p => p.Id == projectId)
                ?? throw new KeyNotFoundException($"Project with ID: {projectId} was not found");

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

        // ADD services

        // DELETE services


        // UPDATE API services
        // Update project title
        public async Task<Project> UpdateProjectTitle(UpdateProjectTitleViewModel model)
        {
            var project = await _context.Projects
                .FindAsync(model.projectId) 
                ?? throw new KeyNotFoundException($"Project with ID: {model.projectId} was not found");
                        
            // Check if the project title is not empty
            if (string.IsNullOrWhiteSpace(model.newProjectTitle))
            {
                throw new ArgumentException("Project title cannot be empty", nameof(model.newProjectTitle));
            }

            project.ProjectTitle = model.newProjectTitle;
            await _context.SaveChangesAsync();

            return project;
        }


        // Update project description
        public async Task<Project> UpdateProjectDescription(UpdateProjectDescriptionViewModel model)
        {
            var project = await _context.Projects
                .FindAsync(model.projectId)
                ?? throw new KeyNotFoundException($"Project with ID: {model.projectId} was not found");

            // Check if the new project description is not null
            if (model.newProjectDescription == null)
            {
                throw new ArgumentNullException(nameof(model.newProjectDescription), "Project description cannot be null");
            }

            project.ProjectDescription = model.newProjectDescription;
            await _context.SaveChangesAsync();

            return project;
        }


        // Update project due date
        public async Task<Project> UpdateProjectDueDate(UpdateProjectDueDateViewModel model)
        {
            var project = await _context.Projects
                .FindAsync(model.ProjectId)
                ?? throw new KeyNotFoundException($"Project with ID: {model.ProjectId} was not found");

            // Check if the new due date is not in the past
            if (model.NewDueDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException("Due date cannot be in the past", nameof(model.NewDueDate));
            }

            project.DueDate = model.NewDueDate;
            await _context.SaveChangesAsync();

            return project;
        }


        // ADD API services
        public async Task<ApplicationUser> AddUserToProject(AddUserProjectViewModel model)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectUsers)
                .FirstOrDefaultAsync(p => p.Id == model.ProjectId)
                ?? throw new KeyNotFoundException($"Project with ID: {model.ProjectId} was not found");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == model.UserId)
                ?? throw new KeyNotFoundException($"User with ID: {model.UserId} was not found");

            // Check if the user is already in the project
            if (project.ProjectUsers.Any(pu => pu.UserId == model.UserId))
            {
                throw new InvalidOperationException($"User with ID: {model.UserId} is already in the project");
            }

            var projectUser = new ProjectUser
            {
                ProjectId = model.ProjectId,
                UserId = model.UserId
            };

            await _context.ProjectUsers.AddAsync(projectUser);
            await _context.SaveChangesAsync();

            return user;
        }


        // DELETE API services
        public async Task<Project> DeleteProject(DeleteProjectDto dto)
        {
            // Check if the dto is null
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "DeleteProjectDto cannot be null");
            }

            // Find the project to be deleted
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId)
                ?? throw new KeyNotFoundException($"Project with ID: {dto.ProjectId} was not found");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return project;
        }
    }
}
