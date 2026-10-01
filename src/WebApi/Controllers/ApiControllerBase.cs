using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Базовий API-контролер для Clean Architecture.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Робота з MediatR та Result Pattern у контролерах):
/// 
/// 1. Що таке ISender?
///    У MediatR є два ключових інтерфейси:
///    - `ISender` — призначений для відправки команд та запитів за патерном "Point-to-Point" (Один до одного).
///      У нього є єдиний обробник (Handler).
///    - `IPublisher` — призначений для публікації доменних подій/сповіщень (Publish/Subscribe, Один до багатьох).
///    - `IMediator` — це просто комбінація обох: `public interface IMediator : ISender, IPublisher`.
/// 
/// 2. Чому контролер використовує саме `ISender`, а не `IMediator`?
///    Це пряме слідування принципу розділення інтерфейсів (Interface Segregation Principle - ISP):
///    контролеру ніколи не потрібно напряму викликати `Publish()` для доменних подій (це робить інфраструктура
///    або хендлери), йому потрібен лише метод `Send()`.
/// 
/// 3. Як працює метод `ISender.Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)`:
///    - Приймає об'єкт команди чи запиту (`IRequest`).
///    - Знаходить у DI контейнері зареєстрований `IRequestHandler<TRequest, TResponse>`.
///    - Пропускає запит через ланцюжок зареєстрованих Pipeline Behaviors (Logging, Validation тощо).
///    - Викликає метод `Handle()` відповідного хендлера та повертає результат.
/// 
/// 4. Підтримка Result Pattern:
///    Метод HandleResult() автоматично перетворює об'єкт Result у стандартизовану HTTP-відповідь:
///    - Result.Success(data) -> 200 OK з даними;
///    - Result.Failure(Error.NotFound) -> 404 Not Found з ProblemDetails;
///    - Result.Failure(Error.Validation) -> 400 Bad Request з ProblemDetails;
///    - Result.Failure(Error.Conflict) -> 409 Conflict з ProblemDetails.
///    Це усуває потребу у викиданні винятків для штатних бізнес-помилок!
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// Лінива ініціалізація (Lazy Initialization) через HttpContext.RequestServices.
    /// Це дозволяє дочірнім контролерам не оголошувати конструктор із залежністю ISender,
    /// зберігаючи їхній код максимально лаконічним.
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// Допоміжний метод для автоматичного перетворення Result&lt;T&gt; у відповідний IActionResult.
    /// </summary>
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is not null ? Ok(result.Value) : NoContent();
        }

        return HandleError(result.Error);
    }

    /// <summary>
    /// Допоміжний метод для перетворення нетипізованого Result у IActionResult.
    /// </summary>
    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleError(result.Error);
    }

    private ActionResult HandleError(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Ресурс не знайдено",
                detail: error.Description),

            ErrorType.Validation => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Помилка валідації",
                detail: error.Description),

            ErrorType.Conflict => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Конфлікт стану",
                detail: error.Description),

            ErrorType.Unauthorized => Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Неавторизовано",
                detail: error.Description),

            ErrorType.Forbidden => Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Доступ заборонено",
                detail: error.Description),

            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Внутрішня помилка сервера",
                detail: error.Description)
        };
    }
}
