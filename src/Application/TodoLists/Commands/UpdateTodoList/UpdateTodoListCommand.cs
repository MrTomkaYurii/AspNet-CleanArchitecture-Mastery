using MediatR;

namespace Application.TodoLists.Commands.UpdateTodoList;

/// <summary>
/// CQRS Command: Оновлення назви та властивостей існуючого списку завдань.
/// </summary>
public record UpdateTodoListCommand : IRequest
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Colour { get; init; }

    public string? Street { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public string? ZipCode { get; init; }
}
