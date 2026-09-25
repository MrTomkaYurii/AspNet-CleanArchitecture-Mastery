using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

/// <summary>
/// Поведінка конвеєра для виявлення та попередження про повільні запити.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// За допомогою Stopwatch вимірюється точний час виконання операції.
/// Якщо запит триває понад заданий поріг (наприклад, 500 мс), система записує Warning у лог,
/// що допомагає вчасно знаходити вузькі місця у продуктивності (повільні SQL-запити, блокування).
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogWarning(
                "УВАГА: Повільний запит! {RequestName} тривав ({ElapsedMilliseconds} мс) {@Request}",
                requestName, elapsedMilliseconds, request);
        }

        return response;
    }
}
