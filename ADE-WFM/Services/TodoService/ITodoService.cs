using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.TodoDtos;

namespace ADE_WFM.Services.TodoService
{
    public interface ITodoService
    {
        // ADD service
        Task AddTodo(AddTodoDto dto);


        // GET service
        Task<ServiceResult<List<ToDoResponseDto>>> GetAllUserTodos(GetToDoDto dto);
        Task<ServiceResult<List<ToDoResponseDto>>> GetAllProjectTodos(GetToDoDto dto);


        // UPDATE service
        Task UpdateTodoTitle(UpdateTodoTitleDto dto);
        Task UpdateTodoDescription(UpdateTodoDescriptionDto dto);
        Task UpdateTodoDueDate(UpdateTodoDueDateDto dto);


        // DELETE service
        Task DeleteTodo(DeleteTodoDto dto);


    }
}
