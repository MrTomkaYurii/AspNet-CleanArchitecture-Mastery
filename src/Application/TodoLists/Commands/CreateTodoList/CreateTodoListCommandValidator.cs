using Application.Common.Interfaces;
using Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.TodoLists.Commands.CreateTodoList;

/// <summary>
/// Валідатор для команди створення списку завдань (FluentValidation).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (FluentValidation):
/// Правила валідації описуються декларативно та ізольовано від самої команди:
/// 1. Перевірка обов'язковості та довжини назви.
/// 2. Асинхронна перевірка унікальності назви списку в базі даних (BeUniqueTitle).
/// 3. Перевірка коректності кольору через доменний Value Object.
/// </summary>
public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandValidator(IApplicationDbContext context)
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

    private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        return await _context.TodoLists
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
