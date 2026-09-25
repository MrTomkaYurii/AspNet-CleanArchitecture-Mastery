using System.Text.Json;
using Application.Common.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middleware;

/// <summary>
/// Глобальний Middleware для централізованої обробки винятків (Global Exception Handling).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Clean Error Handling):
/// Замість того, щоб у кожному методі контролера писати блоки try-catch:
/// 1. Хендлери в Application або доменні сутності Domain просто викидають типові винятки
///    (ValidationException, NotFoundException, UnsupportedColourException).
/// 2. Цей Middleware перехоплює будь-яку помилку на рівні HTTP-конвеєра.
/// 3. Формує стандартизовану JSON-відповідь відповідно до специфікації RFC 7807 (ProblemDetails).
/// 4. Повертає клієнту відповідний статус (400, 404 або 500), не розкриваючи внутрішні стек-трейси у продакшні.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Виникла помилка під час обробки запиту: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new ValidationProblemDetails(validationEx.Errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Помилка валідації вхідних даних",
                    Detail = "Один або більше параметрів запиту не відповідають бізнес-правилам."
                }
            ),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Ресурс не знайдено",
                    Detail = notFoundEx.Message
                }
            ),

            UnsupportedColourException colourEx => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Непідтримуваний колір",
                    Detail = colourEx.Message
                }
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Внутрішня помилка сервера",
                    Detail = "Під час обробки вашого запиту сталася непередбачена помилка."
                }
            )
        };

        context.Response.StatusCode = statusCode;

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResponse = JsonSerializer.Serialize((object)problemDetails, jsonOptions);

        await context.Response.WriteAsync(jsonResponse);
    }
}
