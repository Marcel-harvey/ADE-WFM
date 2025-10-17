using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.CompanyService
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get list of all companies ONLY 
        public async Task<List<WorkFlow>> GetAllWorkFlows()
        {
            var companies = await _context.WorkFlows
                                .Include(project => project.Project)
                                .Include(comment =>  comment.Comment)
                                .Include(user => user.WorkFlowUsers)
                                .ToListAsync();

            return companies; 
        }

        // Get company By id
        public WorkFlow? GetWorkFlowName(int id)
        {
            return  _context.WorkFlows.FirstOrDefault(c => c.Id == id);
        }

        // Create new company Name
        public async Task AddWorkFlow(WorkFlow company)
        {
            await _context.WorkFlows.AddAsync(company);
            _context.SaveChanges();
        }

        // Update company name
        public async Task UpdateWorkFlow(WorkFlow copany)
        {
            await _context.WorkFlows.FindAsync(copany.Id);
            _context.SaveChanges();
        }

        // Delete Company name
        public async Task DeleteWorkFlow(int id)
        {
            await _context.WorkFlows.FindAsync(id);
        }
    }
}
