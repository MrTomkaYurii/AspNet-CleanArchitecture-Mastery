using Application.Common.Interfaces;
using Application.Common.Models;
using Application.TodoItems.Queries.GetTodoItemsWithPagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.TodoItems.Queries.GetTodoItemById;

/// <summary>
/// CQRS Query з використанням Result Pattern: Отримання одного завдання за Id.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Замість `IRequest<TodoItemBriefDto>` (де при відсутності запису довелося б викидати NotFoundException),
/// запит повертає `IRequest<Result<TodoItemBriefDto>>`.
/// Це робить результат явним і безпечним для виклику.
/// </summary>
public record GetTodoItemByIdQuery(int Id) : IRequest<Result<TodoItemBriefDto>>;

/// <summary>
/// Обробник запиту GetTodoItemByIdQuery.
/// </summary>
public class GetTodoItemByIdQueryHandler : IRequestHandler<GetTodoItemByIdQuery, Result<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTodoItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TodoItemBriefDto>> Handle(GetTodoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.TodoItems
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new TodoItemBriefDto
            {
                Id = x.Id,
                ListId = x.ListId,
                Title = x.Title,
                Done = x.Done
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            // Повертаємо структуровану помилку без викидання важкого Exception
            return Error.NotFound("TodoItem.NotFound", $"Завдання з Id {request.Id} не знайдено.");
        }

        return Result<TodoItemBriefDto>.Success(item);
    }
}
