using FluentValidation.Results;

namespace Application.Common.Exceptions;

/// <summary>
/// Виняток валідації запиту (команди або запиту в CQRS).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Коли користувач надсилає команду (наприклад, створити список без назви або з невалідним кольором),
/// валідатор FluentValidation формує перелік помилок.
/// Pipeline Behavior (ValidationBehavior) перехоплює їх і викидає цей виняток.
/// У Presentation шарі глобальний Exception Middleware перетворює його у відповідь 400 Bad Request
/// з детальним JSON описом кожного невалідно заповненого поля за стандартом RFC 7807 (ProblemDetails).
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("Сталася одна або декілька помилок валідації.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
