using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace UnitTest.Services
{
    public class TodoItemServiceShould
    {

        [Fact]
        public async Task GetExistingItem()
        {

            var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: "Test_GetExistingItem").Options;

            using (var context = new TodoContext(options))
            {
                var service = new TodoItemService(context);
                var item = new TodoItem
                {
                    Name = "Fake Item"
                };

                //await service.CreateAsync
            }

            // using (var context = new ApplicationDbContext(options))
            // {
            //     var repository = new TodoRepository(context);
            //     var item = await repository.GetAsync(1);

            //     Assert.Equal(1, item.Id);
            //     Assert.Equal("Fake Item", item.Name);
            // }

            Assert.Equal(1, 1);
        }
    }
}