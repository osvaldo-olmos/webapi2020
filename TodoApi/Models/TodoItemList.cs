using System.Collections.Generic;

namespace TodoApi.Models
{
    public class TodoItemList
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<TodoItem> TodoItems { get; set; }
    }
}