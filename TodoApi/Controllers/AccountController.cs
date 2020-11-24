using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/users")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly TodoContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            TodoContext context
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context =context;
        }
       
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email, 
                Email = dto.Email
            };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return GenerateJwtToken(dto.Email, user);

                //TODO should I return CreatedAtAction ???
            }else
            {
                throw new ApplicationException("UNKNOWN_ERROR"); //TODO resolver con un retorno de error correcto
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDTO dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);
            
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == dto.Email);
                return GenerateJwtToken(dto.Email, appUser);
            }else
            {
                throw new ApplicationException("Invalid Login"); //TODO resolver con un retorno de error correcto
            }
        }

        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/todos")]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItemsByUser(string id)
        {
            return await _context.TodoItems.Where(i => i.Responsible.Id == id)
                                            .Select(item => ItemToDTO(item)).ToListAsync();
        }

        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{id}/todos/{todoId}")]
        public async Task<IActionResult> AssignResponsibleToItem(string id, long todoId)
        {

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return NotFound("user not found");
            }

            var todoItem = await _context.TodoItems.FindAsync(todoId);
            if (todoItem == null)
            {
                return NotFound("todo item not found");
            }

            todoItem.Responsible =appUser;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(todoId))
                {
                    return NotFound("todo item not found");
                }

            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}/todos/{todoId}")]
        public async Task<IActionResult> UnnassignResponsibleFromItem(string id, long todoId)
        {

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return NotFound("user not found");
            }

            var todoItem = await _context.TodoItems.Include(i => i.Responsible).FirstOrDefaultAsync(t => t.Id == todoId);
            if (todoItem == null)
            {
                return NotFound("todo item not found");
            }

            if(!appUser.Equals(todoItem.Responsible))
            {
                return NotFound("item does not belong to user");
            }

            todoItem.Responsible =null;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(todoId))
                {
                    return NotFound("todo item not found");
                }

            }

            return NoContent();
        }


        public static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
                new TodoItemDTO
                {
                    Id = todoItem.Id,
                    Name = todoItem.Name,
                    IsComplete = todoItem.IsComplete
                };
        



        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }


        private string GenerateJwtToken(string email, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}