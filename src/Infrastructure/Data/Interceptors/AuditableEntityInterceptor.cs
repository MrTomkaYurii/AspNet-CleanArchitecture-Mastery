using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors;

/// <summary>
/// EF Core Interceptor для автоматичного аудиту сутностей (авто-заповнення дат та авторів).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (EF Core SaveChangesInterceptor):
/// Замість перевантаження методу SaveChanges у самому DbContext, сучасний EF Core
/// рекомендує використовувати Interceptors.
/// Щоразу перед збереженням у базу:
/// 1. Перехоплювач знаходить усі сутності зі статусом Added або Modified, що успадковані від BaseAuditableEntity.
/// 2. Для нових записів встановлює CreatedAtUtc та CreatedBy.
/// 3. Для оновлених записів встановлює LastModifiedAtUtc та LastModifiedBy.
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTime;

    public AuditableEntityInterceptor(IDateTimeProvider dateTime)
    {
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        var utcNow = _dateTime.UtcNow;
        const string defaultUser = "SystemUser"; // У реальній системі береться з ICurrentUserService / HttpContext

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.CreatedBy ??= defaultUser;
                entry.Entity.CreatedAtUtc = utcNow;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.LastModifiedBy = defaultUser;
                entry.Entity.LastModifiedAtUtc = utcNow;
            }
        }
    }
}
