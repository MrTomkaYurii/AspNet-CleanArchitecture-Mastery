namespace Domain.Common;

/// <summary>
/// Базова доменна подія (Domain Event) у чистій архітектурі.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Інтерфейс INotification у MediatR):
/// 
/// 1. Що таке інтерфейс `MediatR.INotification`?
///    Це маркерний інтерфейс для подій та сповіщень у системі.
///    Він реалізує патерн Publisher/Subscriber (Видавець / Підписник):
///    - Подія публікується через `IPublisher.Publish(notification)` або `IMediator.Publish(notification)`.
///    - На одну подію `INotification` може бути зареєстровано:
///      * 0 обробників (подія просто ігнорується, якщо ніхто не підписаний);
///      * 1 обробник;
///      * 5, 10 чи більше незалежних обробників (`INotificationHandler<T>`), які виконуються послідовно або паралельно!
/// 
/// 2. Порівняння: `IRequest` проти `INotification`:
///    | Критерій | `IRequest<TResponse>` | `INotification` |
///    | :--- | :--- | :--- |
///    | **Патерн** | Point-to-Point (Команда / Запит) | Publish / Subscribe (Доменна подія) |
///    | **Кількість хендлерів** | РІВНО 1 (інакше помилка) | 0, 1 або багато (N) |
///    | **Результат (return)** | Повертає значення TResponse | Нічого не повертає (Task/void) |
///    | **Метод виклику** | `_sender.Send(request)` | `_publisher.Publish(notification)` |
/// 
/// 3. Чому шар Domain посилається на `MediatR.Contracts`?
///    Пакет `MediatR.Contracts` є надлегким (містить лише інтерфейси, 0 сторонніх залежностей).
///    Це дозволяє сутностям Domain генерувати події, які шар Application зможе безпосередньо обробляти
///    через стандартні хендлери MediatR.
/// </summary>
public abstract class BaseEvent : MediatR.INotification
{
    /// <summary>
    /// Дата та час виникнення події у форматі UTC.
    /// </summary>
    public DateTime DateOccurredUtc { get; protected set; } = DateTime.UtcNow;
}
