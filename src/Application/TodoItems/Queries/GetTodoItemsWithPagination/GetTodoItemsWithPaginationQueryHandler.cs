using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Handle(
        GetTodoItemsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.TodoItems
            .Where(x => x.ListId == request.ListId)
            .OrderBy(x => x.Title)
            .Select(x => new TodoItemBriefDto
            {
                Id = x.Id,
                ListId = x.ListId,
                Title = x.Title,
                Done = x.Done
            });

        return await PaginatedList<TodoItemBriefDto>.CreateAsync(
            query, request.PageNumber, request.PageSize);
    }
}
