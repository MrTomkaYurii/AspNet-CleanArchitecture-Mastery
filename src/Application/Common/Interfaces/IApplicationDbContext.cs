using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

/// <summary>
/// Інтерфейс контексту бази даних для шару Application.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Принцип інверсії залежностей - DIP):
/// У Clean Architecture шар Application описує *потреби* бізнес-логіки через інтерфейси,
/// але не реалізує їх.
/// 
/// Ми визначаємо цей інтерфейс тут (у Application), а конкретну реалізацію (ApplicationDbContext)
/// створюємо в шарі Infrastructure.
/// Завдяки цьому:
/// 1. Шар Application не прив'язаний до SQLite, SQL Server чи деталей ORM.
/// 2. Під час Unit-тестування ми легко можемо підставити Mock-об'єкт IApplicationDbContext.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
