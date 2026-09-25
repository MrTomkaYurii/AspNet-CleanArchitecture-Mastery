using Domain.Enums;
using MediatR;

namespace Application.TodoItems.Commands.UpdateTodoItem;

/// <summary>
/// CQRS Command: Оновлення завдання (включаючи назву, примітку, пріоритет та статус виконання).
/// </summary>
public record UpdateTodoItemCommand : IRequest
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Note { get; init; }
    public PriorityLevel Priority { get; init; } = PriorityLevel.None;
    public DateTime? Reminder { get; init; }
    public bool Done { get; init; }
}
