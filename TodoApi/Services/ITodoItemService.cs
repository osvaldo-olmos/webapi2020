using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoItemService
    {
         Task<List<TodoItem>> GetAllAsync();
    }
}