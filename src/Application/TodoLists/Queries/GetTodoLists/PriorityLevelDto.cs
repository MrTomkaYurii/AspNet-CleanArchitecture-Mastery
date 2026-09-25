namespace Application.TodoLists.Queries.GetTodoLists;

/// <summary>
/// DTO для передачі списку доступних пріоритетів на фронтенд.
/// </summary>
public class PriorityLevelDto
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;
}
