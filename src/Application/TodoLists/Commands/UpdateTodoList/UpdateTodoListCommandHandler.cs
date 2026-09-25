using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.TodoLists.Commands.UpdateTodoList;

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(TodoList), request.Id);
        }

        entity.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Colour))
        {
            entity.Colour = Colour.From(request.Colour);
        }

        if (!string.IsNullOrWhiteSpace(request.City) || !string.IsNullOrWhiteSpace(request.Street))
        {
            entity.Location = new Address(
                request.Street ?? string.Empty,
                request.City ?? string.Empty,
                request.State ?? string.Empty,
                request.Country ?? string.Empty,
                request.ZipCode ?? string.Empty);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
