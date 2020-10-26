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
        public async Task<ActionResult<IEnumerable<TodoItemListDTO>>> GetTodoItemLists()
        {
            return await _context.TodoItemLists.Select(x => TodoListToDTO(x)).ToListAsync();
        }

        // GET: api/TodoList/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemListDTO>> GetTodoItemList(long id)
        {
            var todoItemList = await _context.TodoItemLists.FindAsync(id);

            if (todoItemList == null)
            {
                return NotFound();
            }

            return TodoListToDTO(todoItemList);
        }

        // PUT: api/TodoList/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItemList(long id, TodoItemListDTO todoItemListDTO)
        {
            if (id != todoItemListDTO.Id)
            {
                return BadRequest();
            }

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
        public async Task<ActionResult<TodoItemListDTO>> PostTodoItemList(NewTodoItemListDTO newListDTO)
        {
            var todoItemList = new TodoItemList
            {
                Name = newListDTO.Name
            };
            _context.TodoItemLists.Add(todoItemList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItemList", new { id = todoItemList.Id }, TodoListToDTO(todoItemList));
        }

        // DELETE: api/TodoList/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItemListDTO>> DeleteTodoItemList(long id)
        {
            var todoItemList = await _context.TodoItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound();
            }

            _context.TodoItemLists.Remove(todoItemList);
            await _context.SaveChangesAsync();

            return TodoListToDTO(todoItemList);
        }

        private bool TodoItemListExists(long id)
        {
            return _context.TodoItemLists.Any(e => e.Id == id);
        }

        private static TodoItemListDTO TodoListToDTO(TodoItemList todoItemList) =>
                new TodoItemListDTO
                {
                    Id = todoItemList.Id,
                    Name = todoItemList.Name,
                };
    }
}
