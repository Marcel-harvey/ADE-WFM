using ADE_WFM.Models.DTOs.SubTaskDtos;
using ADE_WFM.Models;

namespace ADE_WFM.Services.SubTaskService
{
    public interface ISubTaskService
    {
        // GET serives

        // ADD services
        Task AddSubTasksToTodo(AddSubTasksToTodoDto dto);

        // UPDATE services

        // DELETE serives
    }
}
