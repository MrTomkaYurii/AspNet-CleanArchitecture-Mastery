namespace Application.TodoLists.Queries.GetTodoLists;

/// <summary>
/// Data Transfer Object (DTO) для елемента завдання в структурі списку.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (DTO vs Entity):
/// Ми ніколи не повертаємо сутності Domain (наприклад, TodoItem) напряму клієнту через API!
/// Чому?
/// 1. Безпека: DTO приховує внутрішні поля та структуру БД.
/// 2. Оптимізація: DTO містить лише ті поля, які потрібні конкретному UI екрану.
/// 3. Відсутність циклічних посилань: Entity Framework містить навігаційні зв'язки (Item -> List -> Items),
///    що призводить до помилок серіалізації в JSON.
/// </summary>
public class TodoItemDto
{
    public int Id { get; set; }
    public int ListId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public bool Done { get; set; }
    public int Priority { get; set; }
}
