using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.ViewModels;

namespace ADE_WFM.Services.CompanyService
{
    public interface ICompanyService
    {
        Task<List<WorkFlow>> GetAllWorkFlows();
        WorkFlow? GetWorkFlowName(int Id);
        Task AddWorkFlow(WorkFlow workflow);
        Task UpdateWorkFlow(WorkFlow workflow);
        Task DeleteWorkFlow(int id);
    }
}
