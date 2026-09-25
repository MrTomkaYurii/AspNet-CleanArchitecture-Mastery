using FluentValidation;
using MediatR;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Common.Behaviors;

/// <summary>
/// Поведінка конвеєра MediatR для автоматичної валідації запитів (Pipeline Behavior).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Анатомія IPipelineBehavior у MediatR):
/// 
/// 1. Що таке інтерфейс IPipelineBehavior<TRequest, TResponse>?
///    Це ключовий інтерфейс MediatR для побудови проміжного конвеєра обробки (як ASP.NET Middleware).
///    Він перехоплює виконання запиту TRequest перед тим, як той дійде до IRequestHandler.
/// 
/// 2. Сигнатура методу Handle:
///    `Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)`
///    
///    Параметри:
///    - `request` (TRequest) — сам об'єкт команди чи запиту, який був переданий у `ISender.Send(command)`.
///    - `next` (RequestHandlerDelegate<TResponse>) — це системний делегат MediatR:
///      `public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();`
///      Виклик `await next()` викликає наступний зареєстрований Behavior або, якщо цей Behavior останній,
///      викликає метод `Handle()` безпосередньо у бізнес-хендлері (IRequestHandler).
///    - `cancellationToken` — токен відміни операції (наприклад, якщо клієнт закрив вкладку браузера).
/// 
/// 3. Як працює ланцюжок (Short-circuiting):
///    Якщо валідація не проходить, ми викидаємо `ValidationException` ДО виклику `await next()`.
///    Таким чином бізнес-хендлер (IRequestHandler) взагалі не викликається, база даних не навантажується!
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Метод-перехоплювач конвеєра MediatR.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Якщо для цієї команди TRequest зареєстровано хоча б один валідатор FluentValidation
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            // Виконуємо всі валідатори паралельно
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Збираємо список усіх помилок
            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            // Якщо є помилки — зупиняємо конвеєр (не викликаємо next()) та викидаємо виняток
            if (failures.Any())
            {
                throw new ValidationException(failures);
            }
        }

        // Передаємо виконання далі: наступному Behavior або цільовому IRequestHandler
        return await next();
    }
}
