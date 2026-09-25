using Domain.Enums;
using MediatR;

namespace Application.TodoItems.Commands.CreateTodoItem;

/// <summary>
/// CQRS Command: Створення нового елемента завдання.
/// </summary>
public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Note { get; init; }
    public PriorityLevel Priority { get; init; } = PriorityLevel.None;
    public DateTime? Reminder { get; init; }
}
