using Application.Common.Interfaces;
using Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.TodoLists.Commands.UpdateTodoList;

public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Назва списку є обов'язковою.")
            .MaximumLength(200).WithMessage("Назва списку не повинна перевищувати 200 символів.")
            .MustAsync(BeUniqueTitle).WithMessage("Список із такою назвою вже існує.");

        RuleFor(v => v.Colour)
            .Must(BeValidColour)
            .When(v => !string.IsNullOrEmpty(v.Colour))
            .WithMessage("Вказаний колір не підтримується системою.");
    }

    private async Task<bool> BeUniqueTitle(UpdateTodoListCommand model, string title, CancellationToken cancellationToken)
    {
        return await _context.TodoLists
            .Where(l => l.Id != model.Id)
            .AllAsync(l => l.Title != title, cancellationToken);
    }

    private bool BeValidColour(string? colourCode)
    {
        if (string.IsNullOrEmpty(colourCode)) return true;

        try
        {
            Colour.From(colourCode);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
