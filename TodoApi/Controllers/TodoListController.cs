using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
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
            return await _context.TodoItemLists.ToListAsync();
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

            if(todoItemList.TodoItems.Count !=0)
            {
                return BadRequest(); //TODO: Retornar un error message correcto.
            }

            _context.TodoItemLists.Remove(todoItemList);
            await _context.SaveChangesAsync();

            return todoItemList;
        }

        private bool TodoItemListExists(long id)
        {
            return _context.TodoItemLists.Any(e => e.Id == id);
        }
    }
}
