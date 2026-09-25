using MediatR;

namespace Application.TodoLists.Commands.CreateTodoList;

/// <summary>
/// CQRS Command: Створення нового списку завдань.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Інтерфейс IRequest<TResponse> у MediatR):
/// 
/// 1. Що таке інтерфейс `IRequest<TResponse>`?
///    Це маркерний інтерфейс (Marker Interface) бібліотеки MediatR.
///    Він вказує системі, що цей клас є повідомленням типу "Запит" (Request),
///    який відправляється через `ISender.Send()`, і що в результаті його обробки
///    буде повернуто значення типу `TResponse` (у даному випадку `int` — ідентифікатор створеного списку).
/// 
/// 2. Різниця між `IRequest<TResponse>` та `IRequest`:
///    - `IRequest<TResponse>` — використовується, коли команда чи запит повертає результат (наприклад, ID, DTO, boolean).
///    - `IRequest` (без дженеріка) — використовується, коли команда нічого не повертає (аналог `void`,
///      під капотом MediatR повертає спеціальний тип `Unit`). Наприклад, DeleteTodoListCommand.
/// 
/// 3. Чому record, а не class?
///    Використання C# `record` ідеально підходить для DTO, команд і запитів,
///    оскільки забезпечує незмінність (immutability) через властивості з `init`
///    та автоматичне порівняння за значеннями полів.
/// </summary>
public record CreateTodoListCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public string? Colour { get; init; }

    // Поля для побудови Value Object адреси (опціонально)
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public string? ZipCode { get; init; }
}
