using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/todolists")]
    [ApiController]
    public class TodoListController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoListController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoList
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemList>>> GetTodoItemLists()
        {
            return await _context.TodoItemLists.Include(x => x.TodoItems).ToListAsync();
        }

        // GET: api/TodoList/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemList>> GetTodoItemList(long id)
        {
            var todoItemList = await _context.TodoItemLists.FindAsync(id);

            if (todoItemList == null)
            {
                return NotFound();
            }

            await _context.Entry(todoItemList).Collection(x => x.TodoItems).LoadAsync();

            return todoItemList;
        }

        // PUT: api/TodoList/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItemList(long id, NewTodoItemListDTO todoItemListDTO)
        {

            var todoItemList = await _context.TodoItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound();
            }

            todoItemList.Name = todoItemListDTO.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemListExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/TodoList
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TodoItemList>> PostTodoItemList(NewTodoItemListDTO todoItemListDTO)
        {
            var todoItemList = new TodoItemList {
                Name = todoItemListDTO.Name
            };
            _context.TodoItemLists.Add(todoItemList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItemList", new { id = todoItemList.Id }, todoItemList);
        }

        // DELETE: api/TodoList/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItemList>> DeleteTodoItemList(long id)
        {
            var todoItemList = await _context.TodoItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound();
            }

            await _context.Entry(todoItemList).Collection(x => x.TodoItems).LoadAsync();
            if(todoItemList.TodoItems.Count !=0)
            {
                return BadRequest("Cannot delete a non empty list");
            }

            _context.TodoItemLists.Remove(todoItemList);
            await _context.SaveChangesAsync();

            return todoItemList;
        }

        [HttpPost("{id}/todos")]
        public async Task<ActionResult<TodoItemList>> PostTodoItemIntoList(long id, NewTodoItemDTO todoItemDTO)
        {
            var todoItemList = await _context.TodoItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound();
            }
            await _context.Entry(todoItemList).Collection(x => x.TodoItems).LoadAsync();

            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };

            todoItemList.TodoItems.Add(todoItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemListExists(id))
            {
                return NotFound();
            }

            return CreatedAtAction("PostTodoItemIntoList", new { id = todoItem.Id }, TodoController.ItemToDTO(todoItem));
        }

        [HttpPost("{id}/todos/{todoId}")]
        public async Task<IActionResult> AddTodoItemIntoList(long id, long todoId)
        {

            var todoItemList = await _context.TodoItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound("list not found");
            }

            var todoItem = await _context.TodoItems.FindAsync(todoId);
            if (todoItem == null)
            {
                return NotFound("todo item not found");
            }

            await _context.Entry(todoItemList).Collection(x => x.TodoItems).LoadAsync();
            todoItemList.TodoItems.Add(todoItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemListExists(id))
                {
                    return NotFound("list not found");
                }

                if (!TodoItemExists(todoId))
                {
                    return NotFound("todo item not found");
                }

            }

            return NoContent();
        }

        [HttpDelete("{id}/todos/{todoId}")]
        public async Task<IActionResult> RemoveTodoItemFromList(long id, long todoId)
        {

            var todoItemList = await _context.TodoItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound("list not found");
            }

            var todoItem = await _context.TodoItems.FindAsync(todoId);
            if (todoItem == null)
            {
                return NotFound("todo item not found");
            }

            await _context.Entry(todoItemList).Collection(x => x.TodoItems).LoadAsync();
            todoItemList.TodoItems.Remove(todoItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemListExists(id))
                {
                    return NotFound("list not found");
                }

                if (!TodoItemExists(todoId))
                {
                    return NotFound("todo item not found");
                }

            }

            return NoContent();
        }

        //TODO: Hacer una implementacion generica!
        private bool TodoItemListExists(long id)
        {
            return _context.TodoItemLists.Any(e => e.Id == id);
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
