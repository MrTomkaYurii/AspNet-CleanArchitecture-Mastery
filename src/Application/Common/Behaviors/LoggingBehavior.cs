using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

/// <summary>
/// Поведінка конвеєра для наскрізного логування всіх вхідних запитів MediatR.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (AOP - Aspect-Oriented Programming):
/// Замість того, щоб додавати _logger.LogInformation(...) у кожен із 50 хендлерів,
/// наскрізна функціональність (cross-cutting concern) виноситься в один загальний Behavior.
/// Кожна команда або запит автоматично записується в лог при вході в систему.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Обробка запиту: {RequestName} {@Request}", requestName, request);

        var response = await next();

        _logger.LogInformation("Запит {RequestName} успішно оброблено.", requestName);

        return response;
    }
}
