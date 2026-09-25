namespace Application.Common.Exceptions;

/// <summary>
/// Виняток, що сигналізує про відсутність запитуваної сутності в базі даних.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Якщо клієнт звертається за Id, якого не існує (наприклад, TodoList з Id=999),
/// хендлер викидає NotFoundException.
/// Presentation шар (Middleware) автоматично перехоплює цей виняток і повертає
/// клієнту зрозумілу HTTP-відповідь 404 Not Found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Сутність \"{name}\" з ключем ({key}) не знайдена.")
    {
    }
}
