namespace Application.Common.Models;

/// <summary>
/// Типи помилок для Result Pattern.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Замість того, щоб викидати винятки (Exceptions) під час звичайних бізнес-ситуацій,
/// ми класифікуємо помилку за її типом.
/// Це дозволяє Presentation шару (контролерам) автоматично транслювати тип помилки
/// у відповідний HTTP-статус без накладних витрат на створення Stack Trace:
/// - Validation -> 400 Bad Request
/// - NotFound -> 404 Not Found
/// - Conflict -> 409 Conflict
/// - Unauthorized -> 401 Unauthorized
/// - Forbidden -> 403 Forbidden
/// - Failure -> 500 Internal Server Error
/// </summary>
public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5
}

/// <summary>
/// Модель помилки в Result Pattern.
/// Містить зрозумілий код помилки, її опис та категорію (ErrorType).
/// </summary>
public record Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);
}

/// <summary>
/// Універсальна обгортка результату виконання операції (Result Pattern).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Result Pattern vs Exceptions):
/// 1. Чому НЕ Exceptions для бізнес-помилок:
///    Викидання new NotFoundException() у рантаймі змушує .NET збирати повний Stack Trace
///    та розкручувати стек викликів, що у 100-1000 разів повільніше за повернення об'єкта.
///    Крім того, це порушує принцип: "Винятки - лише для виняткових (аварійних) ситуацій".
/// 
/// 2. Явний контракт методу:
///    Сигнатура `Task<Result<TodoItemDto>> Handle(...)` чітко говорить зухвалому розробнику,
///    що операція може закінчитися успіхом або бізнес-помилкою, і результат обов'язково
///    потрібно перевірити (IsSuccess / IsFailure).
/// </summary>
public class Result<TValue>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TValue? Value { get; }
    public Error Error { get; }

    protected Result(TValue value)
    {
        IsSuccess = true;
        Value = value;
        Error = Error.None;
    }

    protected Result(Error error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    /// <summary>
    /// Фабричний метод створення успішного результату з даними.
    /// </summary>
    public static Result<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Фабричний метод створення результату з помилкою.
    /// </summary>
    public static Result<TValue> Failure(Error error) => new(error);

    /// <summary>
    /// Зручне неявне приведення (Implicit Conversion) зі значення у Result.Success.
    /// Дозволяє писати: `return item;` замість `return Result<TodoItem>.Success(item);`.
    /// </summary>
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    /// <summary>
    /// Неявне приведення з Error у Result.Failure.
    /// Дозволяє писати: `return Error.NotFound(...);`.
    /// </summary>
    public static implicit operator Result<TValue>(Error error) => Failure(error);

    /// <summary>
    /// Функціональний метод Match для елегантної обробки обох гілок (успіх / помилка).
    /// </summary>
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error);
    }
}

/// <summary>
/// Нетипізований Result для операцій, які не повертають даних (аналог void / Unit).
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public static implicit operator Result(Error error) => Failure(error);

    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error);
    }
}
