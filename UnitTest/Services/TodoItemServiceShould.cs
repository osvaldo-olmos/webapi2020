using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace UnitTest.Services
{
    public class TodoItemServiceShould
    {
        [Fact]
        public async Task CreateItemAndGetItOK()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: "Test_CreateItemAndGetItOK").Options;

            using (var context = new TodoContext(options))
            {
                var service = new TodoItemService(context);
                var item = new NewTodoItemDTO
                {
                    Name = "Fake Item",
                    IsComplete = true
                };

                var appUser = new ApplicationUser
                {
                    Id ="fakeId"
                };

                var todoItem = await service.CreateAsync(item, appUser);
            }

            using (var context = new TodoContext(options))
            {
                var service = new TodoItemService(context);
                var item = await service.GetAsync(1L);

                Assert.Equal("Fake Item", item.Name);
                Assert.Equal("fakeId", item.Responsible.Id);
                Assert.False(item.IsComplete);
            }
        }

        [Fact]
        public async Task CreateWithItemIsIncomplete()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: "Test_CreateTodoItem").Options;

            using (var context = new TodoContext(options))
            {
                var service = new TodoItemService(context);
                var item = new NewTodoItemDTO
                {
                    Name = "Fake Item",
                    IsComplete = true
                };

                var appUser = new ApplicationUser
                {
                    Id ="fakeId"
                };

                var todoItem = await service.CreateAsync(item, appUser);

                Assert.False(todoItem.IsComplete);
            }
        }
    }
}