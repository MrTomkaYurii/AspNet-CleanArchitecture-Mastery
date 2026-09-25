using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common;

/// <summary>
/// Базовий клас для всіх сутностей (Entities) у предметній області.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (DDD - Domain Driven Design):
/// 1. Сутність (Entity) визначається своєю унікальною ідентичністю (Id), а не набором властивостей.
///    Навіть якщо всі властивості двох об'єктів збігаються, але їхні Id різні — це дві різні сутності.
/// 2. Сутність також є джерелом доменних подій (Domain Events). Коли в сутності відбувається
///    важлива бізнес-зміна (наприклад, завдання завершено), вона реєструє подію, яка буде
///    опублікована після успішного збереження змін у базі даних (Unit of Work).
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Унікальний ідентифікатор сутності.
    /// За замовчуванням використовуємо цілочисельний int або Guid.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Внутрішній список доменних подій, що накопичуються в процесі виконання бізнес-логіки.
    /// </summary>
    private readonly List<BaseEvent> _domainEvents = new();

    /// <summary>
    /// Публічна колекція доменних подій, доступна лише для читання.
    /// Атрибут [NotMapped] вказує Entity Framework, що це поле не є колонкою в базі даних.
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Додавання нової доменної події.
    /// </summary>
    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Видалення доменної події.
    /// </summary>
    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Очищення списку доменних подій після їх успішної публікації.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
