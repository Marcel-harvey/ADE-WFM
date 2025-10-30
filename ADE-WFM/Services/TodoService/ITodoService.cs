using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.TodoDtos;

namespace ADE_WFM.Services.TodoService
{
    public interface ITodoService
    {
        // ADD service
        Task<ServiceResult<ToDoResponseDto>> AddTodo(AddTodoDto dto);


        // GET service
        Task<ServiceResult<List<ToDoResponseDto>>> GetAllUserTodos(GetToDoDto dto);
        Task<ServiceResult<List<ToDoResponseDto>>> GetAllProjectTodos(GetToDoDto dto);


        // UPDATE service
        Task<ServiceResult<ToDoResponseDto>> UpdateTodo(UpdateTodoDto dto);
        Task UpdateTodoTitle(UpdateTodoTitleDto dto);
        Task UpdateTodoDescription(UpdateTodoDescriptionDto dto);
        Task UpdateTodoDueDate(UpdateTodoDueDateDto dto);


        // DELETE service
        Task DeleteTodo(DeleteTodoDto dto);


    }
}
