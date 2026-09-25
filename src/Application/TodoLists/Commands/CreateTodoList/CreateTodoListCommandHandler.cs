using Application.Common.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.TodoLists.Commands.CreateTodoList;

/// <summary>
/// Обробник команди створення списку завдань.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Інтерфейс IRequestHandler у MediatR):
/// 
/// 1. Що таке інтерфейс `IRequestHandler<TRequest, TResponse>`?
///    Це головний інтерфейс обробника в MediatR.
///    Він зв'язує конкретний запит (TRequest = CreateTodoListCommand) з типом відповіді (TResponse = int).
///    Коли контролер викликає `_sender.Send(command)`, MediatR за допомогою DI контейнера знаходить саме цей клас!
/// 
/// 2. Сигнатура методу Handle:
///    `Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)`
///    
///    Параметри методу:
///    - `request` — екземпляр команди з усіма переданими клієнтом даними.
///    - `cancellationToken` — критично важливий параметр асинхронного програмування в .NET.
///      Якщо користувач скасував HTTP-запит або від'єднався до того, як запис зберігся у базі,
///      цей токен перейде в стан Cancelled. Передаючи його у `_context.SaveChangesAsync(cancellationToken)`,
///      ми гарантуємо, що база даних не витрачатиме ресурси на виконання операцій, які вже нікому не потрібні.
/// 
/// 3. Однозначність (Single Handler Principle):
///    Для кожної команди чи запиту `IRequest` у системі може існувати РІВНО ОДИН `IRequestHandler`.
///    Якщо зареєструвати два хендлери для однієї команди — MediatR викине помилку конфігурації при старті.
/// </summary>
public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Головний метод виконання бізнес-логіки команди створення списку.
    /// </summary>
    public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList
        {
            Title = request.Title
        };

        if (!string.IsNullOrWhiteSpace(request.Colour))
        {
            entity.Colour = Colour.From(request.Colour);
        }

        if (!string.IsNullOrWhiteSpace(request.City) || !string.IsNullOrWhiteSpace(request.Street))
        {
            entity.Location = new Address(
                request.Street ?? string.Empty,
                request.City ?? string.Empty,
                request.State ?? string.Empty,
                request.Country ?? string.Empty,
                request.ZipCode ?? string.Empty);
        }

        _context.TodoLists.Add(entity);

        // Передаємо cancellationToken для безпечного переривання у разі скасування запиту клієнтом
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
