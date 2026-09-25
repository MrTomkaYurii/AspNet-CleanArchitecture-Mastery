using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.TodoLists.Queries.GetTodoLists;

/// <summary>
/// Обробник запиту GetTodoListsQuery.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Оптимізація читання в EF Core):
/// 1. AsNoTracking():
///    Оскільки це Query (читання), ми вимикаємо відстеження змін у ChangeTracker EF Core.
///    Це суттєво економить пам'ять та прискорює виконання запиту.
/// 2. Мапінг у DTO виконується безпосередньо через LINQ Select:
///    Це дозволяє EF Core згенерувати SQL, який вибирає з бази лише потрібні колонки (Projectables).
/// </summary>
public class GetTodoListsQueryHandler : IRequestHandler<GetTodoListsQuery, TodoListsVm>
{
    private readonly IApplicationDbContext _context;

    public GetTodoListsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoListsVm> Handle(GetTodoListsQuery request, CancellationToken cancellationToken)
    {
        var priorityLevels = Enum.GetValues(typeof(PriorityLevel))
            .Cast<PriorityLevel>()
            .Select(p => new PriorityLevelDto
            {
                Value = (int)p,
                Name = p.ToString()
            })
            .ToList();

        var lists = await _context.TodoLists
            .AsNoTracking()
            .OrderBy(t => t.Title)
            .Select(t => new TodoListDto
            {
                Id = t.Id,
                Title = t.Title,
                Colour = t.Colour.Code,
                Location = t.Location != null ? t.Location.ToString() : null,
                Items = t.Items.Select(i => new TodoItemDto
                {
                    Id = i.Id,
                    ListId = i.ListId,
                    Title = i.Title,
                    Note = i.Note,
                    Done = i.Done,
                    Priority = (int)i.Priority
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new TodoListsVm
        {
            PriorityLevels = priorityLevels,
            Lists = lists
        };
    }
}
