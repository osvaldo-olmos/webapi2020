namespace TodoApi.Dtos
{
    public class NewTodoItemDTO
    {
        public string Name { get; set; }

        public bool IsComplete { get; set; }

        public string ResponsibleId { get; set; }
    }
}