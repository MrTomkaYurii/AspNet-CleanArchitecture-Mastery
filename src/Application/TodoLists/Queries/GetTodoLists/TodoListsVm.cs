namespace Application.TodoLists.Queries.GetTodoLists;

/// <summary>
/// ViewModel для відповіді на запит GetTodoListsQuery.
/// Містить як самі списки, так і довідкові дані (пріоритети).
/// </summary>
public class TodoListsVm
{
    public IList<PriorityLevelDto> PriorityLevels { get; set; } = new List<PriorityLevelDto>();
    public IList<TodoListDto> Lists { get; set; } = new List<TodoListDto>();
}
