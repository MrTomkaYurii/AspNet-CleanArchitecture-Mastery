using Microsoft.EntityFrameworkCore;

namespace Application.Common.Models;

/// <summary>
/// Універсальна обгортка для результатів запитів з пагінацією (розбиттям на сторінки).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Пагінація в CQRS Queries):
/// При читанні великих списків (наприклад, тисячі завдань) неприпустимо завантажувати
/// всю таблицю в оперативну пам'ять (ToListAsync()).
/// Замість цього виконується підрахунок кількості (CountAsync) і вибірка потрібної сторінки
/// за допомогою LINQ операторів .Skip() та .Take() безпосередньо в базі даних на рівні SQL-запиту:
/// SELECT ... LIMIT @pageSize OFFSET @skip.
/// </summary>
/// <typeparam name="T">Тип елементів у списку.</typeparam>
public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Асинхронний фабричний метод для побудови пагінованого результату безпосередньо з IQueryable.
    /// </summary>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
