using Domain.Common;
using Domain.Enums;
using Domain.Events;

namespace Domain.Entities;

/// <summary>
/// Сутність елемента завдання (TodoItem).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Інкапсуляція бізнес-правил у моделі):
/// Замість "анемічної" моделі (де всі поля мають публічний setter), багата доменна модель
/// містить методи для зміни стану, що захищають бізнес-інваріанти.
/// Наприклад, метод MarkComplete() не просто змінює прапорець Done,
/// а й реєструє доменну подію TodoItemCompletedEvent.
/// </summary>
public class TodoItem : BaseAuditableEntity
{
    /// <summary>
    /// Зовнішній ключ до списку-власника.
    /// </summary>
    public int ListId { get; set; }

    /// <summary>
    /// Навігаційна властивість до списку завдань.
    /// </summary>
    public TodoList? List { get; set; }

    /// <summary>
    /// Заголовок або назва завдання.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Детальний опис або примітка до завдання.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Пріоритетність виконання завдання.
    /// </summary>
    public PriorityLevel Priority { get; set; } = PriorityLevel.None;

    /// <summary>
    /// Дата та час нагадування про завдання.
    /// </summary>
    public DateTime? Reminder { get; set; }

    private bool _done;

    /// <summary>
    /// Статус виконання завдання.
    /// </summary>
    public bool Done
    {
        get => _done;
        set
        {
            if (value && !_done)
            {
                // Якщо завдання було невиконаним і позначається виконаним - генеруємо доменну подію
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    /// <summary>
    /// Бізнес-метод для явного позначення завдання виконаним.
    /// </summary>
    public void MarkComplete()
    {
        Done = true;
    }
}
