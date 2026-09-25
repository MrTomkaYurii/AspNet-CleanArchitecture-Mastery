using System.Reflection;
using Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Реєстрація служб шару Application у контейнері Dependency Injection.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Конфігурація та методи MediatR):
/// 
/// 1. `services.AddMediatR(cfg => { ... })`:
///    Головний метод розширення для налаштування MediatR у додатку.
/// 
/// 2. `cfg.RegisterServicesFromAssembly(assembly)`:
///    Сканує передану збірку (Assembly) за допомогою рефлексії та автоматично реєструє в DI:
///    - Усі класи, що реалізують `IRequestHandler<TRequest, TResponse>` (обробники команд і запитів);
///    - Усі класи, що реалізують `INotificationHandler<TNotification>` (обробники доменних подій).
///    Завдяки цьому вам не потрібно вручну прописувати `services.AddTransient<IRequestHandler<...>>()`.
/// 
/// 3. Що таке відкритий узагальнений тип (Open Generic Type):
///    `typeof(IPipelineBehavior<,>)` та `typeof(ValidationBehavior<,>)` мають дві коми без зазначення конкретних типів.
///    Це означає, що цей Behavior автоматично застосовуватиметься ДО БУДЬ-ЯКОГО запиту `TRequest`,
///    який відправляється через `ISender.Send()`, незалежно від того, що це за команда чи який тип повертається!
/// 
/// 4. Порядок виклику (Pipeline Execution Order):
///    Поведінки виконуються за принципом цибулини (матроски/middleware):
///    Вхідний запит:
///      -> LoggingBehavior (вхідний лог)
///        -> ValidationBehavior (перевірка валідності)
///          -> PerformanceBehavior (запуск таймера)
///            -> Кінцевий Handler.Handle(...) (виконання бізнес-логіки)
///          <- PerformanceBehavior (зупинка таймера, перевірка >500мс)
///        <- ValidationBehavior
///      <- LoggingBehavior (вихідний лог)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Автоматична реєстрація всіх валідаторів FluentValidation у поточній збірці
        services.AddValidatorsFromAssembly(assembly);

        // Реєстрація MediatR та його конвеєрів (Pipeline Behaviors)
        services.AddMediatR(cfg =>
        {
            // Метод 1: Автоматичне виявлення всіх Handlers у збірці Application
            cfg.RegisterServicesFromAssembly(assembly);

            // Метод 2: Додавання поведінок у конвеєр (Pipeline Behaviors).
            // Порядок реєстрації визначає порядок виклику!

            // Крок 1 конвеєра: Логування початку та завершення запиту
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // Крок 2 конвеєра: Автоматична валідація команди через FluentValidation.
            // Якщо валідація не пройшла, виконання переривається викиданням ValidationException.
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Крок 3 конвеєра: Замір часу виконання запиту через Stopwatch
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        return services;
    }
}
