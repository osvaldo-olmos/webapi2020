using System.Threading.Tasks;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoListService
    {
         Task<TodoItemList> CreateTodoItemListAsync(NewTodoItemListDTO todoItemListDTO);
    }
}