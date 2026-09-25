using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

/// <summary>
/// Доменна подія: завдання виконано.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Спрацьовує, коли завдання переходить у статус Done = true.
/// Хендлер цієї події знаходиться в шарі Application (або Infrastructure),
/// що дозволяє ізолювати побічні ефекти (надсилання сповіщень, перерахунок статистики)
/// від самого агрегату завдань.
/// </summary>
public class TodoItemCompletedEvent : BaseEvent
{
    public TodoItemCompletedEvent(TodoItem item)
    {
        Item = item;
    }

    public TodoItem Item { get; }
}
