using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.TodoDtos;

namespace ADE_WFM.Services.TodoService {
    public interface ITodoService {
        // ADD service
        Task<ServiceResult<ToDoResponseDto>> AddTodo(AddTodoDto dto);


        // GET service
        Task<ServiceResult<List<ToDoResponseDto>>> GetAllUserTodos();
        Task<ServiceResult<List<ToDoResponseDto>>> GetAllProjectTodos(GetToDoDto dto);


        // UPDATE service
        Task<ServiceResult<ToDoResponseDto>> UpdateTodo(UpdateTodoDto dto);
        Task<ServiceResult<ToDoResponseDto>> MarkTodoCompletion(MarkTodoCompletionDto dto);


        // DELETE service
        Task<ServiceResult<ToDoResponseDto>> DeleteTodo(GetToDoDto dto);


    }
}
