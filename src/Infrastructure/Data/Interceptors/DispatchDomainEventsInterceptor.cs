using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors;

/// <summary>
/// EF Core Interceptor для автоматичної публікації доменних подій.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Метод IMediator.Publish):
/// 
/// 1. Що таке `IMediator` у цьому перехоплювачі?
///    `IMediator` реалізує `IPublisher` (інтерфейс з методом `Publish`).
///    Ми інжектуємо IMediator сюди, щоб сповістити всю систему про події, які накопичилися
///    в сутностях під час виконання бізнес-логіки.
/// 
/// 2. Метод `_mediator.Publish(object notification, CancellationToken cancellationToken)`:
///    - Приймає об'єкт події (успадкований від BaseEvent : INotification).
///    - Знаходить УСІ зареєстровані класи `INotificationHandler<T>` у додатку.
///    - Викликає їх метод `Handle()`.
/// 
/// 3. Чому події публікуються саме тут, в Interceptor?
///    Це шаблон "Two-Phase Domain Events":
///    - Етап 1 (у пам'яті): Сутність змінює свій стан і викликає `AddDomainEvent()`. Подія поки НЕ публікується!
///    - Етап 2 (збереження в БД): Викликається `SaveChangesAsync()`. Перехоплювач очищає чергу подій
///      та публікує їх через `_mediator.Publish()`.
///    Це гарантує, що якщо транзакція в БД впаде з помилкою, побічні ефекти (відправка email тощо)
///    не відбудуться з незбереженими даними!
/// </summary>
public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public DispatchDomainEventsInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context is null) return;

        // 1. Знаходимо всі відстежувані ChangeTracker сутності, які мають накопичені події
        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        // 2. Збираємо всі події в один список
        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // 3. Очищаємо події в сутностях, щоб запобігти їх повторній публікації
        entities.ForEach(e => e.ClearDomainEvents());

        // 4. Публікуємо кожну подію через метод _mediator.Publish()
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}
