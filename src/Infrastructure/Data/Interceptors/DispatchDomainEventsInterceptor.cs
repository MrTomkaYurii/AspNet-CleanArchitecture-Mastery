using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors;

/// <summary>
/// EF Core Interceptor для автоматичної публікації доменних подій.
/// 
/// 📌 КОРОТКО (ЩО ЦЕ ЗА КЛАСИ І НАВІЩО ВОНИ):
/// • `SaveChangesInterceptor` — це вбудований клас Entity Framework Core (пастка / хук / middleware).
///   Він автоматично спрацьовує щоразу, коли десь у системі викликається `_context.SaveChangesAsync()`.
/// • `DispatchDomainEventsInterceptor` — наш клас-«поштовик».
///   Перед тим як записати дані в базу, він автоматично забирає всі доменні події, накопичені в сутностях
///   (наприклад, «Завдання виконано»), і розсилає їх слухачам через MediatR.
/// 
/// ─────────────────────────────────────────────────────────────────────────────
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

    /// <summary>
    /// Автоматично перехоплює синхронний виклик SaveChanges().
    /// </summary>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Автоматично перехоплює асинхронний виклик SaveChangesAsync().
    /// </summary>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Головний алгоритм: дістати всі події із сутностей і розіслати їх через MediatR.
    /// </summary>
    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context is null) return;

        // КРОК 1: Знаходимо в ChangeTracker (кеші пам'яті EF Core) усі сутності, які мають невідправлені події
        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        // КРОК 2: Збираємо всі події в єдиний список
        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // КРОК 3: Очищаємо чергу подій у сутностях, щоб запобігти їх повторній відправці
        entities.ForEach(e => e.ClearDomainEvents());

        // КРОК 4: Публікуємо кожну подію через _mediator.Publish() для всіх зареєстрованих INotificationHandler
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}
