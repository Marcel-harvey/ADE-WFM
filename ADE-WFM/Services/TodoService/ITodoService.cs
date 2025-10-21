using ADE_WFM.Models;
using ADE_WFM.Models.DTOs.TodoDtos;

namespace ADE_WFM.Services.TodoService
{
    public interface ITodoService
    {
        // GET service
        Task<List<Todo>> GetAllTodos();
        Task<Todo> GetTodoById(GetTodoByIdDto dto);
        Task<List<Todo>> GetAllUserTodos(GetAllUserTodoDto dto);


        // ADD service
        Task AddTodo(AddTodoDto dto);


        // UPDATE service
        Task UpdateTodoTitle(UpdateTodoTitleDto dto);
        Task UpdateTodoDescription(UpdateTodoDescriptionDto dto);
        Task UpdateTodoDueDate(UpdateTodoDueDateDto dto);


        // DELETE service


    }
}
