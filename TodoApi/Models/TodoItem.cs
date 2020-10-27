namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public string Secret  { get; set; }

        public long? TodoItemListId { get; set; }
        public TodoItemList TodoItemList  { get; set; }
    }
}