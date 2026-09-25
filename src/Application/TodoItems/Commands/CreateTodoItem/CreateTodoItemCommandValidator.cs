using FluentValidation;

namespace Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(v => v.ListId)
            .NotEmpty().WithMessage("ListId є обов'язковим.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Заголовок завдання є обов'язковим.")
            .MaximumLength(200).WithMessage("Заголовок завдання не повинен перевищувати 200 символів.");
    }
}
