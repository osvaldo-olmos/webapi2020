using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/todos")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ITodoItemService _todoItemService;

        public TodoController(UserManager<ApplicationUser> userManager,
                                ITodoItemService todoItemService)
        {
            _userManager =userManager;
            _todoItemService =todoItemService;
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            var todoItems = await _todoItemService.GetAllAsync();
            return todoItems.Select(item => ItemToDTO(item)).ToList();
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _todoItemService.GetAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }


            return ItemToDTO(todoItem);
        }

        // PUT: api/Todo/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

            try
            {
                await _todoItemService.UpdateAsync(id, todoItemDTO);            
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }

        // POST: api/Todo
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(NewTodoItemDTO todoItemDTO)
        {
            ApplicationUser appUser =null;
            
            if(todoItemDTO.UserId !=null)
            {
                appUser = await _userManager.FindByIdAsync(todoItemDTO.UserId);

                if(appUser is null)
                {
                    return BadRequest("Invalid userId");
                }

            }

            var todoItem = await _todoItemService.CreateAsync(todoItemDTO, appUser);

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, ItemToDTO(todoItem));
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItemDTO>> DeleteTodoItem(long id)
        {
            try
            {
                await _todoItemService.DeleteAsync(id);            
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }

        public static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
                new TodoItemDTO
                {
                    Id = todoItem.Id,
                    Name = todoItem.Name,
                    IsComplete = todoItem.IsComplete,
                    Responsible = todoItem.Responsible.Email
                };
    }
}
