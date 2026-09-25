using Application.Common.Models;
using MediatR;

namespace Application.TodoItems.Queries.GetTodoItemsWithPagination;

/// <summary>
/// CQRS Query: Отримання завдань конкретного списку з пагінацією.
/// </summary>
public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
