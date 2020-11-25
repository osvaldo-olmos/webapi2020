using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoItemService
    {
         Task<List<TodoItem>> GetAllAsync();
         Task<TodoItem> GetAsync(long id);

         Task UpdateAsync(long id, TodoItemDTO dto);
         Task<TodoItem> CreateAsync(NewTodoItemDTO dto, ApplicationUser appUser);

         Task DeleteAsync(long id);
    }
}