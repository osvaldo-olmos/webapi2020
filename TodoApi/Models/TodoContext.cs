using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoContext : IdentityDbContext<ApplicationUser>
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoItemList> TodoItemLists { get; set; }
    }
}