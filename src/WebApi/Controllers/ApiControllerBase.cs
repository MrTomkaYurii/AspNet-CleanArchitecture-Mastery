using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Базовий API-контролер для Clean Architecture.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Робота з MediatR у контролерах):
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
    /// 
    /// Метод:
    /// `Mediator.Send(command, cancellationToken)` — надсилає команду або запит у шину MediatR.
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
