namespace Application.TodoItems.Queries.GetTodoItemsWithPagination;

/// <summary>
/// Коротке DTO для елемента завдання у пагінованому списку.
/// </summary>
public class TodoItemBriefDto
{
    public int Id { get; set; }
    public int ListId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Done { get; set; }
}
