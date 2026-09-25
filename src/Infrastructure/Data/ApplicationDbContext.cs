using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>
/// Реалізація контексту бази даних Entity Framework Core.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (ApplicationDbContext):
/// 1. Реалізує інтерфейс IApplicationDbContext, визначений у шарі Application.
/// 2. Застосовує перехоплювачі (Interceptors) для аудиту та доменних подій.
/// 3. Метод ApplyConfigurationsFromAssembly автоматично сканує всю поточну збірку
///    і знаходить усі класи, що реалізують IEntityTypeConfiguration<T> (Fluent API).
///    Більше ніколи не потрібно вручну додавати кожен конфігураційний клас до контексту!
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;
    private readonly DispatchDomainEventsInterceptor _dispatchDomainEventsInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor,
        DispatchDomainEventsInterceptor dispatchDomainEventsInterceptor)
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
        _dispatchDomainEventsInterceptor = dispatchDomainEventsInterceptor;
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Підключаємо перехоплювачі до життєвого циклу DbContext
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor, _dispatchDomainEventsInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Автоматично застосовуємо всі класи Fluent API з поточної збірки Infrastructure
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
