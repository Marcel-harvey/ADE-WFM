using ADE_WFM.Models.DTOs.SubTaskDtos;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;

namespace ADE_WFM.Services.SubTaskService
{
    public interface ISubTaskService
    {
        // ADD services
        Task<ServiceResult<SubTaskResponseDto>> AddSubTasksToTodo(AddSubTasksToTodoDto dto);

        // GET serives
        Task<ServiceResult<List<SubTaskResponseDto>>> GetTodoSubTasks(GetSubTasksDto dto);

        // UPDATE services
        Task<ServiceResult<SubTaskResponseDto>> UpdateSubTask(UpdateSubTaskDto dto);
        Task<ServiceResult<SubTaskResponseDto>> MarkSubTaskCompletion(MarkSubTaskCompletionDto dto);

        // DELETE serives
        Task<ServiceResult<SubTaskResponseDto>> DeleteSubTask(GetSubTasksDto dto);
    }
}
