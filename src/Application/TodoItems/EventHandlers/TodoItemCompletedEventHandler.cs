using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.TodoItems.EventHandlers;

/// <summary>
/// Обробник доменної події: TodoItemCompletedEvent (завдання виконано).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Інтерфейс INotificationHandler у MediatR):
/// 
/// 1. Що таке `INotificationHandler<TNotification>`?
///    Це інтерфейс підписника (Subscriber / Listener) на певну доменну подію TNotification.
///    Він автоматично виявляється та реєструється за допомогою `cfg.RegisterServicesFromAssembly(assembly)`.
/// 
/// 2. Сигнатура методу Handle:
///    `Task Handle(TNotification notification, CancellationToken cancellationToken)`
///    
///    Параметри:
///    - `notification` — об'єкт події, що містить посилання на сутність (у нас TodoItem) та час виникнення.
///    - `cancellationToken` — токен для безпечного переривання асинхронних операцій.
/// 
/// 3. Можливість мати декілька хендлерів (One-to-Many):
///    Для однієї події `TodoItemCompletedEvent` ми можемо створити ОДРАЗУ КІЛЬКА хендлерів:
///    - `TodoItemCompletedEventHandler` (записує в лог);
///    - `SendEmailNotificationHandler` (надсилає листа автору завдання);
///    - `UpdateUserGamificationPointsHandler` (додає бали користувачу за виконане завдання).
///    Кожен такий клас живе в окремому файлі, не знає про інші хендлери та не порушує Single Responsibility Principle!
/// </summary>
public class TodoItemCompletedEventHandler : INotificationHandler<TodoItemCompletedEvent>
{
    private readonly ILogger<TodoItemCompletedEventHandler> _logger;

    public TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Метод, який автоматично викликається MediatR при виклику `_mediator.Publish(new TodoItemCompletedEvent(...))`.
    /// </summary>
    public Task Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "ДОМЕННА ПОДІЯ: Завдання \"{Title}\" (Id: {Id}) позначено як виконане! Час: {DateOccurred}",
            notification.Item.Title,
            notification.Item.Id,
            notification.DateOccurredUtc);

        return Task.CompletedTask;
    }
}
