using ADE_WFM.Models;

namespace ADE_WFM.Services.WorkFlowService
{
    public interface IWorkFlowService
    {
        Task<List<WorkFlow>> GetAllWorkFlows();
    }
}
