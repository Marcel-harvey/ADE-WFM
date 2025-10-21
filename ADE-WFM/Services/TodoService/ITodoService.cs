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

        // UPDATE service

        // DELETE service


    }
}
