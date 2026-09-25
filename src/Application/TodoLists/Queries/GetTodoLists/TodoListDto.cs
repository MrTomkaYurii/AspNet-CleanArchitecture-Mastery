namespace Application.TodoLists.Queries.GetTodoLists;

/// <summary>
/// DTO для передачі даних списку завдань разом із його елементами та Value Objects.
/// </summary>
public class TodoListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public string? Location { get; set; }
    public IList<TodoItemDto> Items { get; set; } = new List<TodoItemDto>();
}
