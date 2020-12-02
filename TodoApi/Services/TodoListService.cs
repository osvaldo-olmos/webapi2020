using System.Threading.Tasks;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoListService : ITodoListService
    {
        private readonly TodoContext _context;

        public TodoListService(TodoContext context)
        {
            _context = context;
        }

        public async Task<TodoItemList> CreateTodoItemListAsync(NewTodoItemListDTO todoItemListDTO)
        {
            var todoItemList = new TodoItemList {
                Name = todoItemListDTO.Name
            };
            _context.TodoItemLists.Add(todoItemList);
            await _context.SaveChangesAsync();

            return todoItemList;
        }
    }
}