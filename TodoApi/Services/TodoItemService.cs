using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly TodoContext _context;

        public TodoItemService(TodoContext context)
        {
            _context = context;
        }

        public async Task<List<TodoItem>> GetAllAsync()
        {
            return await _context.TodoItems.ToListAsync();
        }

        public async Task<TodoItem> GetAsync(long id)
        {
            return await _context.TodoItems.Include(i => i.Responsible).
                            FirstOrDefaultAsync( i => i.Id ==id);
        }

        public async Task UpdateAsync(long id, TodoItemDTO dto)
        {
            var item = await _context.TodoItems.FindAsync(id);

            if(item is null)
            {
                throw new ArgumentException("item not found");
            }
            
            item.Name = dto.Name;
            item.IsComplete = dto.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                throw new InvalidOperationException($"item {id} has been deleted");
            }
        }

        public async Task<TodoItem> CreateAsync(NewTodoItemDTO dto, ApplicationUser appUser)
        {
            var todoItem = new TodoItem
            {
                IsComplete = false,
                Name = dto.Name,
                Responsible = appUser
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }
        public async Task DeleteAsync(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                throw new ArgumentException("item not found");
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}